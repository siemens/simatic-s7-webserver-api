// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The entity is not a file.
    /// </summary>
    public class ApiEntityNotAFileException : Exception
    {
        private static readonly string message = "The entity is not a file";
        /// <summary>
        /// The entity is not a file
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotAFileException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The entity is not a file
        /// </summary>
        public ApiEntityNotAFileException() : base(message) { }

        /// <summary>
        /// The entity is not a file
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityNotAFileException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The entity is not a file
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotAFileException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
