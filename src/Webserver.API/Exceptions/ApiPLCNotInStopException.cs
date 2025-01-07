// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
    /// </summary>
    public class ApiPLCNotInStopException : Exception
    {
        private static string message = "The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.";
        /// <summary>
        /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPLCNotInStopException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
        /// </summary>
        public ApiPLCNotInStopException() : base(message) { }

        /// <summary>
        /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPLCNotInStopException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPLCNotInStopException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
