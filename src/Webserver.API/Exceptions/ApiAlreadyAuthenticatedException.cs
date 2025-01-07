// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again"
    /// </summary>
    public class ApiAlreadyAuthenticatedException : Exception
    {
        private static string message = "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again";
        /// <summary>
        /// "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again"
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiAlreadyAuthenticatedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again"
        /// </summary>
        public ApiAlreadyAuthenticatedException() : base(message) { }


        /// <summary>
        /// "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again"
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiAlreadyAuthenticatedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// "The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again"
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiAlreadyAuthenticatedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
