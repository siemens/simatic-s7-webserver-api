// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// A backup restoration is currently in progress
    /// </summary>
    public class ApiRestoreInProgressException : Exception
    {
        private static readonly string message = "A backup restoration is currently in progress.";
        /// <summary>
        /// A backup restoration is currently in progress
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRestoreInProgressException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// A backup restoration is currently in progress
        /// </summary>
        public ApiRestoreInProgressException() : base(message) { }

        /// <summary>
        /// A backup restoration is currently in progress
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiRestoreInProgressException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// A backup restoration is currently in progress
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRestoreInProgressException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
