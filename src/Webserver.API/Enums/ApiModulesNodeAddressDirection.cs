// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible addressing modes of an input or output module
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiModulesNodeAddressDirection
    {
        /// <summary>
        /// The address represents an input address
        /// </summary>
        Input = 1,

        /// <summary>
        /// The address represents an output address
        /// </summary>
        Output = 2,
    }
}
