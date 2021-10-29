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
    /// There is no existing application with the given name. Create an application with the given name before calling this method.
    /// </summary>
    public class ApiApplicationDoesNotExistException : Exception
    {
        private static string message = "There is no existing application with the given name. Create an application with the given name before calling this method.";
        /// <summary>
        /// There is no existing application with the given name. Create an application with the given name before calling this method.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiApplicationDoesNotExistException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// There is no existing application with the given name. Create an application with the given name before calling this method.
        /// </summary>
        public ApiApplicationDoesNotExistException() : base(message) { }

        /// <summary>
        /// There is no existing application with the given name. Create an application with the given name before calling this method.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiApplicationDoesNotExistException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// There is no existing application with the given name. Create an application with the given name before calling this method.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiApplicationDoesNotExistException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
