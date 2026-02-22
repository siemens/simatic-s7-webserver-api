// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// ApiWebAppBrowseResult: Applications and Max Applications
    /// </summary>
    public class ApiWebAppBrowseResult
    {
        /// <summary>
        /// 4 (as of 2.9 on PLC1500)
        /// </summary>
        public uint Max_Applications { get; set; }

        /// <summary>
        /// WebApplications Array - only 1 Application if Browsing for just that one but always Array!
        /// </summary>
        public List<ApiWebAppData> Applications { get; set; }

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
