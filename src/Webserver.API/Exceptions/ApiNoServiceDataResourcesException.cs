// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Only one simultaneous ticket resource for service data across all users is possible at a time
    /// </summary>
    public class ApiNoServiceDataResourcesException : Exception
    {
        private static readonly string message = "Only one simultaneous ticket resource for service data across all users is possible at a time.";
        /// <summary>
        /// Only one simultaneous ticket resource for service data across all users is possible at a time
        /// </summary>
        public ApiNoServiceDataResourcesException() : base(message) { }
        /// <summary>
        /// Only one simultaneous ticket resource for service data across all users is possible at a time
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNoServiceDataResourcesException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Only one simultaneous ticket resource for service data across all users is possible at a time
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNoServiceDataResourcesException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Only one simultaneous ticket resource for service data across all users is possible at a time
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNoServiceDataResourcesException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
