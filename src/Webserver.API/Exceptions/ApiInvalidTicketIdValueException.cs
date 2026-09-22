// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The given Ticket Id value does not match 28 Chars which is the only valid value for a ticket id
    /// </summary>
    public class ApiInvalidTicketIdValueException : Exception
    {
        private static readonly string message = "Api Tickets must have 28 bytes of data!";
        /// <summary>
        /// The given Ticket Id value does not match 28 Chars which is the only valid value for a ticket id
        /// </summary>
        public ApiInvalidTicketIdValueException() : base(message) { }

        /// <summary>
        /// The given Ticket Id value does not match 28 Chars which is the only valid value for a ticket id
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTicketIdValueException(Exception innerException) : base(message, innerException) { }


        /// <summary>
        /// The given Ticket Id value does not match 28 Chars which is the only valid value for a ticket id
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInvalidTicketIdValueException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The given Ticket Id value does not match 28 Chars which is the only valid value for a ticket id
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInvalidTicketIdValueException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
