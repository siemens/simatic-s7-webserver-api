// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given entity does not refer to an inactive datalog.
    /// </summary>
    public class ApiEntityNotInactiveDatalogException : Exception
    {
        private static readonly string message = "The given entity does not refer to an inactive datalog";
        /// <summary>
        /// The given entity does not refer to an inactive datalog
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotInactiveDatalogException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The given entity does not refer to an inactive datalog
        /// </summary>
        public ApiEntityNotInactiveDatalogException() : base(message) { }

        /// <summary>
        /// The given entity does not refer to an inactive datalog
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiEntityNotInactiveDatalogException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given entity does not refer to an inactive datalog
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiEntityNotInactiveDatalogException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
