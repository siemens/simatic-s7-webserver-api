// Copyright (c) 2021, Siemens AG
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
    /// The request has been responded with an error code
    /// </summary>
    public class InvalidHttpRequestException : Exception
    {
        /// <summary>
        /// The request has been responded with an error code
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidHttpRequestException(string message) : base(message) { }
        /// <summary>
        /// The request has been responded with an error code
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidHttpRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
