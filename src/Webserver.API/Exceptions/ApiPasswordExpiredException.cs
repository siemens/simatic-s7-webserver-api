// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
    /// </summary>
    public class ApiPasswordExpiredException : Exception
    {
        private static string message = "The password of the user account has expired. The user needs to change the password to successfully authenticate again.";
        /// <summary>
        /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPasswordExpiredException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
        /// </summary>
        public ApiPasswordExpiredException() : base(message) { }
        /// <summary>
        /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPasswordExpiredException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPasswordExpiredException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}