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
    /// A resource with the given name already exists in this application. 
    /// Either choose a new resource name or delete/rename the existing resource before calling this method again.
    /// </summary>
    public class ApiResourceAlreadyExistsException : Exception
    {
        private static string message = "A resource with the given name already exists in this application. " +
            "Either choose a new resource name or delete/rename the existing resource before calling this method again.";
        /// <summary>
        /// A resource with the given name already exists in this application. 
        /// Either choose a new resource name or delete/rename the existing resource before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceAlreadyExistsException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// A resource with the given name already exists in this application. 
        /// Either choose a new resource name or delete/rename the existing resource before calling this method again.
        /// </summary>
        public ApiResourceAlreadyExistsException() : base(message) { }

        /// <summary>
        /// A resource with the given name already exists in this application. 
        /// Either choose a new resource name or delete/rename the existing resource before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceAlreadyExistsException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// A resource with the given name already exists in this application. 
        /// Either choose a new resource name or delete/rename the existing resource before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceAlreadyExistsException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
