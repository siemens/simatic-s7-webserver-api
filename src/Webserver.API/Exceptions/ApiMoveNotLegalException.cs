// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Moving of the given entity is not allowed.
    /// </summary>
    public class ApiMoveNotLegalException : Exception
    {
        private static readonly string message = "Moving of the given entity is not allowed";
        /// <summary>
        /// Moving of the given entity is not allowed
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMoveNotLegalException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Moving of the given entity is not allowed
        /// </summary>
        public ApiMoveNotLegalException() : base(message) { }

        /// <summary>
        /// Moving of the given entity is not allowed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiMoveNotLegalException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Moving of the given entity is not allowed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMoveNotLegalException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
