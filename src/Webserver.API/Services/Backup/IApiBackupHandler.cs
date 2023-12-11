// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Backup
{
    /// <summary>
    /// Handler for the plcs backup and restore functionality
    /// </summary>
    public interface IApiBackupHandler
    {
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="backupName">will default to the backup name suggested by the plc</param> 
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        FileInfo DownloadBackup(string pathToDownloadDirectory = null, string backupName = null, bool overwriteExistingFile = false);
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="pathToDownloadDirectory">will default to Downloads but will determine path from -DESKTOP-, replaced "Desktop" by "Downloads"</param>
        /// <param name="backupName">will default to the backup name suggested by the plc</param> 
        /// <param name="overwriteExistingFile">choose wether you want to replace an existing file or add another file with that name to you download directory in case one already exists</param>
        /// <returns>FileInfo</returns>
        Task<FileInfo> DownloadBackupAsync(string pathToDownloadDirectory = null, string backupName = null, bool overwriteExistingFile = false, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <returns>Task/void</returns>
        void RestoreBackup(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null);
        /// <summary>
        /// Will send a Downloadresource, Downloadticket and Closeticket request to the API
        /// </summary>
        /// <param name="userName">Username for re-login</param>
        /// <param name="password">Password for re-login</param>
        /// <param name="restoreFilePath">path to the file to be restored</param>
        /// <param name="timeOut">timeout for the waithandler => plc to be up again after reboot, etc.</param>
        /// <returns>Task/void</returns>
        Task RestoreBackupAsync(string restoreFilePath, string userName, string password, TimeSpan? timeOut = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}