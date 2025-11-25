// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// ApiTokenResult: Containing a Token and an optional Web_application_cookie.
    /// May also contain password_expiration information if the authentication mode is "local" and is enabled on the PLC.
    /// </summary>
    public class ApiTokenResult
    {
        /// <summary>
        /// Token given from the Api (used to authenticate in headers as "X-Auth-Token"
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Not c# conform wording to match Api Response behaviour
        /// </summary>
        public string Web_application_cookie { get; set; }

        /// <summary>
        /// Holds password expiration information if local authentication mode is used,
        /// and password expiration is enabled on the PLC.
        /// </summary>
        public ApiPasswordExpiration Password_expiration { get; set; }

        /// <summary>
        /// The inactivity duration after which a log out (using the API method Api.Logout) will be automatically performed.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Runtime_timeout { get; set; }

        /// <summary>
        /// The attribute tells immedisately if the authenticated user has no webserver permissions.
        /// This provides immediate feedback that the user cannot be used for webserver activities without a call to Api.GetPermissions.
        /// </summary>
        public bool Has_no_permissions { get; set; }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
