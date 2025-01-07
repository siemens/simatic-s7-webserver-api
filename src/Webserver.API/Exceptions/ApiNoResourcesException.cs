// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// The system does not have the necessary resources to execute the Web API request. 
    /// Execute the request again as soon as sufficient resources are available again.
    /// Some examples: you have
    ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
    ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
    ///  -system does generally not have the resources currently => wait for other requests to be completed
    /// </summary>
    public class ApiNoResourcesException : Exception
    {
        private static string message = $"The system does not have the necessary resources to execute the Web API request. " +
            $"{Environment.NewLine}Execute the request again as soon as sufficient resources are available again. Some examples: you have{Environment.NewLine}" +
            $" -reached the limit for logins (depending on plc)=> wait 150 seconds and call the method (login) again.{Environment.NewLine}" +
            $" -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. " +
            $"Close all open tickets in order to free resources and call this method again. {Environment.NewLine}" +
            $" -system does generally not have the resources currently => wait for other requests to be completed";
        /// <summary>
        /// The system does not have the necessary resources to execute the Web API request. 
        /// Execute the request again as soon as sufficient resources are available again.
        /// Some examples: you have
        ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
        ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
        ///  -system does generally not have the resources currently => wait for other requests to be completed
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNoResourcesException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// The system does not have the necessary resources to execute the Web API request. 
        /// Execute the request again as soon as sufficient resources are available again.
        /// Some examples: you have
        ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
        ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
        ///  -system does generally not have the resources currently => wait for other requests to be completed
        /// </summary>
        public ApiNoResourcesException() : base(message) { }

        /// <summary>
        /// The system does not have the necessary resources to execute the Web API request. 
        /// Execute the request again as soon as sufficient resources are available again.
        /// Some examples: you have
        ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
        ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
        ///  -system does generally not have the resources currently => wait for other requests to be completed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiNoResourcesException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// The system does not have the necessary resources to execute the Web API request. 
        /// Execute the request again as soon as sufficient resources are available again.
        /// Some examples: you have
        ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
        ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
        ///  -system does generally not have the resources currently => wait for other requests to be completed
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiNoResourcesException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
