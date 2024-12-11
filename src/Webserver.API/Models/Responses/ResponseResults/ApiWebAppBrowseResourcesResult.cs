// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// ApiWebAppBrowseResourcesResult: containing Max_Resources and the requested application resources in an Array
    /// </summary>
    public class ApiWebAppBrowseResourcesResult
    {
        /// <summary>
        /// As of 2.9: 200
        /// </summary>
        public uint Max_Resources { get; set; }

        /// <summary>
        /// Application Resources or only 1 resource when browsing just that one resource (but always Array)
        /// </summary>
        public List<ApiWebAppResource> Resources { get; set; }
    }
}
