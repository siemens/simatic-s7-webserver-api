// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Ticketing
{
    /// <summary>
    /// Handler class for the Ticketing download mechanism
    /// </summary>
    public interface IApiTicketHandler
    {
        /// <summary>
        /// Control whether or not to call BrowseTickets for the provided ticket and check for the state to be completed after 
        /// downloading data
        /// </summary>
        bool CheckAfterDownload { get; set; }

        /// <summary>
        /// Control whether or not to call BrowseTickets for the provided ticket and check for the state to be completed after 
        /// uploading data
        /// </summary>
        bool CheckAfterUpload { get; set; }

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, string pathToDownloadDirectory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string pathToDownloadDirectory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, DirectoryInfo pathToDownloadDirectory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, string filePath, bool overwriteExistingFile = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, string filePath, bool overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, FileInfo filePath, bool overwriteExistingFile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, FileInfo filePath, bool overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string filePath, bool overwriteExistingFile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, string filePath, bool overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, FileInfo filePath, bool overwriteExistingFile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="filePath">Path for the file to be downloaded</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, FileInfo filePath, bool overwriteExistingFile);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, string pathToDownloadDirectory);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, string pathToDownloadDirectory);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, DirectoryInfo pathToDownloadDirectory);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">The ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory);


        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, string pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(string ticketId, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(string ticketId, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        Task<ApiTicket> HandleDownloadAsync(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handler method to get the fileinfo and download the content into a file.
        /// </summary>
        /// <param name="ticket">Ticket to perform the download for</param>
        /// <param name="pathToDownloadDirectory">Path to the download directory</param>
        /// <param name="overwriteExistingFile">whether or not to overwrite an existing file</param>
        /// <param name="fileName">file name for the file to be downloaded</param>
        /// <param name="fileExtension">file extension for the file to be downloaded</param>
        /// <returns>The Ticket for that the Download has been performed</returns>
        /// <exception cref="DirectoryNotFoundException">Given Download Directory does not exist</exception>
        /// <exception cref="Exception">File has no content</exception>
        ApiTicket HandleDownload(ApiTicket ticket, DirectoryInfo pathToDownloadDirectory = null, string fileName = null, string fileExtension = null, bool overwriteExistingFile = false);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        Task<ApiTicket> HandleUploadAsync(string ticketId, string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        ApiTicket HandleUpload(string ticketId, string filePath);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        Task<ApiTicket> HandleUploadAsync(ApiTicket ticket, string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        ApiTicket HandleUpload(ApiTicket ticket, string filePath);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        Task<ApiTicket> HandleUploadAsync(string ticketId, FileInfo filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticketId">ID of the ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        ApiTicket HandleUpload(string ticketId, FileInfo filePath);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <param name="cancellationToken">Cancellation token to cancel pending requests.</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        Task<ApiTicket> HandleUploadAsync(ApiTicket ticket, FileInfo filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handle the Ticket Upload
        /// </summary>
        /// <param name="ticket">The ticket to perform the upload for</param>
        /// <param name="filePath">Path for the file to be uploaded</param>
        /// <returns>The Ticket for that the Upload has been performed</returns>
        ApiTicket HandleUpload(ApiTicket ticket, FileInfo filePath);
    }
}