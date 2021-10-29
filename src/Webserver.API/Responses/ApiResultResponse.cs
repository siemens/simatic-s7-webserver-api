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
    public class ApiResultResponse<T> : BaseApiResponse //BaseResultObject => dann wären Properties von BaseResultObject verfügbar
    {
        /// <summary>
        /// The (requested) Result the Api has responded with
        /// </summary>
        public virtual T Result { get; set; }
    }
}
