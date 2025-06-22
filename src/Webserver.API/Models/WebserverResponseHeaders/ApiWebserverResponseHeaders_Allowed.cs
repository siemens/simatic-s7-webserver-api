// Copyright (c) 2025, Siemens AG
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
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
