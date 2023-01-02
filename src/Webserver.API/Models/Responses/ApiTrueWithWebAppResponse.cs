// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Response for ApiMethods that change a WebApp
    /// </summary>
    public class ApiTrueWithWebAppResponse
    {
        /// <summary>
        /// TrueOnSuccesResponse by the PLC
        /// </summary>
        public ApiTrueOnSuccessResponse TrueOnSuccesResponse { get; set; }

        /// <summary>
        /// WebAppData adjusted(created) by the Implementation of the Methods
        /// </summary>
        public ApiWebAppData NewWebApp { get; set; }
    }
}
