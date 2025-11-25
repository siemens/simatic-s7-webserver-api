// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse
{
    /// <summary>
    /// Optional parameter for Modules.ReadParameters request
    /// </summary>
    public class ApiModules_RequestFilters
    {
        /// <summary>
        /// The mode if the attributes shall either be included or excluded. Can be either include or exclude.
        /// </summary>
        public ApiBrowseFilterMode Mode { get; set; }
        /// <summary>
        /// If present, the user has the option to only request certain attributes, see parameter. <br/>
        /// Possible array entries are: "comment", "parameters", "geo_address"
        /// </summary>
        public List<ApiModulesFilterAttribute> Attributes { get; set; }

        /// <summary>
        /// Base constructor for ApiModules_RequestFilters
        /// </summary>
        public ApiModules_RequestFilters() { }
        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="attributes">List of possible filter attributes.</param>
        public ApiModules_RequestFilters(List<ApiModulesFilterAttribute> attributes)
        {
            Attributes = attributes;
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
