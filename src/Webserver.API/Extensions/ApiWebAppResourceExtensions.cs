// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Extensions
{
    /// <summary>
    /// Extension Methods on ApiWebAppResource that only some might need
    /// </summary>
    public static class ApiWebAppResourceExtensions
    {
        /// <summary>
        /// Method used to determine wether the file is an HTML file or not
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>resource.Media_type == "text/html"</returns>
        public static bool IsHtmlFile(this ApiWebAppResource resource)
        {
            return (resource.Media_type == "text/html");
        }
    }
}
