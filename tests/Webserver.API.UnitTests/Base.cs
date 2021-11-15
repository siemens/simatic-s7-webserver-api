// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class Base
    {
        public ApiHttpClientRequestHandler TestHandler;

        public IPAddress Ip = new IPAddress(new byte[] { 192, 168, 1, 180 });

        public ApiRequestFactory ApiRequestFactory;

        public IIdGenerator ReqIdGenerator;

        public IApiRequestParameterChecker RequestParameterChecker;

        public IApiResponseChecker ApiResponseChecker;

        public Base()
        {
            ReqIdGenerator = new GUIDGenerator();
            RequestParameterChecker = new ApiRequestParameterChecker();
            ApiRequestFactory = new ApiRequestFactory(ReqIdGenerator, RequestParameterChecker);
            ApiResponseChecker = new ApiResponseChecker();
        }
    }
}
