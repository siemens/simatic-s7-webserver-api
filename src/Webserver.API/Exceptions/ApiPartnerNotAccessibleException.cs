// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The data of a PLC of an R/H system is not accessible.
    /// This may happen if the system is in state Syncup or RUN-redundant.
    /// </summary>
    public class ApiPartnerNotAccessibleException : Exception
    {
        private static string message = "The data of a PLC of an R/H system is not accessible. This may happen if the system is in state Syncup or RUN-redundant.";
        /// <summary>
        /// The data of a PLC of an R/H system is not accessible. This may happen if the system is in state Syncup or RUN-redundant or if the service data of the partner PLC has been requested.
        /// </summary>
        public ApiPartnerNotAccessibleException() : base(message) { }
        /// <summary>
        /// The data of a PLC of an R/H system is not accessible. This may happen if the system is in state Syncup or RUN-redundant or if the service data of the partner PLC has been requested.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPartnerNotAccessibleException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The data of a PLC of an R/H system is not accessible. This may happen if the system is in state Syncup or RUN-redundant or if the service data of the partner PLC has been requested.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiPartnerNotAccessibleException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The data of a PLC of an R/H system is not accessible. This may happen if the system is in state Syncup or RUN-redundant or if the service data of the partner PLC has been requested.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiPartnerNotAccessibleException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
