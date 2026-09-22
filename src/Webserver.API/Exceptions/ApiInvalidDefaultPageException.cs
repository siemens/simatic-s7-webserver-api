// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given default page is invalid cannot be set.
    /// </summary>
    public class ApiInvalidDefaultPageException : Exception
    {
        private static readonly string message = "The given default page is invalid cannot be set";
        /// <summary>
        /// The given default page is invalid cannot be set
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidDefaultPageException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given default page is invalid cannot be set
        /// </summary>
        public ApiInvalidDefaultPageException() : base(message) { }

        /// <summary>
        /// The given default page is invalid cannot be set
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidDefaultPageException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given default page is invalid cannot be set
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidDefaultPageException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
