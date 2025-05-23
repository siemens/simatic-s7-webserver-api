﻿// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The WebApp Configuration of the given file is invalid.
    /// </summary>
    public class ApiWebAppConfigParserException : Exception
    {
        private static string message = "The WebApp Configuration of the given file is invalid.";
        /// <summary>
        /// The WebApp Configuration of the given file is invalid.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiWebAppConfigParserException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The WebApp Configuration of the given file is invalid.
        /// </summary>
        public ApiWebAppConfigParserException() : base(message) { }

        /// <summary>
        /// The WebApp Configuration of the given file is invalid.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiWebAppConfigParserException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The WebApp Configuration of the given file is invalid.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiWebAppConfigParserException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
