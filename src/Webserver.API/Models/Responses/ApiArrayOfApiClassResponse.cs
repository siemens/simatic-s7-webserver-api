// Copyright (c) 2021, Siemens AG
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
    /// ApiResponse (Jsonrpc,id) with a List of ApiClasses
    /// </summary>
    public class ApiArrayOfApiClassResponse : ApiResultResponse <List<ApiClass>>
    {
    }
}
