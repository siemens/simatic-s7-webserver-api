// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
    /// </summary>
    public class ApiInvalidParametersException : Exception
    {

        private static string message = "Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)";
        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        public ApiInvalidParametersException() : base(message) { }
        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidParametersException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidParametersException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidParametersException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
