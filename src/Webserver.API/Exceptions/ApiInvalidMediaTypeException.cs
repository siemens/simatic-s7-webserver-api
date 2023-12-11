// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
    /// </summary>
    public class ApiInvalidMediaTypeException : Exception
    {
        private static string message = "The proposed media type is inalid. Adjust the proposed media type before calling this method again.";
        /// <summary>
        /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidMediaTypeException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
        /// </summary>
        public ApiInvalidMediaTypeException() : base(message) { }


        /// <summary>
        /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidMediaTypeException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidMediaTypeException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
