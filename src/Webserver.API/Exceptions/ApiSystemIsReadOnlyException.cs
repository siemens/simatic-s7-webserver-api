// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.
    /// </summary>
    public class ApiSystemIsReadOnlyException : Exception
    {
        private static readonly string message = "The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.";
        /// <summary>
        /// The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.
        /// </summary>
        public ApiSystemIsReadOnlyException() : base(message) { }

        /// <summary>
        /// The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiSystemIsReadOnlyException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiSystemIsReadOnlyException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The system is currently in a write-protected state. Changes to Web Applications are currently not allowed.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiSystemIsReadOnlyException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
