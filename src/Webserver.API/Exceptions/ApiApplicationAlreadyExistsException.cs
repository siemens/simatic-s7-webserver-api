// Copyright (c) 2021, Siemens AG
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
    /// An application with the given name already exists.
    /// </summary>
    public class ApiApplicationAlreadyExistsException : Exception
    {
        private static string message = "An application with the given name already exists.";
        /// <summary>
        /// An application with the given name already exists.
        /// </summary>
        public ApiApplicationAlreadyExistsException() : base(message) { }
        /// <summary>
        /// An application with the given name already exists.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiApplicationAlreadyExistsException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// An application with the given name already exists.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiApplicationAlreadyExistsException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// An application with the given name already exists.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiApplicationAlreadyExistsException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
