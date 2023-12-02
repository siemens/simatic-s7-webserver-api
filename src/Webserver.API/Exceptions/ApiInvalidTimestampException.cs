// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided timestamp does not match the required timestamp format
    /// </summary>
    public class ApiInvalidTimestampException : Exception
    {
        private static string message = "The provided timestamp does not match the required timestamp format";
        /// <summary>
        /// The provided timestamp does not match the required timestamp format
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTimestampException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided timestamp does not match the required timestamp format
        /// </summary>
        public ApiInvalidTimestampException() : base(message) { }
        /// <summary>
        /// The provided timestamp does not match the required timestamp format
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidTimestampException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided timestamp does not match the required timestamp format
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTimestampException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
