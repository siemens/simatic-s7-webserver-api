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
    /// The requested resource is not marked as \"public\". 
    /// You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".
    /// </summary>
    public class ApiResourceVisibilityIsNotPublicException : Exception
    {
        private static string message = "The requested resource is not marked as \"public\". You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".";
        /// <summary>
        /// The requested resource is not marked as \"public\". 
        /// You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceVisibilityIsNotPublicException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The requested resource is not marked as \"public\". 
        /// You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".
        /// </summary>
        public ApiResourceVisibilityIsNotPublicException() : base(message) { }


        /// <summary>
        /// The requested resource is not marked as \"public\". 
        /// You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceVisibilityIsNotPublicException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The requested resource is not marked as \"public\". 
        /// You should either change the resource to be \"public\" or request another resource that is already marked as \"public\".
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceVisibilityIsNotPublicException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
