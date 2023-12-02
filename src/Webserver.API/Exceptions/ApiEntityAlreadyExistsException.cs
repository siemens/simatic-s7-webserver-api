// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested entity already exists.
    /// </summary>
    public class ApiEntityAlreadyExistsException : Exception
    {
        private static string message = "The requested entity already exists.";
        /// <summary>
        /// The requested entity already exists.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityAlreadyExistsException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested entity already exists.
        /// </summary>
        public ApiEntityAlreadyExistsException() : base(message) { }

        /// <summary>
        /// The requested entity already exists.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityAlreadyExistsException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested entity already exists.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityAlreadyExistsException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
