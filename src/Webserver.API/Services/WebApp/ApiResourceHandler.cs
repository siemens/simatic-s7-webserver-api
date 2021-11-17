// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// The ResourceHandler implements comfort functions to Deploy a single Resource or Download a WebApp Resource
    /// </summary>
    public class ApiResourceHandler : IApiResourceHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;

        private readonly IApiWebAppResourceBuilder ApiWebAppResourceBuilder;

        /// <summary>
        /// The ResourceHandler implements comfort functions to Deploy a single Resource or Download a WebApp Resource
        /// </summary>
        /// <param name="apiRequestHandler"></param>
        /// <param name="apiWebAppResourceBuilder"></param>
        public ApiResourceHandler(IApiRequestHandler apiRequestHandler, IApiWebAppResourceBuilder apiWebAppResourceBuilder)
        {
            this.ApiRequestHandler = apiRequestHandler;
            this.ApiWebAppResourceBuilder = apiWebAppResourceBuilder;
        }

        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="resource">the resource you want to deploy - MUST have information:
        /// Name
        /// Media_type
        /// Last_modified
        /// optionally:</param>
        /// etag, visibility
        /// <returns></returns>
        public async Task DeployResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource)
        {
            string path = webApp.PathToWebAppDirectory + @"\" + resource.Name.Replace("/", "\\");
            if (!File.Exists(path))
                throw new FileNotFoundException($"file at: {path} has not been found - did you set the webApp PathToWebAppDirectory correctly? given: {Environment.NewLine + webApp.PathToWebAppDirectory}");
            var ticketIdResponse = await ApiRequestHandler.WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type, resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag);
            string ticketId = ticketIdResponse.Result;
            try
            {
                await ApiRequestHandler.UploadTicketAsync(ticketId, path);
            }
            catch (ApiTicketingEndpointUploadException)
            {
                await ApiRequestHandler.ApiCloseTicketAsync(ticketId);
                throw;
            }
            try
            {
                await ApiRequestHandler.ApiCloseTicketAsync(ticketId);
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
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="resource">the resource you want to deploy - MUST have information:
        /// Name
        /// Media_type
        /// Last_modified
        /// optionally:</param>
        /// etag, visibility
        /// <returns></returns>
        public void DeployResource(ApiWebAppData webApp, ApiWebAppResource resource)
            => DeployResourceAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <returns></returns>
        public async Task DeployResourceAsync(ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public)
        {
            var resource = ApiWebAppResourceBuilder.BuildResourceFromFile(pathToResource, webApp.PathToWebAppDirectory, visibility);
            await DeployResourceAsync(webApp, resource);
        }
        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <returns></returns>
        public void DeployResource(ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public)
            => DeployResourceAsync(webApp, pathToResource, visibility).GetAwaiter().GetResult();

        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        public async Task DownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
        {
            if(pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            //Downloads: 374DE290-123F-4565-9164-39C4925E467B
            string usedPathToDownloadDirectory = pathToDownloadDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads");
            string usedFilename = fileName ?? resource.Name;
            string usedFileExtension = fileExtension ?? "";
            var ticketIdResponse = (await ApiRequestHandler.WebAppDownloadResourceAsync(webApp, resource)).Result;
            var content = await ApiRequestHandler.DownloadTicketAsync(ticketIdResponse);
            await ApiRequestHandler.ApiCloseTicketAsync(ticketIdResponse);
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
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        public void DownloadResource(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
            => DownloadResource(webApp, resource, overrideExistingFile, pathToDownloadDirectory, fileName, fileExtension);
    }
}
