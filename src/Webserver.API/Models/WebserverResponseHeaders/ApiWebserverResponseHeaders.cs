// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiWebserverResponseHeaders headers &&
                   Configured_headers.Count == headers.Configured_headers.Count &&
                   Configured_headers.SequenceEqual(headers.Configured_headers) &&
                   Allowed_headers.Count == headers.Allowed_headers.Count &&
                   Allowed_headers.SequenceEqual(headers.Allowed_headers);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Configured_headers, Allowed_headers).GetHashCode();
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
