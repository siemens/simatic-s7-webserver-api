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
    /// Resource Deployment failed (using the Deployer)
    /// </summary>
    public class ApiResourceDeploymentFailedException : Exception
    {
        private static string message = "Could not successfully deploy the resources correctly!";
        /// <summary>
        /// Resource Deployment failed (using the Deployer)
        /// </summary>
        public ApiResourceDeploymentFailedException() : base(message) { }
        /// <summary>
        /// Resource Deployment failed (using the Deployer)
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceDeploymentFailedException(Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Resource Deployment failed (using the Deployer)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        public ApiResourceDeploymentFailedException(string userMessage) : base(message + Environment.NewLine + userMessage) { }
        /// <summary>
        /// Resource Deployment failed (using the Deployer)
        /// </summary>
        /// <param name="userMessage">Further information about the error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiResourceDeploymentFailedException(string userMessage, Exception innerException) : base(message + Environment.NewLine + userMessage, innerException) { }
    }
}
