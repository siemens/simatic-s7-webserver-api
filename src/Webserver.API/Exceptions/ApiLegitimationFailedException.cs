// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Legitimation to continue the backup restoration failed.
    /// </summary>
    public class ApiLegitimationFailedException : Exception
    {
        private static readonly string message = "Legitimation to continue the backup restoration failed";
        /// <summary>
        /// Legitimation to continue the backup restoration failed
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiLegitimationFailedException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Legitimation to continue the backup restoration failed
        /// </summary>
        public ApiLegitimationFailedException() : base(message) { }

        /// <summary>
        /// Legitimation to continue the backup restoration failed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiLegitimationFailedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Legitimation to continue the backup restoration failed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiLegitimationFailedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
