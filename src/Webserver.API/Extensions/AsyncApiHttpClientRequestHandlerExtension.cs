// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.RequestHandler;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Responses;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Extensions
{
    /// <summary>
    /// Extension methods for the AsyncRequestHandler and the ApiHttpClientRequestHandler => DeployResource, DownloadResource
    /// </summary>
    public static class AsyncApiHttpClientRequestHandlerExtension
    {
        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task DeployResourceAsync(this IAsyncApiRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource)
        {
            var ticketIdResponse = await requestHandler.WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type, resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
            string ticketId = ticketIdResponse.Result;
            string path = webApp.PathToWebAppDirectory + @"\" + resource.Name.Replace("/", "\\");
            if (!File.Exists(path))
                throw new FileNotFoundException($"file at: {path} has not been found - did you set the webApp PathToWebAppDirectory correctly? given: {Environment.NewLine + webApp.PathToWebAppDirectory}");
            try
            {
                await requestHandler.UploadTicketAsync(ticketId, path);
            }
            catch(ApiTicketingEndpointUploadException)
            {
            }
            try
            {
                ApiTicket ticket = (await requestHandler.ApiBrowseTicketsAsync(ticketId)).Result.Tickets.First();
                await requestHandler.ApiCloseTicketAsync(ticketId);
                if (ticket.State != Enums.ApiTicketState.Completed)
                {
                    throw new ApiTicketNotInCompletedStateException(ticket);
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                throw new System.Net.Http.HttpRequestException($"ticketId was: {ticketId} file was:" +
                    $"{Environment.NewLine + resource.Name} size:" +
                    $"{Environment.NewLine + resource.Size}:"
                    , ex);
            }
            catch(ApiTicketNotInCompletedStateException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"ticketId was: {ticketId} file was:" +
                    $"{Environment.NewLine + resource.Name} size:" +
                    $"{Environment.NewLine + resource.Size}:"
                    , ex);
            }
            
        }

        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="resource">the resource you want to deploy - MUST have information:
        /// Name
        /// Media_type
        /// Last_modified
        /// optionally:</param>
        /// etag, visibility
        /// <returns></returns>
        public static async Task DeployResourceAsync(this ApiHttpClientRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource)
        {
            string path = webApp.PathToWebAppDirectory + @"\" + resource.Name.Replace("/", "\\");
            if (!File.Exists(path))
                throw new FileNotFoundException($"file at: {path} has not been found - did you set the webApp PathToWebAppDirectory correctly? given: {Environment.NewLine + webApp.PathToWebAppDirectory}");
            var ticketIdResponse = await requestHandler.WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type, resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
            string ticketId = ticketIdResponse.Result;
            try
            {
                await requestHandler.UploadTicketAsync(ticketId, path);
            }
            catch(ApiTicketingEndpointUploadException ex)
            {
                await requestHandler.ApiCloseTicketAsync(ticketId);
                throw ex;
            }
            try
            {
                await requestHandler.ApiCloseTicketAsync(ticketId);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                throw new System.Net.Http.HttpRequestException($"ticketId was: {ticketId} file was:" +
                    $"{Environment.NewLine + resource.Name} size:" +
                    $"{Environment.NewLine + resource.Size}:"
                    , ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"ticketId was: {ticketId} file was:" +
                    $"{Environment.NewLine + resource.Name} size:" +
                    $"{Environment.NewLine + resource.Size}:"
                    , ex);
            }

        }

        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource you want to deploy</param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <returns></returns>
        public static async Task DeployResourceAsync(this IAsyncApiRequestHandler requestHandler, ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public)
        {
            var resource = ApiWebAppResourceBuilder.BuildResourceFromFile(pathToResource, webApp.PathToWebAppDirectory, visibility);
            await DeployResourceAsync(requestHandler, webApp, resource);
        }

        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <returns></returns>
        public static async Task DeployResourceAsync(this ApiHttpClientRequestHandler requestHandler, ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public)
        {
            var resource = ApiWebAppResourceBuilder.BuildResourceFromFile(pathToResource, webApp.PathToWebAppDirectory, visibility);
            await DeployResourceAsync(requestHandler, webApp, resource);
        }

        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        public static async Task DownloadResourceAsync(this IAsyncApiRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
        {
            //Downloads: 374DE290-123F-4565-9164-39C4925E467B
            string usedPathToDownloadDirectory = pathToDownloadDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads");
            string usedFilename = fileName ?? resource.Name;
            string usedFileExtension = fileExtension??"";
            var ticketIdResponse = (await requestHandler.WebAppDownloadResourceAsync(webApp, resource)).Result;
            var content = await requestHandler.DownloadTicketAsync(ticketIdResponse);
            await requestHandler.ApiCloseTicketAsync(ticketIdResponse);
            string path = Path.Combine(usedPathToDownloadDirectory, usedFilename + usedFileExtension);
            uint counter = 0;
            var firstPath = path;
            while (File.Exists(path) && !overrideExistingFile)
            {
                FileInfo fileInfo = new FileInfo(path);
                DirectoryInfo dir = fileInfo.Directory;
                path = Path.Combine(dir.FullName, (Path.GetFileNameWithoutExtension(firstPath) + "(" + counter + ")" + fileInfo.Extension));
                counter++;
            }
            if(resource.Name.Contains("/"))
            {
                var split = resource.Name.Split('/');
                var paths = "";
                foreach(var s in split)
                {
                    if (s == split.Last())
                        continue;
                    paths += $"\\{s}";
                    if(!Directory.Exists(usedPathToDownloadDirectory+paths))
                    {
                        Directory.CreateDirectory(usedPathToDownloadDirectory + paths);
                    }
                }
            }
            using (FileStream fs = File.Create(path))
            {
                fs.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="requestHandler">an Implementation of the IAsyncApiRequestHandler</param>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        public static async Task DownloadResourceAsync(this ApiHttpClientRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
        {
            //Downloads: 374DE290-123F-4565-9164-39C4925E467B
            string usedPathToDownloadDirectory = pathToDownloadDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads");
            string usedFilename = fileName ?? resource.Name;
            string usedFileExtension = fileExtension ?? "";
            var ticketIdResponse = (await requestHandler.WebAppDownloadResourceAsync(webApp, resource)).Result;
            var content = await requestHandler.DownloadTicketAsync(ticketIdResponse);
            await requestHandler.ApiCloseTicketAsync(ticketIdResponse);
            string path = Path.Combine(usedPathToDownloadDirectory, usedFilename + usedFileExtension);
            uint counter = 0;
            var firstPath = path;
            while (File.Exists(path) && !overrideExistingFile)
            {
                FileInfo fileInfo = new FileInfo(path);
                DirectoryInfo dir = fileInfo.Directory;
                path = Path.Combine(dir.FullName, (Path.GetFileNameWithoutExtension(firstPath) + "(" + counter + ")" + fileInfo.Extension));
                counter++;
            }
            if (resource.Name.Contains("/"))
            {
                var split = resource.Name.Split('/');
                var paths = "";
                foreach (var s in split)
                {
                    if (s == split.Last())
                        continue;
                    paths += $"\\{s}";
                    if (!Directory.Exists(usedPathToDownloadDirectory + paths))
                    {
                        Directory.CreateDirectory(usedPathToDownloadDirectory + paths);
                    }
                }
            }
            using (FileStream fs = File.Create(path))
            {
                fs.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// If plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children: ELSE "normal PlcProgramBrowse implementation"
        /// PlcProgramBrowse that will add the Elements from the response to the children of given var, also adds the parents of var to the list of parents of the children and also var as parent
        /// </summary>
        /// <param name="requestHandler">Api RequestHandler used to send the request</param>
        /// <param name="plcProgramBrowseMode">Mode for PlcProgramBrowse function</param>
        /// <param name="var">the db/structure of which the children should be browsed</param>
        /// <returns>ApiResultResponse of List of ApiPlcProgramData containing the children of the given var</returns>
        public static async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseSetChildrenAndParentsAsync(this ApiHttpClientRequestHandler requestHandler, ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        {
            var response = await requestHandler.PlcProgramBrowseAsync(plcProgramBrowseMode, var);
            if (plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children)
            {
                response.Result.ForEach(el =>
                {
                    el.Parents = new List<ApiPlcProgramData>(var.Parents);
                    el.Parents.Add(var);
                    if (var.Children == null)
                    {
                        var.Children = new List<ApiPlcProgramData>();
                    }
                    if (!var.Children.Any(child => child.Equals(el)))
                    {
                        var.Children.Add(el);
                    }
                });
            }
            return response;
        }

        /// <summary>
        /// If plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children: ELSE "normal PlcProgramBrowse implementation"
        /// PlcProgramBrowse that will add the Elements from the response to the children of given var, also adds the parents of var to the list of parents of the children and also var as parent
        /// </summary>
        /// <param name="requestHandler">Api RequestHandler used to send the request</param>
        /// <param name="plcProgramBrowseMode">Mode for PlcProgramBrowse function</param>
        /// <param name="var">the db/structure of which the children should be browsed</param>
        /// <returns>ApiResultResponse of List of ApiPlcProgramData containing the children of the given var</returns>
        public static async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseSetChildrenAndParentsAsync(this IAsyncApiRequestHandler requestHandler, ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        {
            string varName = var.GetVarNameForMethods();
            var response = await requestHandler.PlcProgramBrowseAsync(plcProgramBrowseMode, varName);
            if (plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children)
            {
                response.Result.ForEach(el =>
                {
                    el.Parents = new List<ApiPlcProgramData>(var.Parents);
                    el.Parents.Add(var);
                    if(var.Children == null)
                    {
                        var.Children = new List<ApiPlcProgramData>();
                    }
                    if (!var.Children.Any(child => child.Equals(el)))
                    {
                        var.Children.Add(el);
                    }
                });
            }
            return response;
        }

        /// <summary>
        /// Method to comfortably read all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToRead">Struct of which the Children should be Read by Bulk Request</param>
        /// <param name="childrenReadMode">Mode in which the child values should be read - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public static async Task<ApiPlcProgramData> PlcProgramReadStructByChildValuesAsync(this ApiHttpClientRequestHandler requestHandler, 
            ApiPlcProgramData structToRead, ApiPlcProgramReadOrWriteMode childrenReadMode = ApiPlcProgramReadOrWriteMode.Simple,
            int threadSleepTimeInMilliseconds = 16)
        {
            var toReturn = structToRead.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                await PlcProgramBrowseSetChildrenAndParentsAsync(requestHandler, ApiPlcProgramBrowseMode.Children, toReturn);
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory();
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramReadRequest(child.GetVarNameForMethods(), childrenReadMode));
                Thread.Sleep(threadSleepTimeInMilliseconds);
            }
            requests.MakeSureRequestIdsAreUnique(threadSleepTimeInMilliseconds: threadSleepTimeInMilliseconds);
            var childvalues = await requestHandler.ApiBulkAsync(requests);
            foreach (var childval in childvalues.SuccessfulResponses)
            {
                var accordingRequest = requests.First(el => el.Id == childval.Id);
                var requestedVarString = accordingRequest.Params["var"];
                var childWithVarString = toReturn.Children.First(el => el.GetVarNameForMethods() == (string)requestedVarString);
                childWithVarString.Value = childval.Result;
            }
            return toReturn;
        }

        /// <summary>
        /// Method to comfortably read all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToRead">Struct of which the Children should be Read by Bulk Request</param>
        /// <param name="childrenReadMode">Mode in which the child values should be read - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public static async Task<ApiPlcProgramData> PlcProgramReadStructByChildValuesAsync(this IAsyncApiRequestHandler requestHandler, 
            ApiPlcProgramData structToRead, ApiPlcProgramReadOrWriteMode childrenReadMode = ApiPlcProgramReadOrWriteMode.Simple,
            int threadSleepTimeInMilliseconds = 16)
        {
            var toReturn = structToRead.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                await PlcProgramBrowseSetChildrenAndParentsAsync(requestHandler, ApiPlcProgramBrowseMode.Children, toReturn);
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory();
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramReadRequest(child.GetVarNameForMethods(), childrenReadMode));
                Thread.Sleep(threadSleepTimeInMilliseconds);
            }
            requests.MakeSureRequestIdsAreUnique(threadSleepTimeInMilliseconds: threadSleepTimeInMilliseconds);
            var childvalues = await requestHandler.ApiBulkAsync(requests);
            foreach (var childval in childvalues.SuccessfulResponses)
            {
                var accordingRequest = requests.First(el => el.Id == childval.Id);
                var requestedVarString = accordingRequest.Params["var"];
                var childWithVarString = toReturn.Children.First(el => el.GetVarNameForMethods() == (string)requestedVarString);
                childWithVarString.Value = childval.Result;
            }
            return toReturn;
        }

        /// <summary>
        /// Method to comfortably write all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToWrite">Struct of which the Children should be written by Bulk Request</param>
        /// <param name="childrenWriteMode">Mode in which the child values should be written - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public static async Task PlcProgramWriteStructByChildValuesAsync(this IAsyncApiRequestHandler requestHandler,
            ApiPlcProgramData structToWrite, ApiPlcProgramReadOrWriteMode childrenWriteMode = ApiPlcProgramReadOrWriteMode.Simple, 
            int threadSleepTimeInMilliseconds = 16)
        {
            var toReturn = structToWrite.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                throw new Exception($"No child elements present on var {toReturn.GetVarNameForMethods()}!");
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory();
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramWriteRequest(child.GetVarNameForMethods(), child.Value, childrenWriteMode));
                Thread.Sleep(threadSleepTimeInMilliseconds);
            }
            requests.MakeSureRequestIdsAreUnique(threadSleepTimeInMilliseconds: threadSleepTimeInMilliseconds);
            await requestHandler.ApiBulkAsync(requests);
        }

        /// <summary>
        /// Method to comfortably write all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToWrite">Struct of which the Children should be written by Bulk Request</param>
        /// <param name="childrenWriteMode">Mode in which the child values should be written - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public static async Task PlcProgramWriteStructByChildValuesAsync(this ApiHttpClientRequestHandler requestHandler,
            ApiPlcProgramData structToWrite, ApiPlcProgramReadOrWriteMode childrenWriteMode = ApiPlcProgramReadOrWriteMode.Simple, 
            int threadSleepTimeInMilliseconds = 16)
        {
            var toReturn = structToWrite.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                throw new Exception($"No child elements present on var {toReturn.GetVarNameForMethods()}!");
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory();
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramWriteRequest(child.GetVarNameForMethods(), child.Value, childrenWriteMode));
                Thread.Sleep(threadSleepTimeInMilliseconds);
            }
            requests.MakeSureRequestIdsAreUnique(threadSleepTimeInMilliseconds: threadSleepTimeInMilliseconds);
            await requestHandler.ApiBulkAsync(requests);
        }


    }
}
