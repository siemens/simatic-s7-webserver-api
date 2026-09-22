// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Reading of I and M data is not supported for the requested hardware identifier.
    /// </summary>
    public class ApiIMdataNotSupportedException : Exception
    {
        private static readonly string message = "Reading of I and M data is not supported for the requested hardware identifier";
        /// <summary>
        /// Reading of I and M data is not supported for the requested hardware identifier
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiIMdataNotSupportedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Reading of I and M data is not supported for the requested hardware identifier
        /// </summary>
        public ApiIMdataNotSupportedException() : base(message) { }

        /// <summary>
        /// Reading of I and M data is not supported for the requested hardware identifier
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiIMdataNotSupportedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Reading of I and M data is not supported for the requested hardware identifier
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiIMdataNotSupportedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
