// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Block type enumeration.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiPlcProgramBlockType
    {
        /// <summary>
        /// OB block.
        /// </summary>
        [EnumMember(Value = "ob")]
        Ob,

        /// <summary>
        /// FB block.
        /// </summary>
        [EnumMember(Value = "fc")]
        Fc,

        /// <summary>
        /// FC block.
        /// </summary>
        [EnumMember(Value = "fb")]
        Fb,

        /// <summary>
        /// SFB block.
        /// </summary>
        [EnumMember(Value = "sfc")]
        Sfc,

        /// <summary>
        /// SFC block.
        /// </summary>
        [EnumMember(Value = "sfb")]
        Sfb,
    }
}
