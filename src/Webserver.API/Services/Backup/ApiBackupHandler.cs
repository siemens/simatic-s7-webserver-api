// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using Siemens.Simatic.S7.Webserver.API.Services.HelperHandlers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Siemens.Simatic.S7.Webserver.API.Services.Ticketing;

namespace Siemens.Simatic.S7.Webserver.API.Services.Backup
{
    /// <summary>
    /// Handler for the plcs backup and restore functionality
    /// </summary>
    public class ApiBackupHandler : IApiBackupHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IApiTicketHandler ApiTicketHandler;

        /// <summary>
        /// Handler for the plcs backup and restore functionality
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        /// <param name="apiTicketHandler">Handler for the Ticketing Endpoint of the PLC</param>
        public ApiBackupHandler(IApiRequestHandler apiRequestHandler, IApiTicketHandler apiTicketHandler)
        {
            ApiRequestHandler = apiRequestHandler;
            ApiTicketHandler = apiTicketHandler;
        }

        /// <summary>
        /// Will send a DownloadbackupName, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="backupName">will default to the backup name suggested by the plc</param> 
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public async Task<FileInfo> DownloadBackupAsync(string pathToDownloadDirectory = null, string backupName = null,  bool overwriteExistingFile = false)
        {
            if (pathToDownloadDirectory != null && !Directory.Exists(pathToDownloadDirectory))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{pathToDownloadDirectory}{Environment.NewLine} has not been found!");
            }
            var ticket = (await ApiRequestHandler.PlcCreateBackupAsync()).Result;
            return (await ApiTicketHandler.HandleDownloadAsync(ticket, pathToDownloadDirectory, backupName, null, overwriteExistingFile)).File_Downloaded;
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
        /// Will send a DownloadbackupName, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc. - defaults to 3 minutes</param>
        /// <returns>Task</returns>
        /// <exception cref="FileNotFoundException">File at restorefilepath has not been found</exception>
        public async Task RestoreBackupAsync(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null)
        {
            var timeToWait = timeOut ?? TimeSpan.FromMinutes(3);
            if(restoreFilePath == null)
            {
                throw new ArgumentNullException(nameof(restoreFilePath));
            }
            if(!File.Exists(restoreFilePath))
            {
                throw new FileNotFoundException($"the given file at {Environment.NewLine}{restoreFilePath}{Environment.NewLine} has not been found!");
            }
            string ticketResponse = (await ApiRequestHandler.PlcRestoreBackupAsync(password)).Result;
            try
            {
                await ApiTicketHandler.HandleUploadAsync(ticketResponse, restoreFilePath);
            }
            // HttpRequestException is okay since during the upload the plc will power cycle and not "successfully answer" the request
            catch (ApiTicketingEndpointUploadException e)
            {
                if (e.InnerException != null || !(e.InnerException is HttpRequestException))
                    throw;
            }
            var waitHandler = new WaitHandler(timeToWait);
            WaitForPlcReboot(waitHandler);
            await ApiRequestHandler.ReLoginAsync(userName, password);
            ticketResponse = (await ApiRequestHandler.PlcRestoreBackupAsync(password)).Result;
            await ApiTicketHandler.HandleUploadAsync(ticketResponse, restoreFilePath);
            WaitForPlcReboot(waitHandler);
            await ApiRequestHandler.ReLoginAsync(userName, password);
        }

        /// <summary>
        /// Will send a DownloadbackupName, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <returns>Task/void</returns>
        public void RestoreBackup(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null)
            => RestoreBackupAsync(restoreFilePath, userName, password, timeOut).GetAwaiter().GetResult();

        /// <summary>
        /// Wait for the PLC to finish the reboot
        /// </summary>
        /// <param name="waitHandler"></param>
        /// <param name="verbose"></param>
        private void WaitForPlcReboot(WaitHandler waitHandler, bool verbose = false)
        {
            waitHandler.ForTrue(() =>
            {
                try
                {
                    var pingRes = ApiRequestHandler.ApiPing();
                    return false;
                }
                catch (Exception e)
                {
                    if (verbose)
                    {
                        Console.WriteLine($"{DateTime.Now}: {e.GetType()}{e.Message} => Plc should be rebooting now.");
                    }
                    return true;
                }
            });
            if (verbose)
            {
                Console.WriteLine($"{DateTime.Now}: Wait for plc to be pingable again (reboot).");
            }
            waitHandler.ForTrue(() =>
            {
                try
                {
                    var pingRes = ApiRequestHandler.ApiPing();
                    if (verbose)
                    {
                        Console.WriteLine($"{DateTime.Now}: Plc pingable again via api pingresult: {pingRes.Result}");
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}