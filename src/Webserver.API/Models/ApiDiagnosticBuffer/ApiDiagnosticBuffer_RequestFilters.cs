// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer
{
    /// <summary>
    /// Optional parameter for ApiDiagnostucBuffer request
    /// </summary>
    public class ApiDiagnosticBuffer_RequestFilters
    {
        /// <summary>
        /// The mode if the attributes shall either be included or excluded. Can be either include or exclude.
        /// </summary>
        public ApiBrowseFilterMode Mode { get; set; }
        /// <summary>
        /// If the entries object is not present in the request, then all parameters must be returned in the response. <br/>
        /// If present, the user has the option to include or exclude certain attributes, see mode parameter. <br/>
        /// Possible array entries are: "short_text", "long_text", "help_text"
        /// </summary>
        public List<ApiDiagnosticBufferBrowseFilterAttributes> Attributes { get; set; }
    }
}
