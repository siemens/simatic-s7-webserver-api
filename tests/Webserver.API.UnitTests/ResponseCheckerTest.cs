// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ResponseCheckerTest : Base
    {
        [Test]
        public void ValidResponse_ErrorInName_NoExceptionThrown()
        {
            ApiResponseChecker.CheckResponseStringForErros(ResponseStrings.PlcProgramBrowseAll, "{\"method\":\"PlcProgram.Browse\",\"jsonrpc\":\"2.0\",\"id\":\"c02cvuwa\",\"params\":{\"var\":\"\"DataTypes\"\",\"mode\":\"children\"}}");
            ApiResponseChecker.CheckResponseStringForErros(ResponseStrings.PlcProgramBrowseErrorStruct, "{\"method\":\"PlcProgram.Browse\",\"jsonrpc\":\"2.0\",\"id\":\"ibf8wom\",\"params\":{\"var\":\"\"DataTypes\".ErrorStr\",\"mode\":\"children\"}}");
        }
    }
}
