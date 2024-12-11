// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// During consistency check an Error has occured
    /// </summary>
    public class ApiInconsistentApiWebAppDataException : Exception
    {
        private static string message = "The ApiWebAppData is inconsistent.";
        /// <summary>
        /// The ApiWebAppData is inconsistent.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInconsistentApiWebAppDataException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The ApiWebAppData is inconsistent.
        /// </summary>
        public ApiInconsistentApiWebAppDataException() : base(message) { }

        /// <summary>
        /// The ApiWebAppData is inconsistent.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInconsistentApiWebAppDataException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The ApiWebAppData is inconsistent.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInconsistentApiWebAppDataException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
