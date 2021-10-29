// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    /// <summary>
    /// Static class to check the WebApi responses for errors to throw according Exceptions with Error Messages
    /// </summary>
    public static class ResponseChecker
    {
        /// <summary>
        /// If the StatusCode is not okay or created throw an InvalidHttpRequestException
        /// </summary>
        /// <param name="message">HttpResponseMessage by PLC</param>
        /// <param name="apiRequestString">requeststring of the ApiRequest sent</param>
        public static void CheckHttpResponseForErrors(HttpResponseMessage message, string apiRequestString)
        {
            if (message.StatusCode != System.Net.HttpStatusCode.OK)
            {
                switch (message.StatusCode)
                {
                    //case System.Net.HttpStatusCode.Accepted:
                    //DataLogs create:
                    case System.Net.HttpStatusCode.Created:
                        break;
                    case System.Net.HttpStatusCode.Conflict:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Forbidden:
                    default:
                        var messageForException = $"Request:{apiRequestString.ToString() + Environment.NewLine}" +
                        $"has been responded with{((int)message.StatusCode).ToString() + message.ReasonPhrase}";
                        throw new InvalidHttpRequestException(messageForException);
                }
            }
        }

        /// <summary>
        /// Check the Plc WebserverApi Response for Errors and throw the according Exception
        /// </summary>
        /// <param name="responseString">responseString got from PLC</param>
        /// <param name="apiRequestString">requeststring of the ApiRequest sent</param>
        public static void CheckResponseStringForErros(string responseString, string apiRequestString)
        {
            // apiErrorModel will be null in case no Error is to be thrown!
            ApiErrorModel apiErrorModel = JsonConvert.DeserializeObject<ApiErrorModel>(responseString);
            if(apiErrorModel.Error != null)
            {
                apiErrorModel.ThrowAccordingException(apiRequestString, responseString);
            }
        }
    }
}
