// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The role of the represented PLC in the redundant system.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiPlcRedundancyRole
    {
        /// <summary>
        /// For the tree of the partner PLC, the role may be unknown in case that the partner PLC is not properly paired.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        /// <summary>
        /// Primary PLC
        /// </summary>
        [EnumMember(Value = "primary")]
        Primary = 1,
        /// <summary>
        /// Backup PLC
        /// </summary>
        [EnumMember(Value = "backup")]
        Backup = 2
    }
}
