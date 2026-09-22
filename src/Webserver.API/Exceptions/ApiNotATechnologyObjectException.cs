// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Exception for Error code 1400 
    /// </summary>
    public class ApiNotATechnologyObjectException : Exception
    {
        private static readonly string message = "The accessed variable is not a variable of a technology object and cannot be read.";
        /// <summary>
        /// The accessed variable is not a variable of a technology object and cannot be read.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotATechnologyObjectException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The accessed variable is not a variable of a technology object and cannot be read.
        /// </summary>
        public ApiNotATechnologyObjectException() : base(message) { }

        /// <summary>
        /// The accessed variable is not a variable of a technology object and cannot be read.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNotATechnologyObjectException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The accessed variable is not a variable of a technology object and cannot be read.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotATechnologyObjectException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
