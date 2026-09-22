// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.WebserverResponseHeaders
{
    /// <summary>
    /// Represents an entry of a HTTP header that is allowed to be configured on the PLC.
    /// </summary>
    public class ApiWebserverResponseHeaders_Allowed
    {
        /// <summary>
        /// The pattern for which the header may be applied for.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The HTTP response header key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiWebserverResponseHeaders_Allowed allowed &&
                   Pattern == allowed.Pattern &&
                   Key == allowed.Key;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Pattern, Key).GetHashCode();
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
