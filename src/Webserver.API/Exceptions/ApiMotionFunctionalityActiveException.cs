// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The request cannot be performed while motion functionality is active.
    /// </summary>
    public class ApiMotionFunctionalityActiveException : Exception
    {
        private static readonly string message = "The request cannot be performed while motion functionality is active";
        /// <summary>
        /// The request cannot be performed while motion functionality is active
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMotionFunctionalityActiveException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The request cannot be performed while motion functionality is active
        /// </summary>
        public ApiMotionFunctionalityActiveException() : base(message) { }

        /// <summary>
        /// The request cannot be performed while motion functionality is active
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiMotionFunctionalityActiveException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The request cannot be performed while motion functionality is active
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiMotionFunctionalityActiveException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
