// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// the given Ticket-ID was not found in the user(-token)s list of tickets
    /// </summary>
    public class ApiTicketNotFoundException : Exception
    {
        private static readonly string message = "the given Ticket-ID was not found in the user(-token)s list of tickets";
        /// <summary>
        /// the given Ticket-ID was not found in the user(-token)s list of tickets
        /// </summary>
        public ApiTicketNotFoundException() : base(message) { }
        /// <summary>
        /// the given Ticket-ID was not found in the user(-token)s list of tickets
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTicketNotFoundException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// the given Ticket-ID was not found in the user(-token)s list of tickets
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiTicketNotFoundException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// the given Ticket-ID was not found in the user(-token)s list of tickets
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTicketNotFoundException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
