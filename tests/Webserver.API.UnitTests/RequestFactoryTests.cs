// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    /// <summary>
    /// Tests for the RequestFactory!
    /// </summary>
    public class RequestFactoryTests : Base
    {

        /// <summary>
        /// Test that the default value of the Request Factory is to Perform the check (locally) !
        /// </summary>
        /// <returns></returns>
        [Test]
        public void DefaultValueCheckerTrue()
        {
            if(ApiRequestFactory.PerformCheck == false)
            {
                Assert.Fail("Perform Check by default is false!");
            }
            ApiRequestFactory.PerformCheck = false;
            if(ApiRequestFactory.PerformCheck)
            {
                Assert.Fail("Perform Check cannot be edited!");
            }
        }
    }
}
