// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
    /// </summary>
    public class ApiInvalidETagException : Exception
    {
        private static string message = "The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.";
        /// <summary>
        /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidETagException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
        /// </summary>
        public ApiInvalidETagException() : base(message) { }


        /// <summary>
        /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidETagException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidETagException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
