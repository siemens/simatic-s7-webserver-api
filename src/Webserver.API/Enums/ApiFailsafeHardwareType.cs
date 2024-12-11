// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The type defines if the requested hardware identifier represents either the safety PLC itself or another failsafe module.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiFailsafeHardwareType
    {
        /// <summary>
        /// The hw id is a CPU
        /// </summary>
        F_cpu,
        /// <summary>
        /// The hw id is a module
        /// </summary>
        F_module
    }
}
