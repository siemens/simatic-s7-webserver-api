// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
    /// The Api Request has been responded with 514 Resource content has been corrupted
    /// </summary>
    public class ApiResourceContentHasBeenCorruptedException : Exception
    {
        private static string message = "The content of the resource requested has been corrupted. Reupload the resource before calling this method again.";
        /// <summary>
        /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceContentHasBeenCorruptedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
        /// </summary>
        public ApiResourceContentHasBeenCorruptedException() : base(message) { }

        /// <summary>
        /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceContentHasBeenCorruptedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceContentHasBeenCorruptedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
