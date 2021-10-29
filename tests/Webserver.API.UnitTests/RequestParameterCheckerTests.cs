// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    class RequestParameterCheckerTests
    {
        [Test]
        public void InvalidLastModifiedIsNotAccepted()
        {
            //Assert.Throws<ApiInvalidParametersException>(() => RequestParameterChecker.CheckLastModified(DateTime.Now.ToString(), true));
        }

        [Test]
        public void ValidLastModifiedIsAccepted()
        {
            RequestParameterChecker.CheckLastModified("2020-08-24T07:08:06.836Z", true);
            RequestParameterChecker.CheckLastModified("2020-08-24T07:08:06.52Z", true);
            RequestParameterChecker.CheckLastModified("2020-08-24T07:08:06.1Z", true);
            RequestParameterChecker.CheckLastModified("2020-08-24T07:08:06Z", true);
            //Assert.Throws<ApiInvalidParametersException>(() => RequestParameterChecker.CheckLastModified("2020-08-24T07:08:06.12345678Z", true));
        }


        [Test]
        public void InvalidWebAppStateIsAcceptedIfCheckerShouldntCheck()
        {
            Assert.Throws<ApiInvalidParametersException>(() => RequestParameterChecker.CheckState(Siemens.Simatic.S7.Webserver.API.Enums.ApiWebAppState.None, true));
            RequestParameterChecker.CheckState(Siemens.Simatic.S7.Webserver.API.Enums.ApiWebAppState.None, false);

        }
    }
}
