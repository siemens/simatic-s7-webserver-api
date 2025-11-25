// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.Technology;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Technlogy.BrowseObjects results
    /// </summary>
    public class ApiTechnologyBrowseObjectsResult
    {
        /// <summary>
        /// The PLC type to differentiate motion capabilities
        /// </summary>
        public ApiTechnologyPlcType Type { get; set; }

        /// <summary>
        /// List containing Technology Objects
        /// </summary>
        public List<ApiTechnologyObject> Objects { get; set; }

        /// <summary>
        /// Compares input object to this ApiTechnologyBrowseObjectsResult
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiTechnologyBrowseObjectsResult result &&
                   Type == result.Type &&
                   Objects.Count == result.Objects.Count &&
                   Objects.SequenceEqual(result.Objects);
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>The Hashcode</returns>
        public override int GetHashCode()
        {
            return (Type, Objects).GetHashCode();
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
