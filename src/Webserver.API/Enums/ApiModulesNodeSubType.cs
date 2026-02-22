// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible subtypes of a node
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesNodeSubType
    {
        /// <summary>
        /// This node has no dedicated subtype
        /// </summary>
        None = 0,

        /// <summary>
        /// This node is of subtype profinet_interface
        /// </summary>
        ProfinetInterface = 1,

        /// <summary>
        /// This node is of subtype profinet_interface_virtual
        /// </summary>
        ProfinetInterfaceVirtual = 2,

        /// <summary>
        /// This node is of subtype profinet_port
        /// </summary>
        ProfinetPort = 3,

        /// <summary>
        /// This node is of subtype profibus_interface
        /// </summary>
        ProfibusInterface = 4,

        /// <summary>
        /// This node is of subtype cpu
        /// </summary>
        Cpu = 5,

        /// <summary>
        /// This node is of subtype central_iosystem
        /// </summary>
        [EnumMember(Value = "central_iosystem")]
        CentralIoSystem = 6,

        /// <summary>
        /// This node is of subtype central_device
        /// </summary>
        CentralDevice = 7,

        /// <summary>
        /// This node is of subtype profinet_iosystem
        /// </summary>
        [EnumMember(Value = "profinet_iosystem")]
        ProfinetIoSystem = 8,

        /// <summary>
        /// This node is of subtype profibus_iosystem
        /// </summary>
        [EnumMember(Value = "profibus_iosystem")]
        ProfibusIoSystem = 9,

        /// <summary>
        /// This node is of subtype gateway
        /// </summary>
        Gateway = 10,
    }
}
