// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
    /// </summary>
    public class ApiHTTPHeaderInvalidException : Exception
    {
        private static string message = "At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.";
        /// <summary>
        /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
        /// </summary>
        public ApiHTTPHeaderInvalidException() : base(message) { }
        /// <summary>
        /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiHTTPHeaderInvalidException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiHTTPHeaderInvalidException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiHTTPHeaderInvalidException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
