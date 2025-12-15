// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given request could not be parsed successfully.
    /// </summary>
    public class ApiParseErrorException : Exception
    {
        private static readonly string message = "The given request could not be parsed successfully";
        /// <summary>
        /// The given request could not be parsed successfully
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiParseErrorException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given request could not be parsed successfully
        /// </summary>
        public ApiParseErrorException() : base(message) { }

        /// <summary>
        /// The given request could not be parsed successfully
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiParseErrorException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given request could not be parsed successfully
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiParseErrorException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
