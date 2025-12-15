// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
    /// </summary>
    public class ApiInvalidUtcOffsetException : Exception
    {
        private static readonly string message = "The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.";
        /// <summary>
        /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidUtcOffsetException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
        /// </summary>
        public ApiInvalidUtcOffsetException() : base(message) { }
        /// <summary>
        /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidUtcOffsetException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidUtcOffsetException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
