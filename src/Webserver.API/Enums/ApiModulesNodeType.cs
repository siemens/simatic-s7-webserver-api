// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible types of a node
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesNodeType
    {
        /// <summary>
        /// This node has no dedicated type
        /// </summary>
        None = 0,

        /// <summary>
        /// This node represents an I/O system
        /// </summary>
        [EnumMember(Value = "iosystem")]
        IoSystem = 1,

        /// <summary>
        /// This node represents a device
        /// </summary>
        Device = 2,

        /// <summary>
        /// This node represents a module
        /// </summary>
        Module = 3,

        /// <summary>
        /// This node represents a submodule
        /// </summary>
        [EnumMember(Value = "submodule")]
        Submodule = 4,
    }
}
