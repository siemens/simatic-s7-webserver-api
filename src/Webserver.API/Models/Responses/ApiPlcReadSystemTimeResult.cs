// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with a ApiPlcReadSystemTimeResult 
    /// </summary>
    public class ApiPlcReadSystemTimeResponse : ApiResultResponse<ApiPlcReadSystemTimeResult>
    {
    }
}
