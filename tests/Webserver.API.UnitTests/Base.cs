// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Services.Backup;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System.IO;
using System.Net;

namespace Webserver.API.UnitTests
{
    public class Base
    {
        public ApiHttpClientRequestHandler TestHandler;

        public IPAddress Ip = new IPAddress(new byte[] { 192, 168, 1, 151 });

        public string FQDN = "1515f.dev.webtest";

        public ApiRequestFactory ApiRequestFactory;

        public IIdGenerator ReqIdGenerator;

        public IApiRequestParameterChecker RequestParameterChecker;

        public IApiResponseChecker ApiResponseChecker;

        public IApiBackupHandler BackupHandler;

        public IApiFileHandler FileHandler;

        public static DirectoryInfo CurrentExeDir
        {
            get
            {
                string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return (new FileInfo(dllPath)).Directory;
            }
        }
        public Base()
        {
            ReqIdGenerator = new GUIDGenerator();
            RequestParameterChecker = new ApiRequestParameterChecker();
            ApiRequestFactory = new ApiRequestFactory(ReqIdGenerator, RequestParameterChecker);
            ApiResponseChecker = new ApiResponseChecker();
        }
    }
}
