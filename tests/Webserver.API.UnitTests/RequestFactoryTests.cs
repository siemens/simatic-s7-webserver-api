// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;

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
            if (!ApiRequestFactory.PerformCheck)
            {
                Assert.Fail("Perform Check by default is false!");
            }
            ApiRequestFactory.PerformCheck = false;
            if (ApiRequestFactory.PerformCheck)
            {
                Assert.Fail("Perform Check cannot be edited!");
            }
        }
    }
}
