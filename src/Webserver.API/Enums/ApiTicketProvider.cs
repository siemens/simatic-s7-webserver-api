﻿// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

    }



}
