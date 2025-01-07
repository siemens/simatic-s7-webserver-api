// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
    /// </summary>
    public class ApiNotAcceptedException : Exception
    {
        private static string message = "Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.";
        /// <summary>
        /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotAcceptedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        public ApiNotAcceptedException() : base(message) { }
        /// <summary>
        /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNotAcceptedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNotAcceptedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
