// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.IO;
using System.Threading;
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
        private readonly IApiTicketHandler ApiTicketHandler;

        /// <summary>
        /// The ResourceHandler implements comfort functions to Deploy a single Resource or Download a WebApp Resource
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        /// <param name="apiWebAppResourceBuilder">used to create a webappresource from a given filename and webappdirectorypath - set resource protection to resourceVisibility given</param>
        /// <param name="apiTicketHandler">Handler for the Ticketing Endpoint of the PLC</param>
        public ApiResourceHandler(IApiRequestHandler apiRequestHandler, IApiWebAppResourceBuilder apiWebAppResourceBuilder, IApiTicketHandler apiTicketHandler)
        {
            ApiRequestHandler = apiRequestHandler;
            ApiWebAppResourceBuilder = apiWebAppResourceBuilder;
            ApiTicketHandler = apiTicketHandler;
        }

        /// <summary>
        /// make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!</param>
        /// <param name="resource">the resource you want to deploy - MUST have information:
        /// <see cref="ApiWebAppResource.Name"/> 
        /// <see cref="ApiWebAppResource.Media_type"/>
        /// <see cref="ApiWebAppResource.Last_modified"/>
        /// optionally:</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <see cref="ApiWebAppResource.Etag"/>, <see cref="ApiWebAppResource.Visibility"/>
        /// <returns>Task</returns>
        public async Task DeployResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default)
        {
            string path = webApp.PathToWebAppDirectory + @"\" + resource.Name.Replace("/", "\\");
            if (!File.Exists(path))
                throw new FileNotFoundException($"file at: {path} has not been found - did you set the webApp PathToWebAppDirectory correctly? given: {Environment.NewLine + webApp.PathToWebAppDirectory}");
            var ticketIdResponse = await ApiRequestHandler.WebAppCreateResourceAsync(webApp.Name, resource.Name, resource.Media_type, resource.Last_modified.ToString(DateTimeFormatting.ApiDateTimeFormat), resource.Visibility, resource.Etag, cancellationToken);
            string ticketId = ticketIdResponse.Result;
            await ApiTicketHandler.HandleUploadAsync(ticketId, path, cancellationToken);
        }
        /// <summary>
        /// make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!</param>
        /// <param name="resource">the resource you want to deploy - MUST have information:
        /// <see cref="ApiWebAppResource.Name"/> 
        /// <see cref="ApiWebAppResource.Media_type"/>
        /// <see cref="ApiWebAppResource.Last_modified"/>
        /// optionally:</param>
        /// <see cref="ApiWebAppResource.Etag"/>, <see cref="ApiWebAppResource.Visibility"/>
        public void DeployResource(ApiWebAppData webApp, ApiWebAppResource resource)
            => DeployResourceAsync(webApp, resource).GetAwaiter().GetResult();

        /// <summary>
        /// make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set! defaults to public</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        public async Task DeployResourceAsync(ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public, CancellationToken cancellationToken = default)
        {
            var resource = ApiWebAppResourceBuilder.BuildResourceFromFile(pathToResource, webApp.PathToWebAppDirectory, visibility);
            await DeployResourceAsync(webApp, resource, cancellationToken);
        }
        /// <summary>
        /// make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the <see cref="ApiWebAppData.PathToWebAppDirectory"/> before calling this method!</param>
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
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="overrideExistingFile">choose whether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>Task/void</returns>
        public async Task<FileInfo> DownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, CancellationToken cancellationToken = default)
        {
            var ticketId = (await ApiRequestHandler.WebAppDownloadResourceAsync(webApp, resource, cancellationToken)).Result;
            string fileNameToUse = fileName ?? resource.Name;
            return (await ApiTicketHandler.HandleDownloadAsync(ticketId, pathToDownloadDirectory, fileNameToUse, fileExtension, overrideExistingFile, cancellationToken)).File_Downloaded;
        }
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="fileName">will default to "resource.name"</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose whether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        public FileInfo DownloadResource(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null)
            => DownloadResourceAsync(webApp, resource, overrideExistingFile, pathToDownloadDirectory, fileName, fileExtension).GetAwaiter().GetResult();
    }
}
