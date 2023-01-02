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
    /// Browsing the specific address is not possible since the given variable is not a structure.
    /// </summary>
    public class ApiVariableIsNotAStructureException : Exception
    {
        private static string message = "Browsing the specific address is not possible since the given variable is not a structure.";
        /// <summary>
        /// Browsing the specific address is not possible since the given variable is not a structure.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiVariableIsNotAStructureException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Browsing the specific address is not possible since the given variable is not a structure.
        /// </summary>
        public ApiVariableIsNotAStructureException() : base(message) { }

        /// <summary>
        /// Browsing the specific address is not possible since the given variable is not a structure.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiVariableIsNotAStructureException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Browsing the specific address is not possible since the given variable is not a structure.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiVariableIsNotAStructureException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
