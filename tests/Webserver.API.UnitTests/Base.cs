﻿// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Requests;
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

        public IRequestParameterChecker RequestParameterChecker;

        public Base()
        {
            ReqIdGenerator = new GUIDGenerator();
            RequestParameterChecker = new RequestParameterChecker();
            ApiRequestFactory = new ApiRequestFactory(ReqIdGenerator, RequestParameterChecker);
        }
    }
}
