// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible attributes of a node
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesNodeAttribute
    {
        /// <summary>
        /// The node represents a failsafe node
        /// </summary>
        Failsafe = 1,

        /// <summary>
        /// The node supports the firmware update
        /// </summary>
        FirmwareUpdate = 2,

        /// <summary>
        /// The node supports the download of its service data
        /// </summary>
        ServiceData = 3,

        /// <summary>
        /// The node has configuration control (option handling) enabled
        /// </summary>
        ConfigurationControl = 4,

        /// <summary>
        /// The node has a configred topology
        /// </summary>
        ConfiguredTopology = 5,
    }
}
