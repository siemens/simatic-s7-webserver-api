// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result containing runtime groups
    /// </summary>
    public class ApiFailsafeReadRuntimeGroupsResult
    {
        /// <summary>
        /// List of runtime groups. The list may be empty if no runtime groups exist.
        /// </summary>
        public List<ApiFailsafeRuntimeGroup> Groups { get; set; }

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
