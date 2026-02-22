// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents the state of the mode selector switch of the PLC. 
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiPlcModeSelectorState
    {
        /// <summary>
        /// No physical mode selector switch is present. 
        /// </summary>
        NoSwitch,
        /// <summary>
        /// The mode selector switch is in position STOP.
        /// </summary>
        Stop,
        /// <summary>
        /// The mode selector switch is in position RUN.
        /// </summary>
        Run,
        /// <summary>
        /// The mode selector switch position could not be determined.
        /// For example, the partner PLC of an R/H system is not reachable.
        /// </summary>
        Unknown
    }
}
