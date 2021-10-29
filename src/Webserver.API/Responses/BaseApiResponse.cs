// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Responses
{
    /// <summary>
    /// Generally an ApiResponse contains "Id" and "JsonRpc"
    /// </summary>
    public abstract class BaseApiResponse
    {
        /// <summary>
        /// Id of the Response (Matches the Id of the according Request)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// JsonRpc of the Response (Matches the Id of the according Request)
        /// </summary>
        public string JsonRpc { get; set; }
    }
}
