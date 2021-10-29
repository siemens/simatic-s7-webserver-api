// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Simatic.S7.Webserver.API.Models;

namespace Siemens.Simatic.S7.Webserver.API.Responses
{
    /// <summary>
    /// Response for ApiMethods that change a resource
    /// </summary>
    public class ApiTrueWithResourceResponse
    {
        /// <summary>
        /// TrueOnSuccesResponse by the PLC
        /// </summary>
        public ApiTrueOnSuccessResponse TrueOnSuccesResponse;

        /// <summary>
        /// ResourceData adjusted(created) by the Implementation of the Methods
        /// </summary>
        public ApiWebAppResource NewResource;
    }
}
