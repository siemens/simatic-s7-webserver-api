// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier.
    /// </summary>
    public class ApiInvalidHwIdException : Exception
    {
        private static string message = "The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier..";
        /// <summary>
        /// The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier.
        /// </summary>
        public ApiInvalidHwIdException() : base(message) { }
        /// <summary>
        /// The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidHwIdException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidHwIdException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested hardware identifier is invalid. The user shall verify the request and look up the correct hardware identifier.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidHwIdException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
