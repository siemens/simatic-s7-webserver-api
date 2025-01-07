// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

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
        public ApiUserAuthenticationMode Authentication_Mode { get; set; }

        /// <summary>
        /// Username of the requested session.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password expiration information. Used only when the authentication mode local or umc is used, and the password expiration functionality is enabled on the PLC.
        /// </summary>
        public ApiPasswordExpiration Password_Expiration { get; set; }

        /// <summary>
        /// The inactivity timespan after which a client application may perform a log out using API method Api.Logout.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan? Runtime_Timeout { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>Returns true if the ApiQuantityStructures are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiSessionInfo;
            if (structure == null) { return false; }
            if ((structure.Password_Expiration == null) != (this.Password_Expiration == null))
            {
                return false;
            }
            if ((structure.Runtime_Timeout == null) != (this.Runtime_Timeout == null))
            {
                return false;
            }
            if (structure.Password_Expiration != null)
            {
                if (!structure.Password_Expiration.Equals(this.Password_Expiration)) { return false; }
            }
            if (structure.Runtime_Timeout != null)
            {
                if (structure.Runtime_Timeout != this.Runtime_Timeout) { return false; }
            }
            return structure.Authentication_Mode == this.Authentication_Mode &&
                   structure.Username == this.Username;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode for the ApiQuantityStructures</returns>
        public override int GetHashCode()
        {
            return (Authentication_Mode, Username, Password_Expiration, Runtime_Timeout).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiSessionInfo
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            string result = $"{nameof(Authentication_Mode)}: {Authentication_Mode} | " +
                            $"{nameof(Username)}: {Username}";
            if (Password_Expiration != null) { result += $" | {nameof(Password_Expiration)}: ({Password_Expiration.ToString()})"; }
            if (Runtime_Timeout != null) { result += $" | {nameof(Runtime_Timeout)}: {Runtime_Timeout}"; }
            return result;
        }
    }
}
