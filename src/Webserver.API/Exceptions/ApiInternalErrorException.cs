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
    /// Upon trying to execute the requested operation an internal error occured.
    /// </summary>
    public class ApiInternalErrorException : Exception
    {
        private static string message = "Upon trying to execute the requested operation an internal error occured.";
        /// <summary>
        /// Upon trying to execute the requested operation an internal error occured.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInternalErrorException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Upon trying to execute the requested operation an internal error occured.
        /// </summary>
        public ApiInternalErrorException() : base(message) { }


        /// <summary>
        /// Upon trying to execute the requested operation an internal error occured.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInternalErrorException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Upon trying to execute the requested operation an internal error occured.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInternalErrorException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
