// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The overall size of all HTTP headers requested to be configured is too large. The user shall either reduce the number of headers or the length of an individual HTTP header.
    /// </summary>
    public class ApiRequestTooLargeException : Exception
    {
        private static string message = "The overall size of all HTTP headers requested to be configured is too large. The user shall either reduce the number of headers or the length of an individual HTTP header.";

        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. The user can choose a HTTP header that is allowed to be set.
        /// </summary>
        public ApiRequestTooLargeException() : base(message) { }
        /// <summary>
        /// The overall size of all HTTP headers requested to be configured is too large. The user shall either reduce the number of headers or the length of an individual HTTP header.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRequestTooLargeException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// The overall size of all HTTP headers requested to be configured is too large. The user shall either reduce the number of headers or the length of an individual HTTP header.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiRequestTooLargeException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The overall size of all HTTP headers requested to be configured is too large. The user shall either reduce the number of headers or the length of an individual HTTP header.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRequestTooLargeException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
