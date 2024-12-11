// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// User Authentication modes of the PLC.<br/>
    /// Depends on the configured user management which authentication modes will be supported by the PLC based on the authentication mode.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiAuthenticationMode
    {
        /// <summary>
        /// The users are stored on the PLC
        /// </summary>
        Local,
        /// <summary>
        /// Legacy User Management
        /// </summary>
        Static,
        /// <summary>
        /// Only Anonymous user is available
        /// </summary>
        Disabled,
        /// <summary>
        /// Central User Management (via remote authentication with Username and Password)
        /// </summary>
        Umc,
    }
}
