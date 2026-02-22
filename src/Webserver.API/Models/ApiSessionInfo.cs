// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Information of the user session, including the authentication mode, username, password expiration and timeout
    /// </summary>
    public class ApiSessionInfo
    {
        /// <summary>
        /// Authentication mode that was used to create this user session.
        /// </summary>
        [JsonProperty("authentication_mode")]
        public ApiUserAuthenticationMode AuthenticationMode { get; set; }

        /// <summary>
        /// Username of the requested session.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password expiration information. Used only when the authentication mode local or umc is used, and the password expiration functionality is enabled on the PLC.
        /// </summary>
        [JsonProperty("password_expiration")]
        public ApiPasswordExpiration PasswordExpiration { get; set; }

        /// <summary>
        /// The inactivity timespan after which a client application may perform a log out using API method Api.Logout.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        [JsonProperty("runtime_timeout")]
        public TimeSpan? RuntimeTimeout { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>Returns true if the ApiQuantityStructures are the same</returns>

        public override bool Equals(object obj)
        {
            return obj is ApiSessionInfo info &&
                   AuthenticationMode == info.AuthenticationMode &&
                   Username == info.Username &&
                   EqualityComparer<ApiPasswordExpiration>.Default.Equals(PasswordExpiration, info.PasswordExpiration) &&
                   EqualityComparer<TimeSpan?>.Default.Equals(RuntimeTimeout, info.RuntimeTimeout);
        }


        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode for the ApiQuantityStructures</returns>
        public override int GetHashCode()
        {
            return (AuthenticationMode, Username, PasswordExpiration, RuntimeTimeout).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiSessionInfo
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            string result = $"{nameof(AuthenticationMode)}: {AuthenticationMode} | " +
                            $"{nameof(Username)}: {Username}";
            if (PasswordExpiration != null) { result += $" | {nameof(PasswordExpiration)}: ({PasswordExpiration})"; }
            if (RuntimeTimeout != null) { result += $" | {nameof(RuntimeTimeout)}: {RuntimeTimeout}"; }
            return result;
        }
    }
}
