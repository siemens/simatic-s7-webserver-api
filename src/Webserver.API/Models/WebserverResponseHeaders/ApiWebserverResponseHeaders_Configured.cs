// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

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

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiWebserverResponseHeaders_Configured configured &&
                   Pattern == configured.Pattern &&
                   Header == configured.Header;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Pattern, Header).GetHashCode();
        }

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
