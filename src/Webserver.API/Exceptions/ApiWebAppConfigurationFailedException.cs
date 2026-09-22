// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// WebApp Configuration failed (by deployer)
    /// </summary>
    public class ApiWebAppConfigurationFailedException : Exception
    {
        private static readonly string message = "WebApp configuration failed!";
        /// <summary>
        /// WebApp Configuration failed (by deployer)
        /// </summary>
        public ApiWebAppConfigurationFailedException() : base(message) { }
        /// <summary>
        /// WebApp Configuration failed (by deployer)
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiWebAppConfigurationFailedException(Exception innerException) : base(message, innerException) { }


        /// <summary>
        /// WebApp Configuration failed (by deployer)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiWebAppConfigurationFailedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// WebApp Configuration failed (by deployer)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiWebAppConfigurationFailedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
