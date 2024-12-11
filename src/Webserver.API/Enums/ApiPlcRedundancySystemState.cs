// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The system state describes the overall state of the R/H system. 
    /// It is determined by the combination of the operating modes of both R/H PLCs.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiPlcRedundancySystemState
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        /// <summary>
        /// Run_solo
        /// </summary>
        [EnumMember(Value = "run_solo")]
        Run_solo = 1,
        /// <summary>
        /// Run_redundant
        /// </summary>
        [EnumMember(Value = "run_redundant")]
        Run_redundant = 2,
        /// <summary>
        /// Stop
        /// </summary>
        [EnumMember(Value = "stop")]
        Stop = 3,
        /// <summary>
        /// Syncup
        /// </summary>
        [EnumMember(Value = "syncup")]
        Syncup = 4,
        /// <summary>
        /// Halt
        /// </summary>
        [EnumMember(Value = "halt")]
        Halt = 5,
        /// <summary>
        /// Startup
        /// </summary>
        [EnumMember(Value = "startup")]
        Startup = 6,

    }
}
