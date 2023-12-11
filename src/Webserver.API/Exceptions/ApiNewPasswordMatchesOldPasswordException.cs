// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided new password is identical with the former password.
    /// </summary>
    public class ApiNewPasswordMatchesOldPasswordException : Exception
    {
        private static string message = "The provided new password is identical with the former password.";
        /// <summary>
        /// The provided new password is identical with the former password.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNewPasswordMatchesOldPasswordException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided new password is identical with the former password.
        /// </summary>
        public ApiNewPasswordMatchesOldPasswordException() : base(message) { }
        /// <summary>
        /// The provided new password is identical with the former password.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNewPasswordMatchesOldPasswordException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided new password is identical with the former password.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNewPasswordMatchesOldPasswordException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
