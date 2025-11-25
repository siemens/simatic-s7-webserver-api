// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Enum of all possible PLC types regarding motion capabilities
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiTechnologyPlcType
    {
        /// <summary>
        /// The PLC is a S7-1500
        /// </summary>
        [EnumMember(Value = "1500")]
        S71500,
        /// <summary>
        /// The PLC is a S7-1500T
        /// </summary>
        [EnumMember(Value = "1500T")]
        S71500T,
        /// <summary>
        /// The PLC is a S7-1200 G2
        /// </summary>
        [EnumMember(Value = "1200")]
        S71200,
        /// <summary>
        /// The PLC type is unknown
        /// </summary>
        Unknown
    }
}