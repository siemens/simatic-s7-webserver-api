// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiErrors always contain ErrorCodes and ErrorMessages
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// ErrorCode sent in the Response
        /// </summary>
        public ApiErrorCode Code { get; set; }

        /// <summary>
        /// Message sent in the Response
        /// </summary>
        public string Message { get; set; }
    }
}
