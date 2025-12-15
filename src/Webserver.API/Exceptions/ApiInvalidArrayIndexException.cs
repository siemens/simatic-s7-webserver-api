// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
    /// </summary>
    public class ApiInvalidArrayIndexException : Exception
    {
        private static readonly string message = "The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?";
        /// <summary>
        /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidArrayIndexException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
        /// </summary>
        public ApiInvalidArrayIndexException() : base(message) { }

        /// <summary>
        /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidArrayIndexException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidArrayIndexException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
