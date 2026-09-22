// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given path is too deep.
    /// </summary>
    public class ApiPathTooDeepException : Exception
    {
        private static readonly string message = "The given path is too deep";
        /// <summary>
        /// The given path is too deep
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPathTooDeepException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given path is too deep
        /// </summary>
        public ApiPathTooDeepException() : base(message) { }

        /// <summary>
        /// The given path is too deep
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPathTooDeepException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given path is too deep
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPathTooDeepException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
