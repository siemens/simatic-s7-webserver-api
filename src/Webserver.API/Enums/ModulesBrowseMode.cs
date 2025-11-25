// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Mode for Modules Browse Request
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ModulesBrowseMode
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Browse the given Node for its properties -> has_children, etc.
        /// </summary>
        Node = 1,
        /// <summary>
        /// Browse the given Node for its children
        /// </summary>
        Children = 2,
    }
}
