// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// ​The type to read out.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ModulesReadIdentificationMaintenanceType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// The current data is read from the module.
        /// </summary>
        Actual = 1,
        /// <summary>
        /// The expected data is read from the hardware configuration.
        /// </summary>
        Configured = 2
    }
}
