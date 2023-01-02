// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Net.Http;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Check the WebApi responses for errors to throw according Exceptions with Error Messages
    /// </summary>
    public interface IApiResponseChecker
    {
        /// <summary>
        /// If the StatusCode is not okay or created throw an InvalidHttpRequestException
        /// </summary>
        /// <param name="message">HttpResponseMessage by PLC</param>
        /// <param name="apiRequestString">requeststring of the ApiRequest sent</param>
        void CheckHttpResponseForErrors(HttpResponseMessage message, string apiRequestString);
        /// <summary>
        /// Check the Plc WebserverApi Response for Errors and throw the according Exception
        /// </summary>
        /// <param name="responseString">responseString got from PLC</param>
        /// <param name="apiRequestString">requeststring of the ApiRequest sent</param>
        void CheckResponseStringForErros(string responseString, string apiRequestString);
    }
}