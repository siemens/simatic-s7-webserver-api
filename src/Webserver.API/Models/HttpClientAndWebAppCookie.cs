// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Class to be returned from AuthorizationHandler in case the user wants to get the WebApplicationCookie Value on top of the Authorized Client (for the login provided)
    /// </summary>
    public class HttpClientAndWebAppCookie
    {
        /// <summary>
        /// Authorized HTTPClient: Has set the X-Auth-Token that has been given by the Plc Response to the headers of the requests (generally)
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Value of the WebApplicationCookie => set this to the WebApplicationCookie in your browser to be able to access the protected pages e.g.
        /// </summary>
        public string WebApplicationCookie { get; set; }

        /// <summary>
        /// Create a new Instance of HttpClientAndWebAppCookie
        /// </summary>
        /// <param name="httpClient">Authorized HTTPClient: Has set the X-Auth-Token that has been given by the Plc Response to the headers of the requests (generally)</param>
        /// <param name="webApplicationCookie">v</param>
        public HttpClientAndWebAppCookie(HttpClient httpClient, string webApplicationCookie)
        {
            this.HttpClient = httpClient;
            this.WebApplicationCookie = webApplicationCookie;
        }
    }
}
