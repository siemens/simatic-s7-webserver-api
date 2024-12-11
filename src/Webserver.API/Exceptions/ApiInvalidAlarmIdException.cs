// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided alarm ID is invalid. This alarm does not exist (anymore).
    /// </summary>
    public class ApiInvalidAlarmIdException : Exception
    {
        private static string message = "The provided alarm ID is invalid. This alarm does not exist (anymore).";
        /// <summary>
        /// The provided alarm ID is invalid. This alarm does not exist (anymore).
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAlarmIdException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided alarm ID is invalid
        /// </summary>
        public ApiInvalidAlarmIdException() : base(message) { }
        /// <summary>
        /// The provided alarm ID is invalid. This alarm does not exist (anymore).
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidAlarmIdException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided alarm ID is invalid. This alarm does not exist (anymore).
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAlarmIdException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
