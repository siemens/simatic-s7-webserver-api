// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The PLC(Server) responded with an invalid message!
    /// </summary>
    public class ApiInvalidResponseException : Exception
    {
        /// <summary>
        /// The PLC(Server) responded with an invalid message!
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiInvalidResponseException(string message) : base(message) { }
    }
}
