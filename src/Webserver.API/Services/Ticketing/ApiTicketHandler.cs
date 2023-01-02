// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Ticketing
{
    /// <summary>
    /// Handler class for the Ticketing download mechanism
    /// </summary>
    public class ApiTicketHandler : IApiTicketHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;

        /// <summary>
        /// Control wether or not to call BrowseTickets for the provided ticket and check for the state to be completed after 
        /// downloading data
        /// </summary>
        public bool CheckAfterDownload { get; set; } = true;

        /// <summary>
        /// Control wether or not to call BrowseTickets for the provided ticket and check for the state to be completed after 
        /// uploading data
        /// </summary>
        public bool CheckAfterUpload { get; set; } = true;

        /// <summary>
        /// Handler class for the Ticketing download mechanism
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        public ApiTicketHandler(IApiRequestHandler apiRequestHandler)
        {
            ApiRequestHandler = apiRequestHandler;
        }

        private async Task<ApiTicket> CheckTicketAsync(string ticketId, bool performCheck)
        {
            if (performCheck)
            {
                var brTicketsResp = await ApiRequestHandler.ApiBrowseTicketsAsync(ticketId);
                var ticket = brTicketsResp.Result.Tickets.First();
                if (ticket.State != Enums.ApiTicketState.Completed)
                {
                    throw new ApiTicketNotInCompletedStateException(ticket);
                }
                return ticket;
            }
            else
            {
                return new ApiTicket() { Id = ticketId, };
            }
        }

        private async Task<ApiTicket> HandleWriteFileAndCheckAsync(string ticketId, byte[] content, string filePath, bool overwriteExistingFile)
        {
            try
            {
                if (content.Count() > 0)
                {
                    if (File.Exists(filePath))
                    {
                        if (overwriteExistingFile)
                        {
                            File.Delete(filePath);
                        }
                        else
                        {
                            throw new IOException($"File at {filePath} already exists and {nameof(overwriteExistingFile)} was {overwriteExistingFile}!");
                        }
                    }
                    using (FileStream fs = File.Create(filePath))
                    {
                        fs.Write(content, 0, content.Length);
                    }
                }
                else
                {
                    throw new Exception("The downloaded file has no content.");
                }
                var ticket = await CheckTicketAsync(ticketId, CheckAfterDownload);
                ticket.File_Downloaded = new FileInfo(filePath);
                return ticket;
            }
            finally
            {
                await ApiRequestHandler.ApiCloseTicketAsync(ticketId);
            }
        }

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, string filePath, bool overwriteExistingFile = false)
        {
            var dirPath = filePath.Substring(0, filePath.LastIndexOf(@"\") + 1);
            if (!Directory.Exists(dirPath))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{dirPath}{Environment.NewLine} has not been found!");
            }
            var content = await ApiRequestHandler.DownloadTicketAsync(ticketId);
            return await HandleWriteFileAndCheckAsync(ticketId, content, filePath, overwriteExistingFile);
        }

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, string filePath, bool overwriteExistingFile)
            => HandleDownloadAsync(ticketId, filePath, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, string pathToDownloadDirectory)
            => await HandleDownloadAsync(ticketId, pathToDownloadDirectory, null, null, false);
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, string pathToDownloadDirectory)
         => HandleDownloadAsync(ticketId, pathToDownloadDirectory).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string pathToDownloadDirectory)
            => await HandleDownloadAsync(ticket.Id, pathToDownloadDirectory);
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, string pathToDownloadDirectory)
            => HandleDownloadAsync(ticket, pathToDownloadDirectory).GetAwaiter().GetResult();
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, DirectoryInfo pathToDownloadDirectory)
            => await HandleDownloadAsync(ticketId, pathToDownloadDirectory.FullName);
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, DirectoryInfo pathToDownloadDirectory)
            => HandleDownloadAsync(ticketId, pathToDownloadDirectory).GetAwaiter().GetResult();
        
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory)
            => await HandleDownloadAsync(ticket.Id, pathToDownloadDirectory);
        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory)
         => HandleDownloadAsync(ticket, pathToDownloadDirectory).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, FileInfo filePath, bool overwriteExistingFile)
            => await HandleDownloadAsync(ticketId, filePath.FullName, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, FileInfo filePath, bool overwriteExistingFile)
            => HandleDownloadAsync(ticketId, filePath, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string filePath, bool overwriteExistingFile)
         => await HandleDownloadAsync(ticket.Id, filePath, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, string filePath, bool overwriteExistingFile)
         => HandleDownloadAsync(ticket, filePath, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, FileInfo filePath, bool overwriteExistingFile)
         => await HandleDownloadAsync(ticket.Id, filePath, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, FileInfo filePath, bool overwriteExistingFile)
         => HandleDownloadAsync(ticket, filePath, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
        {
            if (pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            var response = await ApiRequestHandler.DownloadTicketAndGetResponseAsync(ticketId);
            //Downloads: 374DE290-123F-4565-9164-39C4925E467B
            string usedPathToDownloadDirectory = pathToDownloadDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads");
            string usedFilename = fileName ?? response.Content.Headers.ContentDisposition.FileName;
            string usedFileExtension = fileExtension ?? "";
            var content = await response.Content.ReadAsByteArrayAsync();
            string path = Path.Combine(usedPathToDownloadDirectory, usedFilename + usedFileExtension);
            uint counter = 0;
            var firstPath = path;
            while (File.Exists(path) && !overwriteExistingFile)
            {
                FileInfo fileInfo = new FileInfo(path);
                DirectoryInfo dir = fileInfo.Directory;
                path = Path.Combine(dir.FullName, (Path.GetFileNameWithoutExtension(firstPath) + "(" + counter + ")" + fileInfo.Extension));
                counter++;
            }
            if (usedFilename.Contains("/"))
            {
                var split = usedFilename.Split('/');
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
            return await HandleWriteFileAndCheckAsync(ticketId, content, path, overwriteExistingFile);
        }

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => HandleDownloadAsync(ticketId, pathToDownloadDirectory, fileName, fileExtension, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
            => await HandleDownloadAsync(ticket.Id, pathToDownloadDirectory, fileName, fileExtension, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => HandleDownloadAsync(ticket, pathToDownloadDirectory, fileName, fileExtension, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(string ticketId, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => await HandleDownloadAsync(ticketId, pathToDownloadDirectory.FullName, fileName, fileExtension, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(string ticketId, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => HandleDownloadAsync(ticketId, pathToDownloadDirectory, fileName, fileExtension, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public async Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => await HandleDownloadAsync(ticket.Id, pathToDownloadDirectory.FullName, fileName, fileExtension, overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">wether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        public ApiTicket HandleDownload(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false)
         => HandleDownloadAsync(ticket, pathToDownloadDirectory, fileName, fileExtension, overwriteExistingFile).GetAwaiter().GetResult();

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public async Task<ApiTicket> HandleUploadAsync(string ticketId, string filePath)
        {
            try
            {
                await ApiRequestHandler.UploadTicketAsync(ticketId, filePath);
                return await CheckTicketAsync(ticketId, CheckAfterUpload);
            }
            finally
            {
                await ApiRequestHandler.ApiCloseTicketAsync(ticketId);
            }
        }

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public ApiTicket HandleUpload(string ticketId, string filePath)
            => HandleUploadAsync(ticketId, filePath).GetAwaiter().GetResult();

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public async Task<ApiTicket> HandleUploadAsync(ApiTicket ticket, string filePath)
            => await HandleUploadAsync(ticket.Id, filePath);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public ApiTicket HandleUpload(ApiTicket ticket, string filePath)
            => HandleUploadAsync(ticket, filePath).GetAwaiter().GetResult();

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public async Task<ApiTicket> HandleUploadAsync(string ticketId, FileInfo filePath)
            => await HandleUploadAsync(ticketId, filePath.FullName);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public ApiTicket HandleUpload(string ticketId, FileInfo filePath)
            => HandleUploadAsync(ticketId, filePath).GetAwaiter().GetResult();

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public async Task<ApiTicket> HandleUploadAsync(ApiTicket ticket, FileInfo filePath)
            => await HandleUploadAsync(ticket.Id, filePath);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        public ApiTicket HandleUpload(ApiTicket ticket, FileInfo filePath)
            => HandleUploadAsync(ticket, filePath).GetAwaiter().GetResult();

        
    }
}
