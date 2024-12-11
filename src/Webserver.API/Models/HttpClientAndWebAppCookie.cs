// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using System.Net.Http;

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

        /// <summary>
        /// Check for HttpClient and WebAppCookie
        /// </summary>
        /// <param name="obj">to compare</param>
        /// <returns>wether Properties are having the same values</returns>
        public override bool Equals(object obj)
        {
            var cookie = obj as HttpClientAndWebAppCookie;
            return cookie != null &&
                   EqualityComparer<HttpClient>.Default.Equals(HttpClient, cookie.HttpClient) &&
                   WebApplicationCookie == cookie.WebApplicationCookie;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the HttpClientAndWebAppCookie</returns>
        public override int GetHashCode()
        {
            var hashCode = 1898870105;
            hashCode = hashCode * -1521134295 + EqualityComparer<HttpClient>.Default.GetHashCode(HttpClient);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WebApplicationCookie);
            return hashCode;
        }
    }
}
