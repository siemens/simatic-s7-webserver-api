// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The mode if the attributes shall either be included or excluded.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiBrowseFilterMode
    {
        /// <summary>
        /// The response will include the listed attributes and exclude the others.
        /// </summary>
        [EnumMember(Value = "include")]
        Include = 0,
        /// <summary>
        /// The response will exclude the listed attributes and include the others.
        /// </summary>
        [EnumMember(Value = "exclude")]
        Exclude = 1,
    }
}