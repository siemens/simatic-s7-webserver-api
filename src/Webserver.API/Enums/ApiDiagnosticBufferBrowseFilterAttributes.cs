// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible filters for ApiDiagnosticBufferBrowse request
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiDiagnosticBufferBrowseFilterAttributes
    {
        /// <summary>
        /// Filter for the short diagnostic buffer text.
        /// </summary>
        [EnumMember(Value = "short_text")]
        ShortText = 1,
        /// <summary>
        /// Filter for the long diagnostic buffer text.
        /// </summary>
        [EnumMember(Value = "long_text")]
        LongText = 2,
        /// <summary>
        /// Filter for the help text message in case of an incoming event.
        /// </summary>
        [EnumMember(Value = "help_text")]
        HelpText = 3,
    }
}
