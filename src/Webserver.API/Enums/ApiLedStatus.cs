// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents the status of an LED
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiLedStatus
    {
        /// <summary>
        /// The LED is switched on
        /// </summary>
        On,
        /// <summary>
        /// The LED is switched off
        /// </summary>
        Off,
        /// <summary>
        /// The LED is flashing
        /// </summary>
        Flashing
    }
}
