// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// ApiTokenResult: Containing a Token and optional a Web_application_cookie.
    /// </summary>
    public class ApiTokenResult
    {
        /// <summary>
        /// Token given from the Api (used to authenticate in headers as "X-Auth-Token"
        /// </summary>
        public string Token;

        /// <summary>
        /// Not c# conform wording to match Api Response behaviour
        /// </summary>
        public string Web_application_cookie;
    }
}
