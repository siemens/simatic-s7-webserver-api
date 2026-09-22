// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested entity is already in use.
    /// </summary>
    public class ApiEntityInUseException : Exception
    {
        private static readonly string message = "The requested entity is already in use.";
        /// <summary>
        /// The requested entity is already in use.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityInUseException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested entity is already in use.
        /// </summary>
        public ApiEntityInUseException() : base(message) { }

        /// <summary>
        /// The requested entity is already in use.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityInUseException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested entity is already in use.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityInUseException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
