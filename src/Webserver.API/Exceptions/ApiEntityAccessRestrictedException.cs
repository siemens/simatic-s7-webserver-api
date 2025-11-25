// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Access to the given entity is restricted
    /// </summary>
    public class ApiEntityAccessRestrictedException : Exception
    {
        private static string message = "Access to the given entity is restricted.";
        /// <summary>
        /// Access to the given entity is restricted
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityAccessRestrictedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Access to the given entity is restricted
        /// </summary>
        public ApiEntityAccessRestrictedException() : base(message) { }

        /// <summary>
        /// Access to the given entity is restricted
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityAccessRestrictedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Access to the given entity is restricted
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityAccessRestrictedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
