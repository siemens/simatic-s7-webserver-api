// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The requested operation cannot be executet since the system is currently processing another request. 
    /// Send the request again once the currently processed request is completed
    /// </summary>
    public class ApiSystemIsBusyException : Exception
    {
        private static readonly string message = "The requested operation cannot be executet since the system is currently processing another request." +
            " Send the request again once the currently processed request is completed";
        /// <summary>
        /// The requested operation cannot be executet since the system is currently processing another request. 
        /// Send the request again once the currently processed request is completed
        /// </summary>
        public ApiSystemIsBusyException() : base(message) { }

        /// <summary>
        /// The requested operation cannot be executet since the system is currently processing another request. 
        /// Send the request again once the currently processed request is completed
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiSystemIsBusyException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// The requested operation cannot be executet since the system is currently processing another request. 
        /// Send the request again once the currently processed request is completed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiSystemIsBusyException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested operation cannot be executet since the system is currently processing another request. 
        /// Send the request again once the currently processed request is completed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiSystemIsBusyException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
