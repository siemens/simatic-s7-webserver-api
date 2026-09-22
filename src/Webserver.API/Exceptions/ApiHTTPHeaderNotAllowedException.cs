// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
    /// </summary>
    public class ApiHTTPHeaderNotAllowedException : Exception
    {
        private static readonly string message = "At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.";
        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
        /// </summary>
        public ApiHTTPHeaderNotAllowedException() : base(message) { }
        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiHTTPHeaderNotAllowedException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiHTTPHeaderNotAllowedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiHTTPHeaderNotAllowedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
