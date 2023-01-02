// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Handler for the plcs files.*, datalogs.download and webapp.downloadresource functionality with comfort functions to perform full download etc.
    /// </summary>
    public interface IApiFileHandler
    {
        /// <summary>
        /// Send a Downloadresource, Downloadticket and Closeticket request to the API. Creates a file ticket which the client can use to retrieve a file from the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        Task<FileInfo> DownloadFileAsync(string resource, string pathToDownloadDirectory = null, bool overrideExistingFile = false);

        /// <summary>
        /// Send a Downloadresource, Downloadticket and Closeticket request to the API. Creates a file ticket which the client can use to retrieve a file from the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        FileInfo DownloadFile(string resource, string pathToDownloadDirectory = null, bool overrideExistingFile = false);

        /// <summary>
        /// Creates a file on the PLC : creates a file ticket which the client can use to transfer a file to the PLC. This is referred to as "uploading" a file to the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="filePath">Path of the file to upload</param>
        /// <returns>Task</returns>
        Task DeployFileAsync(string resource, string filePath);

        /// <summary>
        /// Creates a file on the PLC : creates a file ticket which the client can use to transfer a file to the PLC. This is referred to as "uploading" a file to the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <param name="filePath">Path of the file to upload</param>
       /// <returns>void</returns>
        void DeployFile(string resource, string filePath);

        /// <summary>
        /// Creates a file on the PLC : creates a file ticket which the client can use to transfer a file to the PLC. This is referred to as "uploading" a file to the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Task</returns>
        Task DeployFileAsync(ApiFileResource resource);

        /// <summary>
        /// Creates a file on the PLC : creates a file ticket which the client can use to transfer a file to the PLC. This is referred to as "uploading" a file to the PLC.
        /// </summary>
        /// <param name="resource">Path of the file relative to the memory card root.</param>
        /// <returns>Task</returns>
        void DeployFile(ApiFileResource resource);

        /// <summary>
        /// Download and clear a datalog on the PLC. Creates a file ticket which the client can use to retrieve a data log from the PLC and clear it upon completion. 
        /// Clients are encouraged to use the Files.Download method if the user does not wish to clear the contents of the data log file after the download.
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overrideExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        Task<FileInfo> DataLogs_DownloadAndClearAsync(string resource, string pathToDownloadDirectory = null, bool overrideExistingFile = false);

        /// <summary>
        /// Download and clear a datalog on the PLC. Creates a file ticket which the client can use to retrieve a data log from the PLC and clear it upon completion. 
        /// Clients are encouraged to use the Files.Download method if the user does not wish to clear the contents of the data log file after the download.
        /// </summary>
        /// <param name="resource">Resource name of data log to retrieve, including the path.</param>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        FileInfo DataLogs_DownloadAndClear(string resource, string pathToDownloadDirectory = null, bool overwriteExistingFile = false);

    }
}
