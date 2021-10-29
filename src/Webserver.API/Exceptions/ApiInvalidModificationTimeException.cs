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
    /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
    /// </summary>
    public class ApiInvalidModificationTimeException : Exception
    {
        private static string message = "The proposed modification time is inalid. Adjust the proposed modification before calling this method again.";
        /// <summary>
        /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
        /// </summary>
        public ApiInvalidModificationTimeException() : base(message) { }
        /// <summary>
        /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidModificationTimeException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidModificationTimeException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidModificationTimeException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
