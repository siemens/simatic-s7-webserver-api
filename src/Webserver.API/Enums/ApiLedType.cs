// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents the type of an LED
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiLedType
    {
        /// <summary>
        /// This should never happen
        /// </summary>
        None,
        /// <summary>
        /// The LED represents the RUN/STOP LED
        /// </summary>
        RunStop,
        /// <summary>
        /// The LED represents the Maintenance LED
        /// </summary>
        Maintenance,
        /// <summary>
        /// The LED represents the Error LED
        /// </summary>
        Error
    }
}
