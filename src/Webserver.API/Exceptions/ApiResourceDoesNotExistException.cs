// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
    /// </summary>
    public class ApiResourceDoesNotExistException : Exception
    {
        private static string message = "The requested resource does not exist inside the given application.Request a resource that exists in the application.";
        /// <summary>
        /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceDoesNotExistException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
        /// </summary>
        public ApiResourceDoesNotExistException() : base(message) { }

        /// <summary>
        /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceDoesNotExistException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceDoesNotExistException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
