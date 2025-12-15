// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Services.HelperHandlers;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Backup
{
    /// <summary>
    /// Handler for the plcs backup and restore functionality
    /// </summary>
    public class ApiBackupHandler : IApiBackupHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IApiTicketHandler ApiTicketHandler;
        private readonly ILogger Logger;

        /// <summary>
        /// Handler for the plcs backup and restore functionality
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        /// <param name="apiTicketHandler">Handler for the Ticketing Endpoint of the PLC</param>
        /// <param name="logger">Logger for the ApiBackupHandler</param>
        public ApiBackupHandler(IApiRequestHandler apiRequestHandler, IApiTicketHandler apiTicketHandler, ILogger logger = null)
        {
            ApiRequestHandler = apiRequestHandler;
            ApiTicketHandler = apiTicketHandler;
            Logger = logger;
        }

        /// <summary>
        /// Will send a DownloadbackupName, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="backupName">will default to the backup name suggested by the plc</param> 
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>FileInfo</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public async Task<FileInfo> DownloadBackupAsync(string pathToDownloadDirectory = null, string backupName = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default)
        {
            if (pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            var ticket = (await ApiRequestHandler.PlcCreateBackupAsync(cancellationToken)).Result;
            return (await ApiTicketHandler.HandleDownloadAsync(ticket, pathToDownloadDirectory, backupName, null, overwriteExistingFile, cancellationToken)).File_Downloaded;
        }

        /// <summary>
        /// Will send a DownloadbackupName, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="backupName">will default to the backup name suggested by the plc</param>
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public FileInfo DownloadBackup(string pathToDownloadDirectory = null, string backupName = null, bool overwriteExistingFile = false)
            => DownloadBackupAsync(pathToDownloadDirectory, backupName).GetAwaiter().GetResult();

        /// <summary>
        /// Restores the PLC from a backup file
        /// </summary>
        /// <remarks>
        /// Works only if the PLC is in STOP! <br/>
        /// Checks if the restore file can be found at the restoreFilePath. <br/>
        /// Sends a PlcRestoreBackup then starts to upload the backup file with the response ticket. <br/>
        /// If the PLC validated the restorebackup file header, the upload is cancelled and the PLC will reboot in 3 seconds. <br/>
        /// After the reboot completed, sends a Login, then a PlcRestoreBackup request. <br/>
        /// Uploads again the backup file with the response ticket. <br/> 
        /// When the upload finished, the PLC will reboot again. After that sends a final login request.
        /// </remarks>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc. - defaults to 3 minutes</param>
        /// <param name="externalCancellationToken">Optional token to cancel the async method</param>
        /// <returns>Task</returns>
        /// <exception cref="FileNotFoundException">File at restorefilepath has not been found</exception>
        /// <exception cref="InvalidOperationException">Restore is not possible</exception>
        public async Task RestoreBackupAsync(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null, CancellationToken externalCancellationToken = default)
        {
            var timeToWait = timeOut ?? TimeSpan.FromMinutes(3);
            if (restoreFilePath == null)
            {
                throw new ArgumentNullException(nameof(restoreFilePath));
            }
            if (!File.Exists(restoreFilePath))
            {
                throw new FileNotFoundException($"the given file at {Environment.NewLine}{restoreFilePath}{Environment.NewLine} has not been found!");
            }

            var browseResult = (await ApiRequestHandler.ApiBrowseAsync(externalCancellationToken)).Result;
            bool restoreMode = !browseResult.Any(x => x.Name == "Plc.CreateBackup");
            string uploadTicket;
            var waitHandler = new WaitHandler(timeToWait, logger: Logger);

            if (!restoreMode)
            {
                uploadTicket = (await ApiRequestHandler.PlcRestoreBackupAsync(password, externalCancellationToken)).Result;
                using (var internalCancellationTokenSource = new CancellationTokenSource())
                {
                    try
                    {
                        Task<Models.ApiTicket> uploadTask = ApiTicketHandler.HandleUploadAsync(uploadTicket, restoreFilePath, internalCancellationTokenSource.Token);
                        Stopwatch sw = Stopwatch.StartNew();
                        while (sw.ElapsedMilliseconds < 60_000 || uploadTask.IsCompleted || uploadTask.IsFaulted)
                        {
                            var brTicketsResp = ApiRequestHandler.ApiBrowseTickets(uploadTicket);
                            string apiTicketData = brTicketsResp.Result.Tickets.First().Data.ToString();
                            if (apiTicketData.Contains("\"restore_state\": \"rebooting_format\"") || externalCancellationToken.IsCancellationRequested)
                            {
                                internalCancellationTokenSource.Cancel();
                                break;
                            }
                        }
                        sw.Stop();
                        await uploadTask;
                    }
                    catch (ApiTicketingEndpointUploadException e) when (e.InnerException is TaskCanceledException) {
                        Logger?.LogDebug($"Upload cancelled as expecteded due to plc rebooting for format - {e.Message}.");
                    }
                    finally
                    {
                        externalCancellationToken.ThrowIfCancellationRequested();
                    }
                }
                WaitForPlcReboot(waitHandler, externalCancellationToken);
                await ApiRequestHandler.ReLoginAsync(userName, password, cancellationToken: externalCancellationToken);
            }
            uploadTicket = (await ApiRequestHandler.PlcRestoreBackupAsync(password, externalCancellationToken)).Result;
            await ApiTicketHandler.HandleUploadAsync(uploadTicket, restoreFilePath, externalCancellationToken);
            WaitForPlcReboot(waitHandler, externalCancellationToken);
            await ApiRequestHandler.ReLoginAsync(userName, password, cancellationToken: externalCancellationToken);
        }

        /// <summary>
        /// Restores the PLC from a backup file
        /// </summary>
        /// <remarks>
        /// Works only if the PLC is in STOP! <br/>
        /// Checks if the restore file can be found at the restoreFilePath. <br/>
        /// Sends a PlcRestoreBackup then starts to upload the backup file with the response ticket. <br/>
        /// If the PLC validated the restorebackup file header, the upload is cancelled and the PLC will reboot in 3 seconds. <br/>
        /// After the reboot completed, sends a Login, then a PlcRestoreBackup request. <br/>
        /// Uploads again the backup file with the response ticket. <br/> 
        /// When the upload finished, the PLC will reboot again. After that sends a final login request.
        /// </remarks>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc. - defaults to 3 minutes</param>
        /// <returns>Task</returns>
        /// <exception cref="FileNotFoundException">File at restorefilepath has not been found</exception>
        public void RestoreBackup(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null)
            => RestoreBackupAsync(restoreFilePath, userName, password, timeOut).GetAwaiter().GetResult();

        /// <summary>
        /// Wait for the PLC to finish the reboot
        /// </summary>
        /// <param name="waitHandler">waithandler to use for conditional waits</param>
        /// <param name="cancellationToken">cancellation token (breaks waithandler)</param>
        private void WaitForPlcReboot(WaitHandler waitHandler, CancellationToken cancellationToken)
        {
            waitHandler.ForTrue(() =>
            {
                try
                {
                    ApiRequestHandler.ApiPing();
                    return false;
                }
                catch (Exception e)
                {
                    Logger?.LogDebug(e, $"Plc should be rebooting now.");
                    return true;
                }
            });
            Logger?.LogDebug("Wait for plc to be pingable again (reboot).");
            waitHandler.ForTrue(() =>
            {
                var pingRes = ApiRequestHandler.ApiPing();
                Logger?.LogDebug(string.Format("Plc pingable again via api pingresult: {0}", pingRes.Result));
                return true;
            });
        }
    }
}