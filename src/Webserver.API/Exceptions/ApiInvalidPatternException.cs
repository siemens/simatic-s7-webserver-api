// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
    /// </summary>
    public class ApiInvalidPatternException : Exception
    {
        private static string message = "The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.";
        /// <summary>
        /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
        /// </summary>
        public ApiInvalidPatternException() : base(message) { }
        /// <summary>
        /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidPatternException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidPatternException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidPatternException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
