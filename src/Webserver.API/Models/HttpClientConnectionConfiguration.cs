// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Data Class for parameterization of HttpClient configuration
    /// </summary>
    public class HttpClientConnectionConfiguration
    {
        /// <summary>
        /// Function to MemberwiseClone a HttpClientConnectionConfiguration to another Object.
        /// </summary>
        /// <returns></returns>
        public HttpClientConnectionConfiguration ShallowCopy()
        {
            return (HttpClientConnectionConfiguration)this.MemberwiseClone();
        }

        /// <summary>
        /// Mandatory
        /// PLC base Address/DNS name
        /// </summary>
        public string BaseAddress { get; set; }
        /// <summary>
        /// Mandatory
        /// Plc User Management username to login with
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Mandatory
        /// Plc User Management password for Username
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional
        /// defaults to application/json
        /// </summary>
        public MediaTypeHeaderValue MediaTypeHeaderValue { get; set; }

        /// <summary>
        /// Optional
        /// Api request Factory
        /// </summary>
        public IApiRequestFactory ApiRequestFactory { get; set; }

        private IIdGenerator _idGenerator;
        /// <summary>
        /// Optional Id Generator for request ID generation
        /// </summary>
        public IIdGenerator IdGenerator { get
            {
                return _idGenerator;
            }
            set
            {
                _idGenerator = value;
                if(ApiRequestFactory is ApiRequestFactory)
                {
                    ApiRequestFactory = new ApiRequestFactory(_idGenerator, RequestParameterChecker);
                }
            }
        }

        private IRequestParameterChecker _requestParameterChecker;
        /// <summary>
        /// Optional Request Parameter Checker for request generation
        /// </summary>
        public IRequestParameterChecker RequestParameterChecker
        {
            get
            {
                return _requestParameterChecker;
            }
            set
            {
                _requestParameterChecker = value;
                if (ApiRequestFactory is ApiRequestFactory)
                {
                    ApiRequestFactory = new ApiRequestFactory(IdGenerator, _requestParameterChecker);
                }
            }
        }

        /// <summary>
        /// Optional
        /// defaults to false
        /// </summary>
        public bool ConnectionClose { get; set; }

        /// <summary>
        /// Optional 
        /// defaults to ten minutes
        /// </summary>
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// Optional
        /// defaults to false
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Optional defaults to true
        /// </summary>
        public bool DiscardPasswordAfterConnect { get; set; }

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="baseAddress">PLC base Address/DNS name</param>
        /// <param name="username">Plc User Management username to login with</param>
        /// <param name="password">Plc User Management password for Username</param>
        public HttpClientConnectionConfiguration(string baseAddress, string username, string password)
        {
            this.BaseAddress = baseAddress;
            this.Username = username;
            this.Password = password;
            this.TimeOut = TimeSpan.FromMinutes(10);
            this.ConnectionClose = false;
            this.IdGenerator = new GUIDGenerator();
            this.RequestParameterChecker = new RequestParameterChecker();
            this.ApiRequestFactory = new ApiRequestFactory(IdGenerator, RequestParameterChecker);
            this.MediaTypeHeaderValue = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            this.AllowAutoRedirect = false;
            this.DiscardPasswordAfterConnect = true;
        }
    }
}
