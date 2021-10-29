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
    /// Extension methods for the RequestHandler => DeployResource, DownloadResource
    /// </summary>
    public static class ApiRequestHandlerExtensions
    {
        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static void DeployResource(this IApiRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource)
        {
            var ticketIdResponse = requestHandler.WebAppCreateResource(webApp.Name, resource.Name, resource.Media_type, resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
            string ticketId = ticketIdResponse.Result;
            string path = webApp.PathToWebAppDirectory + @"\" + resource.Name.Replace("/", "\\");
            if (!File.Exists(path))
                throw new FileNotFoundException($"file at: {path} has not been found - did you set the webApp PathToWebAppDirectory correctly? given: {Environment.NewLine + webApp.PathToWebAppDirectory}");
            try
            {
                requestHandler.UploadTicket(ticketId, path);
            }
            catch (ApiTicketingEndpointUploadException)
            {
            }
            try
            {
                ApiTicket ticket = (requestHandler.ApiBrowseTickets(ticketId)).Result.Tickets.First();
                requestHandler.ApiCloseTicket(ticketId);
                if (ticket.State != ApiTicketState.Completed)
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
            catch (ApiTicketNotInCompletedStateException ex)
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
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource"></param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <returns></returns>
        public static void DeployResource(this IApiRequestHandler requestHandler, ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public)
        {
            var resource = ApiWebAppResourceBuilder.BuildResourceFromFile(pathToResource, webApp.PathToWebAppDirectory, visibility);
            DeployResource(requestHandler, webApp, resource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <param name="webApp"></param>
        /// <param name="resource"></param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension"></param>
        /// <param name="overrideExistingFile"></param>
        /// <returns></returns>
        public static void DownloadResource(this IApiRequestHandler requestHandler, ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
        {
            //Downloads: 374DE290-123F-4565-9164-39C4925E467B
            string usedPathToDownloadDirectory = pathToDownloadDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads");//Environment.GetFolderPath(Environment.SpecialFolder.d;
            string usedFilename = fileName ?? resource.Name;
            string usedFileExtension = fileExtension ?? "";
            var ticketIdResponse = (requestHandler.WebAppDownloadResource(webApp, resource)).Result;
            var content = requestHandler.DownloadTicket(ticketIdResponse);
            requestHandler.ApiCloseTicket(ticketIdResponse);
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
        public static ApiPlcProgramBrowseResponse PlcProgramBrowseSetChildrenAndParents(this IApiRequestHandler requestHandler, ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        {
            string varName = var.GetVarNameForMethods();
            var response = requestHandler.PlcProgramBrowse(plcProgramBrowseMode, varName);
            if (plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children)
            {
                response.Result.ForEach(el =>
                {
                    el.Parents = new List<ApiPlcProgramData>(var.Parents);
                    el.Parents.Add(var);
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
        public static ApiPlcProgramData PlcProgramReadStructByChildValues(this IApiRequestHandler requestHandler,
            ApiPlcProgramData structToRead, ApiPlcProgramReadOrWriteMode childrenReadMode = ApiPlcProgramReadOrWriteMode.Simple,
            int threadSleepTimeInMilliseconds = 16)
        {
            var toReturn = structToRead.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                PlcProgramBrowseSetChildrenAndParents(requestHandler, ApiPlcProgramBrowseMode.Children, toReturn);
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory();
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramReadRequest(child.GetVarNameForMethods(), childrenReadMode));
                Thread.Sleep(threadSleepTimeInMilliseconds);
            }
            requests.MakeSureRequestIdsAreUnique(threadSleepTimeInMilliseconds: threadSleepTimeInMilliseconds);
            var childvalues = requestHandler.ApiBulk(requests);
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
        public static void PlcProgramWriteStructByChildValues(this IApiRequestHandler requestHandler,
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
            requestHandler.ApiBulk(requests);
        }
    }
}
