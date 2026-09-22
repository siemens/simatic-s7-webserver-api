// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The data for the requested hardware identifier is not readable.
    /// </summary>
    public class ApiIMdataNotReadableException : Exception
    {
        private static readonly string message = "The data for the requested hardware identifier is not readable";
        /// <summary>
        /// The data for the requested hardware identifier is not readable
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiIMdataNotReadableException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The data for the requested hardware identifier is not readable
        /// </summary>
        public ApiIMdataNotReadableException() : base(message) { }

        /// <summary>
        /// The data for the requested hardware identifier is not readable
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiIMdataNotReadableException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The data for the requested hardware identifier is not readable
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiIMdataNotReadableException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
