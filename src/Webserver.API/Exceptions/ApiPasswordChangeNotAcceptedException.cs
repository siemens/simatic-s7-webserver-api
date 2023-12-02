// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
    /// </summary>
    public class ApiPasswordChangeNotAcceptedException : Exception
    {
        private static string message = "The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.";
        /// <summary>
        /// The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPasswordChangeNotAcceptedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        public ApiPasswordChangeNotAcceptedException() : base(message) { }
        /// <summary>
        /// The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPasswordChangeNotAcceptedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPasswordChangeNotAcceptedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
