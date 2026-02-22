// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// ApiTicketProvider: Origin of the Ticket
    /// </summary>
    public enum ApiTicketProvider
    {
        /// <summary>
        /// should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// For Api: WebApp.CreateResource
        /// </summary>
        [JsonProperty("WebApp.CreateResource")]
        [EnumMember(Value = "WebApp.CreateResource")]
        WebApp_CreateResource = 1,
        /// <summary>
        /// For Api: WebApp.DownloadResource
        /// </summary>
        [JsonProperty("WebApp.DownloadResource")]
        [EnumMember(Value = "WebApp.DownloadResource")]
        WebApp_DownloadResource = 2,
        /// <summary>
        /// For Api: Plc.RestoreBackup
        /// </summary>
        [JsonProperty("Plc.RestoreBackup")]
        [EnumMember(Value = "Plc.RestoreBackup")]
        Plc_RestoreBackup = 3,
        /// <summary>
        /// For Api: Files.Create
        /// </summary>
        [JsonProperty("Files.Create")]
        [EnumMember(Value = "Files.Create")]
        Files_Create = 4,
        /// <summary>
        /// For Api: Plc.CreateBackup
        /// </summary>
        [JsonProperty("Plc.CreateBackup")]
        [EnumMember(Value = "Plc.CreateBackup")]
        Plc_CreateBackup = 5,
        /// <summary>
        /// For Api: PlcProgram.DownloadProfilingData
        /// </summary>
        [JsonProperty("PlcProgram.DownloadProfilingData")]
        [EnumMember(Value = "PlcProgram.DownloadProfilingData")]
        PlcProgram_DownloadProfilingData = 6,
        /// <summary>
        /// For Api: Files.Download
        /// </summary>
        [JsonProperty("Files.Download")]
        [EnumMember(Value = "Files.Download")]
        Files_Download = 7,
        /// <summary>
        /// For Api: DataLogs.DownloadAndClear
        /// </summary>
        [JsonProperty("DataLogs.DownloadAndClear")]
        [EnumMember(Value = "DataLogs.DownloadAndClear")]
        DataLogs_DownloadAndClear = 8,
    }
}
