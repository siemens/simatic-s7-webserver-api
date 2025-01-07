// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
    /// </summary>
    public class ApiInvalidApplicationNameException : Exception
    {
        private static string message = $"The given application name is invalid.Set a name that is applying to the naming conventions for an application name from the manual.{Environment.NewLine}";
        /// <summary>
        /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
        /// </summary>
        public ApiInvalidApplicationNameException() : base(message) { }
        /// <summary>
        /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidApplicationNameException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidApplicationNameException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidApplicationNameException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
