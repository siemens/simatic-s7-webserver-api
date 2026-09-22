// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The version string provided does not meet the criteria of a valid version string.
    /// </summary>
    public class ApiInvalidVersionStringException : Exception
    {
        private static readonly string message = "The version string provided does not meet the criteria of a valid version string. Valid version string example: V1.2";
        /// <summary>
        /// The version string provided does not meet the criteria of a valid version string.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidVersionStringException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The version string provided does not meet the criteria of a valid version string.
        /// </summary>
        public ApiInvalidVersionStringException() : base(message) { }


        /// <summary>
        /// The version string provided does not meet the criteria of a valid version string.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidVersionStringException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The version string provided does not meet the criteria of a valid version string.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidVersionStringException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
