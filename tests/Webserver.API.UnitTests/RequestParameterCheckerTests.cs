// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
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

        [Test]
        public void CheckWebAppName()
        {
            var invalidLength = "";
            Assert.Throws<ApiInvalidParametersException>(() => 
            RequestParameterChecker.CheckWebAppName(invalidLength, true));
            RequestParameterChecker.CheckWebAppName(invalidLength, false);
            for (int i = 0; i <= 100; i++)
            {
                invalidLength += "a";
            }
            Assert.Throws<ApiInvalidApplicationNameException>(() =>
            RequestParameterChecker.CheckWebAppName(invalidLength, true));
            RequestParameterChecker.CheckWebAppName(invalidLength, false);
            var invalidChars = "$!{}/ÄÖÜ~";
            foreach(var invChar in invalidChars)
            {
                Assert.Throws<ApiInvalidApplicationNameException>(() =>
                RequestParameterChecker.CheckWebAppName(invChar.ToString(), true));
                RequestParameterChecker.CheckWebAppName(invChar.ToString(), false);
            }
        }

        [Test]
        public void CheckResourceName()
        {
            var invalidLength = "";
            Assert.Throws<ApiInvalidParametersException>(() =>
            RequestParameterChecker.CheckResourceName(invalidLength, true));
            RequestParameterChecker.CheckResourceName(invalidLength, false);
            for (int i = 0; i <= 200; i++)
            {
                invalidLength += "a";
            }
            Assert.Throws<ApiInvalidResourceNameException>(() =>
            RequestParameterChecker.CheckResourceName(invalidLength, true));
            RequestParameterChecker.CheckResourceName(invalidLength, false);
            var invalidChars = "${}ÄÖÜ~";
            foreach (var invChar in invalidChars)
            {
                Assert.Throws<ApiInvalidResourceNameException>(() =>
                RequestParameterChecker.CheckResourceName(invChar.ToString(), true));
                RequestParameterChecker.CheckResourceName(invChar.ToString(), false);
            }
        }

        [Test]
        public void CheckPlcProgramReadOrWriteDataType()
        {
            List<ApiPlcProgramDataType> types = new List<ApiPlcProgramDataType>()
            // many more!
            { ApiPlcProgramDataType.Struct, ApiPlcProgramDataType.Cref, ApiPlcProgramDataType.Error_struct };
            foreach(var unsuppType in types)
            {
                Assert.Throws<ApiUnsupportedAddressException>(() =>
            RequestParameterChecker.CheckPlcProgramWriteOrReadDataType(unsuppType, true));
                RequestParameterChecker.CheckPlcProgramWriteOrReadDataType(unsuppType, false);
            }
            var type = ApiPlcProgramDataType.None;
            Assert.Throws<ApiHelperInvalidPlcProgramDataTypeException>(() =>
            RequestParameterChecker.CheckPlcProgramWriteOrReadDataType(type, true));
            RequestParameterChecker.CheckPlcProgramWriteOrReadDataType(type, false);
        }

        [Test]
        public void CheckPlcRequestChangeOperatingMode()
        {
            List<ApiPlcOperatingMode> invModes = new List<ApiPlcOperatingMode>() {
                ApiPlcOperatingMode.Hold, ApiPlcOperatingMode.None, ApiPlcOperatingMode.Startup,
                ApiPlcOperatingMode.Stop_fwupdate, ApiPlcOperatingMode.Unknown
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                RequestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, true));
                RequestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, false);
            }
            List<ApiPlcOperatingMode> validModes = new List<ApiPlcOperatingMode>() { ApiPlcOperatingMode.Stop, ApiPlcOperatingMode.Run };
            foreach (var mode in validModes)
            {
                RequestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, true);
                RequestParameterChecker.CheckPlcRequestChangeOperatingMode(mode, false);
            }
        }

        [Test]
        public void CheckPlcProgramBrowseMode()
        {
            List<ApiPlcProgramBrowseMode> invModes = new List<ApiPlcProgramBrowseMode>() {
                ApiPlcProgramBrowseMode.None
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                RequestParameterChecker.CheckPlcProgramBrowseMode(mode, true));
                RequestParameterChecker.CheckPlcProgramBrowseMode(mode, false);
            }
            List<ApiPlcProgramBrowseMode> validModes = new List<ApiPlcProgramBrowseMode>() { ApiPlcProgramBrowseMode.Children, ApiPlcProgramBrowseMode.Var };
            foreach (var mode in validModes)
            {
                RequestParameterChecker.CheckPlcProgramBrowseMode(mode, true);
                RequestParameterChecker.CheckPlcProgramBrowseMode(mode, false);
            }
        }

        [Test]
        public void CheckPlcProgramReadOrWriteMode()
        {
            List<ApiPlcProgramReadOrWriteMode> invModes = new List<ApiPlcProgramReadOrWriteMode>() {
                ApiPlcProgramReadOrWriteMode.None
            };
            foreach (var mode in invModes)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                RequestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, true));
                RequestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, false);
            }
            List<ApiPlcProgramReadOrWriteMode?> validModes = new List<ApiPlcProgramReadOrWriteMode?>()
            { ApiPlcProgramReadOrWriteMode.Raw, ApiPlcProgramReadOrWriteMode.Simple, null };
            foreach (var mode in validModes)
            {
                RequestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, true);
                RequestParameterChecker.CheckPlcProgramReadOrWriteMode(mode, false);
            }
        }

        [Test]
        public void CheckTicket()
        {
            List<string> invTickets = new List<string>() {
                //29
                "01234567890123456789012345678",
                //27
                "012345678901234567890123456", null, ""
            };
            foreach (var ticket in invTickets)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                RequestParameterChecker.CheckTicket(ticket, true));
                RequestParameterChecker.CheckTicket(ticket, false);
            }
            List<string> validTickets = new List<string>() { "0123456789012345678901234567" };
            foreach (var ticket in validTickets)
            {
                RequestParameterChecker.CheckTicket(ticket, true);
                RequestParameterChecker.CheckTicket(ticket, false);
            }
        }

        [Test]
        public void CheckETag()
        {
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
                RequestParameterChecker.CheckETag(etag, true));
                RequestParameterChecker.CheckETag(etag, false);
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
                RequestParameterChecker.CheckETag(etag, true);
                RequestParameterChecker.CheckETag(etag, false);
            }
        }

        [Test]
        public void CheckVisibility()
        {
            List<ApiWebAppResourceVisibility> invVis = new List<ApiWebAppResourceVisibility>() {
                ApiWebAppResourceVisibility.None
            };
            foreach (var vis in invVis)
            {
                Assert.Throws<ApiInvalidParametersException>(() =>
                RequestParameterChecker.CheckVisibility(vis, true));
                RequestParameterChecker.CheckVisibility(vis, false);
            }
            List<ApiWebAppResourceVisibility> validVis = new List<ApiWebAppResourceVisibility>()
            { ApiWebAppResourceVisibility.Protected, ApiWebAppResourceVisibility.Public };
            foreach (var vis in validVis)
            {
                RequestParameterChecker.CheckVisibility(vis, true);
                RequestParameterChecker.CheckVisibility(vis, false);
            }
        }
    }
}
