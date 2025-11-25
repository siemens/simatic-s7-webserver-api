// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible own/subordinate states of a node
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesNodeState
    {
        /// <summary>
        /// good state
        /// </summary>
        Good = 1,

        /// <summary>
        /// deactivated state
        /// </summary>
        Deactivated = 2,

        /// <summary>
        /// maintenance required state
        /// </summary>
        MaintenanceRequired = 3,

        /// <summary>
        /// maintenance demanded state
        /// </summary>
        MaintenanceDemanded = 4,

        /// <summary>
        /// error state
        /// </summary>
        Error = 5,

        /// <summary>
        /// not reachable state
        /// </summary>
        NotReachable = 6,

        /// <summary>
        /// unknown state
        /// </summary>
        Unknown = 7,

        /// <summary>
        /// I/O not available state
        /// </summary>
        IoNotAvailable = 8,
    }
}
