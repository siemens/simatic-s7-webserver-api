// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The ticketing Upload failed! Not enough memory left on the card?
    /// </summary>
    public class ApiTicketingEndpointUploadException : Exception
    {
        private static string message = $"The ticketing Upload failed! Not enough memory left on the card? Ticket Id:{Environment.NewLine}";
        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTicketingEndpointUploadException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        public ApiTicketingEndpointUploadException() : base(message) { }


        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        /// <param name="ticketId">ticketId of the upload request</param>
        public ApiTicketingEndpointUploadException(string ticketId) : base(message + Environment.NewLine + ticketId) { }
        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        /// <param name="ticketId">ticketId of the upload request</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTicketingEndpointUploadException(string ticketId, Exception innerException) : base(message + Environment.NewLine + ticketId, innerException) { }

        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        /// <param name="ticketId">ticketId of the upload request</param>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiTicketingEndpointUploadException(string ticketId, string userMessage) : base(message + Environment.NewLine + ticketId + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The ticketing Upload failed! Not enough memory left on the card?
        /// </summary>
        /// <param name="ticketId">ticketId of the upload request</param>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiTicketingEndpointUploadException(string ticketId, string userMessage, Exception innerException) : base(message + Environment.NewLine + ticketId + Environment.NewLine + userMessage, innerException) { }
    }
}
