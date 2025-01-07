// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// A class containing a list of authentication modes
    /// </summary>
    public class ApiGetAuthenticationModeResult
    {
        /// <summary>
        /// A list of authentication modes
        /// </summary>
        public List<ApiAuthenticationMode> Authentication_modes { get; set; }
    }
}
