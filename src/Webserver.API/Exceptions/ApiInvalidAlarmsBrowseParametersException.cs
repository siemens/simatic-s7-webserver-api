// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
    /// </summary>
    public class ApiInvalidAlarmsBrowseParametersException : Exception
    {
        private static string message = "The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.";
        /// <summary>
        /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAlarmsBrowseParametersException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
        /// </summary>
        public ApiInvalidAlarmsBrowseParametersException() : base(message) { }
        /// <summary>
        /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidAlarmsBrowseParametersException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidAlarmsBrowseParametersException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
