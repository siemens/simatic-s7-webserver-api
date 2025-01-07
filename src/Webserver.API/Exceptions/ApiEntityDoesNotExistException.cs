// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested entity does not exist or the webserver cannot access the requested address.
    /// </summary>
    public class ApiEntityDoesNotExistException : Exception
    {
        private static string message = "The requested entity does not exist or the webserver cannot access the requested address.";
        /// <summary>
        /// The requested entity does not exist or the webserver cannot access the requested address.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityDoesNotExistException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested entity does not exist or the webserver cannot access the requested address.
        /// </summary>
        public ApiEntityDoesNotExistException() : base(message) { }

        /// <summary>
        /// The requested entity does not exist or the webserver cannot access the requested address.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityDoesNotExistException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested entity does not exist or the webserver cannot access the requested address.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityDoesNotExistException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
