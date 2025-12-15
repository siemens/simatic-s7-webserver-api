// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The timestamp is not within the allowed range
    /// </summary>
    public class ApiTimestampOutOfRangeException : Exception
    {
        private static readonly string message = "The timestamp is not within the allowed range";
        /// <summary>
        /// The timestamp is not within the allowed range
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTimestampOutOfRangeException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The timestamp is not within the allowed range
        /// </summary>
        public ApiTimestampOutOfRangeException() : base(message) { }
        /// <summary>
        /// The timestamp is not within the allowed range
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiTimestampOutOfRangeException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The timestamp is not within the allowed range
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTimestampOutOfRangeException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
