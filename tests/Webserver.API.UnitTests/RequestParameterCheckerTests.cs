// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
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
            //Assert.Throws<ApiInvalidParametersException>(() => requestParameterChecker.CheckLastModified(DateTime.Now.ToString(), true));
            //Assert.Throws<ApiInvalidParametersException>(() => requestParameterChecker.CheckLastModified("2020-08-24T07:08:06.12345678Z", true));
        }

        [Test]
        public void ValidLastModifiedIsAccepted()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            requestParameterChecker.CheckLastModified("2020-08-24T07:08:06.836Z", true);
            requestParameterChecker.CheckLastModified("2020-08-24T07:08:06.52Z", true);
            requestParameterChecker.CheckLastModified("2020-08-24T07:08:06.1Z", true);
            requestParameterChecker.CheckLastModified("2020-08-24T07:08:06Z", true);
        }


        [Test]
        public void InvalidWebAppStateIsAcceptedIfCheckerShouldntCheck()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            Assert.Throws<ApiInvalidParametersException>(() => requestParameterChecker.CheckWebAppState(ApiWebAppState.None, true));
            requestParameterChecker.CheckWebAppState(ApiWebAppState.None, false);
        }

        [Test]
        public void CheckWebAppName()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            var invalidLength = "";
            Assert.Throws<ApiInvalidParametersException>(() => 
            requestParameterChecker.CheckWebAppName(invalidLength, true));
            requestParameterChecker.CheckWebAppName(invalidLength, false);
            for (int i = 0; i <= 100; i++)
            {
                invalidLength += "a";
            }
            Assert.Throws<ApiInvalidApplicationNameException>(() =>
            requestParameterChecker.CheckWebAppName(invalidLength, true));
            requestParameterChecker.CheckWebAppName(invalidLength, false);
            var invalidChars = "$!{}/ÄÖÜ~";
            foreach(var invChar in invalidChars)
            {
                Assert.Throws<ApiInvalidApplicationNameException>(() =>
                requestParameterChecker.CheckWebAppName(invChar.ToString(), true));
                requestParameterChecker.CheckWebAppName(invChar.ToString(), false);
            }
        }

        [Test]
        public void CheckResourceName()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            var invalidLength = "";
            Assert.Throws<ApiInvalidParametersException>(() =>
            requestParameterChecker.CheckResourceName(invalidLength, true));
            requestParameterChecker.CheckResourceName(invalidLength, false);
            for (int i = 0; i <= 200; i++)
            {
                invalidLength += "a";
            }
            Assert.Throws<ApiInvalidResourceNameException>(() =>
            requestParameterChecker.CheckResourceName(invalidLength, true));
            requestParameterChecker.CheckResourceName(invalidLength, false);
            var invalidChars = "${}ÄÖÜ~";
            foreach (var invChar in invalidChars)
            {
                Assert.Throws<ApiInvalidResourceNameException>(() =>
                requestParameterChecker.CheckResourceName(invChar.ToString(), true));
                requestParameterChecker.CheckResourceName(invChar.ToString(), false);
            }
        }

        [Test]
        public void CheckPlcProgramReadOrWriteDataType()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<ApiPlcProgramDataType> types = new List<ApiPlcProgramDataType>()
            // many more!
            { ApiPlcProgramDataType.Struct, ApiPlcProgramDataType.Cref, ApiPlcProgramDataType.Error_struct };
            foreach(var unsuppType in types)
            {
                Assert.Throws<ApiUnsupportedAddressException>(() =>
            requestParameterChecker.CheckPlcProgramReadOrWriteDataType(unsuppType, true));
                requestParameterChecker.CheckPlcProgramReadOrWriteDataType(unsuppType, false);
            }
            var type = ApiPlcProgramDataType.None;
            Assert.Throws<ApiHelperInvalidPlcProgramDataTypeException>(() =>
            requestParameterChecker.CheckPlcProgramReadOrWriteDataType(type, true));
            requestParameterChecker.CheckPlcProgramReadOrWriteDataType(type, false);
            var validType = ApiPlcProgramDataType.Bool;
            requestParameterChecker.CheckPlcProgramReadOrWriteDataType(validType, true);
            requestParameterChecker.CheckPlcProgramReadOrWriteDataType(validType, false);
        }

        [Test]
        public void CheckPlcRequestChangeOperatingMode()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<ApiPlcOperatingMode> invModes = new List<ApiPlcOperatingMode>() {
                ApiPlcOperatingMode.Hold, ApiPlcOperatingMode.None, ApiPlcOperatingMode.Startup,
                ApiPlcOperatingMode.Stop_fwupdate, ApiPlcOperatingMode.Unknown
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                requestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, true));
                requestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, false);
            }
            List<ApiPlcOperatingMode> validModes = new List<ApiPlcOperatingMode>() { ApiPlcOperatingMode.Stop, ApiPlcOperatingMode.Run };
            foreach (var mode in validModes)
            {
                requestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, true);
                requestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, false);
            }
        }

        [Test]
        public void CheckPlcProgramBrowseMode()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<ApiPlcProgramBrowseMode> invModes = new List<ApiPlcProgramBrowseMode>() {
                ApiPlcProgramBrowseMode.None
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                requestParameterChecker.CheckPlcProgramBrowseMode(mode, true));
                requestParameterChecker.CheckPlcProgramBrowseMode(mode, false);
            }
            List<ApiPlcProgramBrowseMode> validModes = new List<ApiPlcProgramBrowseMode>() { ApiPlcProgramBrowseMode.Children, ApiPlcProgramBrowseMode.Var };
            foreach (var mode in validModes)
            {
                requestParameterChecker.CheckPlcProgramBrowseMode(mode, true);
                requestParameterChecker.CheckPlcProgramBrowseMode(mode, false);
            }
        }

        [Test]
        public void CheckPlcProgramReadOrWriteMode()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<ApiPlcProgramReadOrWriteMode> invModes = new List<ApiPlcProgramReadOrWriteMode>() {
                ApiPlcProgramReadOrWriteMode.None
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                requestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, true));
                requestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, false);
            }
            List<ApiPlcProgramReadOrWriteMode?> validModes = new List<ApiPlcProgramReadOrWriteMode?>()
            { ApiPlcProgramReadOrWriteMode.Raw, ApiPlcProgramReadOrWriteMode.Simple, null };
            foreach (var mode in validModes)
            {
                requestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, true);
                requestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, false);
            }
        }

        [Test]
        public void CheckTicket()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<string> invTickets = new List<string>() {
                //29
                "01234567890123456789012345678",
                //27
                "012345678901234567890123456", null, ""
            };
            foreach (var ticket in invTickets)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                requestParameterChecker.CheckTicket(ticket, true));
                requestParameterChecker.CheckTicket(ticket, false);
            }
            List<string> validTickets = new List<string>() { "0123456789012345678901234567" };
            foreach (var ticket in validTickets)
            {
                requestParameterChecker.CheckTicket(ticket, true);
                requestParameterChecker.CheckTicket(ticket, false);
            }
        }

        [Test]
        public void CheckETag()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<string> invEtags = new List<string>() {
                
            };
            var invTag = "";
            for (int i = 0; i <= 128; i++)
            {
                invTag += "a";
            }
            invEtags.Add(invTag);
            foreach (var etag in invEtags)
            {
                Assert.Throws<ApiInvalidETagException>(() =>
                requestParameterChecker.CheckETag(etag, true));
                requestParameterChecker.CheckETag(etag, false);
            }
            List<string> validEtags = new List<string>() { "a", "", null };
            var validTag = "";
            for(int i = 0; i < 128; i++)
            {
                validTag += "a";
            }
            validEtags.Add(validTag);
            foreach (var etag in validEtags)
            {
                requestParameterChecker.CheckETag(etag, true);
                requestParameterChecker.CheckETag(etag, false);
            }
        }

        [Test]
        public void CheckVisibility()
        {
            var requestParameterChecker = new ApiRequestParameterChecker();
            List<ApiWebAppResourceVisibility> invVis = new List<ApiWebAppResourceVisibility>() {
                ApiWebAppResourceVisibility.None
            };
            foreach (var vis in invVis)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                requestParameterChecker.CheckWebAppResourceVisibility(vis, true));
                requestParameterChecker.CheckWebAppResourceVisibility(vis, false);
            }
            List<ApiWebAppResourceVisibility> validVis = new List<ApiWebAppResourceVisibility>()
            { ApiWebAppResourceVisibility.Protected, ApiWebAppResourceVisibility.Public };
            foreach (var vis in validVis)
            {
                requestParameterChecker.CheckWebAppResourceVisibility(vis, true);
                requestParameterChecker.CheckWebAppResourceVisibility(vis, false);
            }
        }
    }
}
