// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The entity is not a directory.
    /// </summary>
    public class ApiEntityNotADirectoryException : Exception
    {
        private static readonly string message = "The entity is not a directory.";
        /// <summary>
        /// The entity is not a directory.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotADirectoryException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The entity is not a directory.
        /// </summary>
        public ApiEntityNotADirectoryException() : base(message) { }

        /// <summary>
        /// The entity is not a directory.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityNotADirectoryException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The entity is not a directory.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotADirectoryException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
