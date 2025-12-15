// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.
    /// </summary>
    public class ApiRequestBiggerThanMaxMessageSizeException : Exception
    {
        private static readonly string message = "The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.";
        /// <summary>
        /// The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRequestBiggerThanMaxMessageSizeException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.
        /// </summary>
        public ApiRequestBiggerThanMaxMessageSizeException() : base(message) { }

        /// <summary>
        /// The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiRequestBiggerThanMaxMessageSizeException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The Api Request is bigger than the Max Message Size. The request cannot be sent to the server.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRequestBiggerThanMaxMessageSizeException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
