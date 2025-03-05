// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result of a WebServer.ReadDefaultPage request 
    /// </summary>
    public class WebServerDefaultPageResult
    {
        /// <summary>
        /// Default page's name given from the Api
        /// </summary>
        public string Default_page;

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
