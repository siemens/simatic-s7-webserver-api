// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.FileHandling;
using System.IO;

namespace Webserver.API.UnitTests
{
    public class ApiFileHandlerTests
    {
        [TestCase("../outside.txt")]
        [TestCase("..\\outside.txt")]
        [TestCase("nested\\../../outside.txt")]
        [TestCase("/outside.txt")]
        [TestCase("\\outside.txt")]
        public void DeployFileAsync_PathOutsideLocalDirectory_ThrowsBeforeCreatingTicket(string resourceName)
        {
            var resource = new ApiFileResource
            {
                PathToLocalDirectory = Path.Combine(Path.GetTempPath(), "upload"),
                Name = resourceName
            };
            var handler = new ApiFileHandler(null, null);

            Assert.ThrowsAsync<IOException>(() => handler.DeployFileAsync(resource));
        }

        [Test]
        public void DeployFileAsync_AbsoluteResourcePath_ThrowsBeforeCreatingTicket()
        {
            var rootDirectory = Path.Combine(Path.GetTempPath(), "upload");
            var resource = new ApiFileResource
            {
                PathToLocalDirectory = rootDirectory,
                Name = Path.Combine(rootDirectory, "file.txt")
            };
            var handler = new ApiFileHandler(null, null);

            Assert.ThrowsAsync<IOException>(() => handler.DeployFileAsync(resource));
        }
    }
}