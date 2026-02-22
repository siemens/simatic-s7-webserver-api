// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible filters for ApiModulesReadParameters request
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesFilterAttribute
    {
        /// <summary>
        /// Filter for the comment.
        /// </summary>        
        Comment = 1,
        /// <summary>
        /// Filter for the parameters.
        /// </summary>
        Parameters = 2,
        /// <summary>
        /// Filter for the geo address.
        /// </summary>
        GeoAddress = 3,
    }
}
