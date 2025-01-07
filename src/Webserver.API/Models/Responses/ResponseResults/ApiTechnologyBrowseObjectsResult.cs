// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Technology;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Technlogy.BrowseObjects results
    /// </summary>
    public class ApiTechnologyBrowseObjectsResult
    {
        /// <summary>
        /// List containing Technology Objects
        /// </summary>
        public List<ApiTechnologyObject> Objects { get; set; }
    }
}
