// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The structure of the given name to the symbolic address is not correct.
    /// </summary>
    public class ApiInvalidAddressException : Exception
    {
        private static readonly string message = "The structure of the given name to the symbolic address is not correct.";
        /// <summary>
        /// The structure of the given name to the symbolic address is not correct.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAddressException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The structure of the given name to the symbolic address is not correct.
        /// </summary>
        public ApiInvalidAddressException() : base(message) { }

        /// <summary>
        /// The structure of the given name to the symbolic address is not correct.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidAddressException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The structure of the given name to the symbolic address is not correct.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAddressException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
