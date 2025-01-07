// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.WebserverResponseHeaders
{
    /// <summary>
    /// Represents a HTTP header that is currently configured on the PLC.
    /// </summary>
    public class ApiWebserverResponseHeaders_Configured
    {
        /// <summary>
        /// The pattern defines for which webserver endpoint this HTTP header must be returned for. <br/>
        /// For now, this value is always /~**/*.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The HTTP response header.
        /// </summary>
        public string Header { get; set; }
    }
}
