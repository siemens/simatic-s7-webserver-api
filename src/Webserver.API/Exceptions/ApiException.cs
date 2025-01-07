// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Exceptions
{
    /// <summary>
    /// Api Exception => containing ErrorModel and by chance the requeststring
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// ApiException => general Exception for the ErroModel; often used as innerException
        /// </summary>
        /// <param name="apiErrorModel">ErrorModel containing the errorcode and errormessage</param>
        public ApiException(ApiErrorModel apiErrorModel)
            : base($"The Api request {Environment.NewLine}" +
            $"would have been responded with (Error):{Environment.NewLine}{JsonConvert.SerializeObject(apiErrorModel, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })}")
        { }
        /// <summary>
        /// ApiException => general Exception for the ErroModel; often used as innerException
        /// </summary>
        /// <param name="apiErrorModel">ErrorModel containing the errorcode and errormessage</param>
        /// <param name="apiRequestString">further information about the Api requeest the user tried to send (or was trying to send)</param>
        public ApiException(ApiErrorModel apiErrorModel, string apiRequestString)
            : base($"The Api request: {apiRequestString}{Environment.NewLine}" +
            $"has been responded with following (Error):{Environment.NewLine}{JsonConvert.SerializeObject(apiErrorModel, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })}")
        { }
    }
}
