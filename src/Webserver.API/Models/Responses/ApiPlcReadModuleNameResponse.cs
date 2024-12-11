﻿// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with a ApiPlcReadModuleNameResult
    /// </summary>
    public class ApiPlcReadModuleNameResponse : ApiResultResponse<ApiPlcReadModuleNameResult>
    {
    }
}