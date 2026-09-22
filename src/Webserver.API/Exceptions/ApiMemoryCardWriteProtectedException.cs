// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The memory card is write-protected.
    /// </summary>
    public class ApiMemoryCardWriteProtectedException : Exception
    {
        private static readonly string message = "The memory card is write-protected";
        /// <summary>
        /// The memory card is write-protected
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMemoryCardWriteProtectedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The memory card is write-protected
        /// </summary>
        public ApiMemoryCardWriteProtectedException() : base(message) { }

        /// <summary>
        /// The memory card is write-protected
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiMemoryCardWriteProtectedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The memory card is write-protected
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMemoryCardWriteProtectedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
