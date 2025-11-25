// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided path contains an illegal sequence.
    /// </summary>
    public class ApiPathIllegalSequenceException : Exception
    {
        private static string message = "The provided path contains an illegal sequence.";
        /// <summary>
        /// The provided path contains an illegal sequence
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPathIllegalSequenceException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided path contains an illegal sequence
        /// </summary>
        public ApiPathIllegalSequenceException() : base(message) { }

        /// <summary>
        /// The provided path contains an illegal sequence
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPathIllegalSequenceException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided path contains an illegal sequence
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPathIllegalSequenceException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
