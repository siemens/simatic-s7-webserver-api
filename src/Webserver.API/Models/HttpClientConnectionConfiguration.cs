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

        private IApiRequestParameterChecker _requestParameterChecker;
        /// <summary>
        /// Optional Request Parameter Checker for request generation
        /// </summary>
        public IApiRequestParameterChecker RequestParameterChecker
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
        /// Optional (plc-webapi) Response Checker 
        /// </summary>
        public IApiResponseChecker ResponseChecker { get; set; }

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
            this.RequestParameterChecker = new ApiRequestParameterChecker();
            this.ApiRequestFactory = new ApiRequestFactory(IdGenerator, RequestParameterChecker);
            this.ResponseChecker = new ApiResponseChecker();
            this.MediaTypeHeaderValue = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            this.AllowAutoRedirect = false;
            this.DiscardPasswordAfterConnect = true;
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as HttpClientConnectionConfiguration);

        /// <summary>
        /// Equals => ()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(HttpClientConnectionConfiguration obj)
        {
            if (obj is null)
                return false;
            var toReturn = this.BaseAddress == obj.BaseAddress;
            toReturn &= this.Username == obj.Username;
            toReturn &= this.Password == obj.Password;
            toReturn &= this.ConnectionClose == obj.ConnectionClose;
            toReturn &= this.AllowAutoRedirect == obj.AllowAutoRedirect;
            toReturn &= this.DiscardPasswordAfterConnect == obj.DiscardPasswordAfterConnect;
            toReturn &= this.IdGenerator.Equals(obj.IdGenerator);
            toReturn &= this.RequestParameterChecker.Equals(obj.RequestParameterChecker);
            toReturn &= this.ApiRequestFactory.Equals(obj.ApiRequestFactory);
            toReturn &= this.ResponseChecker.Equals(obj.ResponseChecker);
            toReturn &= this.ApiRequestFactory.Equals(obj.ApiRequestFactory);
            toReturn &= this.TimeOut.Equals(obj.TimeOut);
            toReturn &= this.MediaTypeHeaderValue.Equals(obj.MediaTypeHeaderValue);
            return toReturn;
        }

        /// <summary>
        /// GetHashCode for SequenceEqual etc.
        /// </summary>
        /// <returns>hashcode of the connectionConfiguration</returns>
        public override int GetHashCode()
        {
            var hashCode = 939475008;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Username);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
            hashCode = hashCode * -1521134295 + EqualityComparer<MediaTypeHeaderValue>.Default.GetHashCode(MediaTypeHeaderValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<IApiRequestFactory>.Default.GetHashCode(ApiRequestFactory);
            hashCode = hashCode * -1521134295 + EqualityComparer<IIdGenerator>.Default.GetHashCode(IdGenerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<IApiRequestParameterChecker>.Default.GetHashCode(RequestParameterChecker);
            hashCode = hashCode * -1521134295 + EqualityComparer<IApiResponseChecker>.Default.GetHashCode(ResponseChecker);
            hashCode = hashCode * -1521134295 + ConnectionClose.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(TimeOut);
            hashCode = hashCode * -1521134295 + AllowAutoRedirect.GetHashCode();
            hashCode = hashCode * -1521134295 + DiscardPasswordAfterConnect.GetHashCode();
            return hashCode;
        }
    }
}
