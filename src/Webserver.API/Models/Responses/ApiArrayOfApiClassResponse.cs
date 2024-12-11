// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// ApiResponse (Jsonrpc,id) with a List of ApiClasses
    /// </summary>
    public class ApiArrayOfApiClassResponse : ApiResultResponse<List<ApiClass>>
    {
    }
}
