// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.ResponseResults
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
    }
}
