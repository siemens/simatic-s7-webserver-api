// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Handler for Api requests the plcs Files and Datalogs requests.
    /// BrowseFiles/Async - sends a Files.Browse Request with a resource
    /// DownloadFile/Async - Send a Files.Download, Downloadticket and Api.Closeticket request to the API. Creates a file ticket which the client can use to retrieve a file from the PLC.
    /// CreateFileOnPlc/Async - Createsa file ticket with the Files.Create request which the client can use to transfer a file to the PLC. This is referred to as "uploading" a file to the PLC.
    /// CreateDirectoryOnPlc/Async - Create a directory on the PLC with the Files.CreateDirectory request
    /// RenameFileOrPathOnPlc/Async - Change the name of a file or directory with a Files.Rename request. In addition, the method can also move a file from one directory to another directory.
    /// DeleteFile/Async - Delete a file from the PLC with the Files.Delete request.
    /// DeleteDirectory/Async - Delete a directory from the PLC with the Files.DeleteDirectory request.
    /// DataLogs_DownloadAndClearAsync - Download and clear a datalog on the PLC. Creates a file ticket which the client can use to retrieve a data log from the PLC and clear it upon completion.
    /// WebApp_DownloadResource/Async - Send a WebApp.DownloadResource, Downloadticket and Api.Closeticket request to the API. Creates a file ticket which the client can use to retrieve a resource.
    /// </summary>
    public class ApiFileHandler : IApiFileHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IApiTicketHandler ApiTicketHandler;

        /// <summary>
        /// Handler for the plcs filehandling functionalities
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        /// <param name="apiTicketHandler">Handler for the Ticketing Endpoint of the PLC</param>
        public ApiFileHandler(IApiRequestHandler apiRequestHandler, IApiTicketHandler apiTicketHandler)
        {
            ApiRequestHandler = apiRequestHandler;
            ApiTicketHandler = apiTicketHandler;
        }

        /// <summary>
        /// Send a Files.Download, Downloadticket and Api.Closeticket request to the API. Creates a file ticket which the client can use to retrieve a file from the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>FileInfo</returns>
        public async Task<FileInfo> DownloadFileAsync(string resource, string pathToDownloadDirectory = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            var ticketId = (await ApiRequestHandler.FilesDownloadAsync(resource, cancellationToken)).Result;
            return (await ApiTicketHandler.HandleDownloadAsync(ticketId, pathToDownloadDirectory, overwriteExistingFile, cancellationToken)).File_Downloaded;
        }

        /// <summary>
        /// Send a Downloadresource, Downloadticket and Closeticket request to the API. Creates a file ticket which the client can use to retrieve a file from the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        public FileInfo DownloadFile(string resource, string pathToDownloadDirectory = null, bool overrideExistingFile = false)
            => DownloadFileAsync(resource, pathToDownloadDirectory, overrideExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Download and clear a datalog on the PLC. Creates a file ticket which the client can use to retrieve a data log from the PLC and clear it upon completion. 
        /// Clients are encouraged to use the Files.Download method if the user does not wish to clear the contents of the data log file after the download.
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>FileInfo</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public async Task<FileInfo> DataLogs_DownloadAndClearAsync(string resource, string pathToDownloadDirectory = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            var ticketId = (await ApiRequestHandler.DatalogsDownloadAndClearAsync(resource, cancellationToken)).Result;
            return (await ApiTicketHandler.HandleDownloadAsync(ticketId, pathToDownloadDirectory, overwriteExistingFile, cancellationToken)).File_Downloaded;
        }

        /// <summary>
        /// Download and clear a datalog on the PLC. Creates a file ticket which the client can use to retrieve a data log from the PLC and clear it upon completion. 
        /// Clients are encouraged to use the Files.Download method if the user does not wish to clear the contents of the data log file after the download.
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        public FileInfo DataLogs_DownloadAndClear(string resource, string pathToDownloadDirectory = null, bool overwriteExistingFile = false)
            => DataLogs_DownloadAndClearAsync(resource, pathToDownloadDirectory, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Upload a resource to the File API
        /// </summary>
        /// <param name="resource">resource name on the PLC File API endpoint</param>
        /// <param name="filePath">path to the local file to upload</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task to upload the file</returns>
        public async Task DeployFileAsync(string resource, string filePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            var ticket = await ApiRequestHandler.FilesCreateAsync(resource, cancellationToken);
            await ApiTicketHandler.HandleUploadAsync(ticket.Result, filePath, cancellationToken);
        }
        /// <summary>
        /// Upload a resource to the File API
        /// </summary>
        /// <param name="resource">resource name on the PLC File API endpoint</param>
        /// <param name="filePath">path to the local file to upload</param>
        public void DeployFile(string resource, string filePath)
         => DeployFileAsync(resource, filePath).GetAwaiter().GetResult();

        /// <summary>
        /// Upload a resource to the File API
        /// </summary>
        /// <param name="resource">resource to upload - filepath built via the ResourcePathResolver, name on PLC File Api built via GetVarNameForMethods()</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task DeployFileAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var varNameForMethods = resource.GetVarNameForMethods();
            var accordingFile = Path.Combine(resource.PathToLocalDirectory, resource.Name);
            await DeployFileAsync(varNameForMethods, accordingFile, cancellationToken);
        }


        /// <summary>
        /// Upload a resource to the File API
        /// </summary>
        /// <param name="resource">resource to upload - filepath built via the ResourcePathResolver, name on PLC File Api built via GetVarNameForMethods()</param>
        public void DeployFile(ApiFileResource resource)
            => DeployFileAsync(resource).GetAwaiter().GetResult();
    }
}
