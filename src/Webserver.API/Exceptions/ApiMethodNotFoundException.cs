// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Method not found - check method name and PLC firmware version.
    /// </summary>
    public class ApiMethodNotFoundException : Exception
    {
        private static readonly string message = "Method not found - check method name and PLC firmware version.";
        /// <summary>
        /// Method not found - check method name and PLC firmware version.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMethodNotFoundException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Method not found - check method name and PLC firmware version.
        /// </summary>
        public ApiMethodNotFoundException() : base(message) { }

        /// <summary>
        /// Method not found - check method name and PLC firmware version.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiMethodNotFoundException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Method not found - check method name and PLC firmware version.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMethodNotFoundException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
