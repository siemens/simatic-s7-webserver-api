// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
    /// </summary>
    public class ApiInvalidResourceNameException : Exception
    {
        private static string message = $"The given resource name is invalid. Correct the resource name according to the manuals naming convention " +
            $"before calling this method again.";
        /// <summary>
        /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidResourceNameException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
        /// </summary>
        public ApiInvalidResourceNameException() : base(message) { }

        /// <summary>
        /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidResourceNameException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidResourceNameException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
