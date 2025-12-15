// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.
    /// </summary>
    public class ApiTooManyHTTPHeadersException : Exception
    {
        private static readonly string message = "Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.";
        /// <summary>
        /// Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.
        /// </summary>
        public ApiTooManyHTTPHeadersException() : base(message) { }
        /// <summary>
        /// Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTooManyHTTPHeadersException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiTooManyHTTPHeadersException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Too many HTTP response headers are configured -- reduce the number to a supported number of headers. The number is currently capped at 1.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTooManyHTTPHeadersException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
