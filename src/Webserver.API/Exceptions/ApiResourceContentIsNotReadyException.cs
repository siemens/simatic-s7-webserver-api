// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
    /// Resource content is not ready exception - might happen e.g. right after/during upload to ticketing endpoint and trying to acces/download the file
    /// </summary>
    public class ApiResourceContentIsNotReadyException : Exception
    {
        private static readonly string message = "The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.";
        /// <summary>
        /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
        /// Resource content is not ready exception - might happen e.g. right after/during upload to ticketing endpoint and trying to acces/download the file
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceContentIsNotReadyException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
        /// Resource content is not ready exception - might happen e.g. right after/during upload to ticketing endpoint and trying to acces/download the file
        /// </summary>
        public ApiResourceContentIsNotReadyException() : base(message) { }

        /// <summary>
        /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
        /// Resource content is not ready exception - might happen e.g. right after/during upload to ticketing endpoint and trying to acces/download the file
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceContentIsNotReadyException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
        /// Resource content is not ready exception - might happen e.g. right after/during upload to ticketing endpoint and trying to acces/download the file
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceContentIsNotReadyException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
