// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with an ApiTokenResult
    /// </summary>
    public class ApiLoginResponse : ApiResultResponse<ApiTokenResult>
    {
        
    }
}
