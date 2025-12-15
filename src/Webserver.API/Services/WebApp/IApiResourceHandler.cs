// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// The ResourceHandler implements comfort functions to Deploy a single Resource or Download a WebApp Resource
    /// </summary>
    public interface IApiResourceHandler
    {
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
        void DeployResource(ApiWebAppData webApp, ApiWebAppResource resource);
        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set!</param>
        /// <returns></returns>
        void DeployResource(ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public);
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
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// etag, visibility
        /// <returns></returns>
        Task DeployResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, CancellationToken cancellationToken = default);
        /// <summary>
        /// make sure to set the webapp PathToWebAppDirectory before calling this method!
        /// Will send a Createresource, Uploadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">make sure to set the webapp PathToWebAppDirectory before calling this method!</param>
        /// <param name="pathToResource">filepath to the resource that should be deployed!</param>
        /// <param name="visibility">Visibility for the resource that shall be set!</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns></returns>
        Task DeployResourceAsync(ApiWebAppData webApp, string pathToResource, ApiWebAppResourceVisibility visibility = ApiWebAppResourceVisibility.Public, CancellationToken cancellationToken = default);
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">Directory the file should be stored in</param>
        /// <param name="fileName">name for the downloaded file</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="overrideExistingFile">choose whether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        FileInfo DownloadResource(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null);
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="webApp">Webapp that contains the resource you want</param>
        /// <param name="resource">the resource you want to download (Name must match filename on the webapp)</param>
        /// <param name="pathToDownloadDirectory">Directory the file should be stored in</param>
        /// <param name="fileName">name for the downloaded file</param>
        /// <param name="fileExtension">in case you want to set a specific fileExtension (normally included in filename)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="overrideExistingFile">choose whether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>task/void</returns>
        Task<FileInfo> DownloadResourceAsync(ApiWebAppData webApp, ApiWebAppResource resource, bool overrideExistingFile = false, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, CancellationToken cancellationToken = default);
    }
}