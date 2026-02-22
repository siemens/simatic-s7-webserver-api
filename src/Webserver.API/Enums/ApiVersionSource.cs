// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible Version Sources
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiVersionSource
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// TIA Portal
        /// </summary>
        TiaPortal = 1,
        /// <summary>
        /// STEP 7 Safety
        /// </summary>
        Step7Safety = 2
    }
}
