// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
    /// </summary>
    public class ApiInfrastructureErrorException : Exception
    {
        private static readonly string message = "Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.";
        /// <summary>
        /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInfrastructureErrorException(Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
        /// </summary>
        public ApiInfrastructureErrorException() : base(message) { }
        /// <summary>
        /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiInfrastructureErrorException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiInfrastructureErrorException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
