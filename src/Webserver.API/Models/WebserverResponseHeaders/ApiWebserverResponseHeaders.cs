// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.WebserverResponseHeaders
{
    /// <summary>
    /// The user-configured HTTP response headers, as well as a list of HTTP response headers that are configurable.
    /// </summary>
    public class ApiWebserverResponseHeaders
    {
        /// <summary>
        /// Holds an array of objects where each object represents a HTTP header that is currently configured on the PLC.
        /// </summary>
        public List<ApiWebserverResponseHeaders_Configured> Configured_headers { get; set; }

        /// <summary>
        /// Holds an array of objects where each object represents an entry of a HTTP header that the user is allowed to configure on the PLC.
        /// </summary>
        public List<ApiWebserverResponseHeaders_Allowed> Allowed_headers { get; set; }
    }
}
