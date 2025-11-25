// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents the color of an LED
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiLedColor
    {
        /// <summary>
        /// LED color is unknown. This should never happen
        /// </summary>
        None,
        /// <summary>
        /// LED is green
        /// </summary>
        Green,
        /// <summary>
        /// LED is yellow
        /// </summary>
        Yellow,
        /// <summary>
        /// LED is red
        /// </summary>
        Red
    }
}
