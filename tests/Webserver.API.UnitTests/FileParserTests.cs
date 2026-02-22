// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.FileParser;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.IO;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class FileParserTests : Base
    {
        [Test]
        public void InvalidApplicationsExceptionThrown()
        {
            string dirPath = Path.Combine(CurrentExeDir.FullName, "tmp");
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                ApiWebAppData missingStateApp = new ApiWebAppData()
                {
                    Name = "validName",
                    Type = ApiWebAppType.User
                };
                string serializedAppString = JsonConvert.SerializeObject(missingStateApp);
                string fileName = "webappconfig.json";
                string filePath = Path.Combine(dirPath, fileName);
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(serializedAppString);
                }
                ;
                ApiWebAppConfigParser parser = new ApiWebAppConfigParser(dirPath, fileName, new ApiWebAppResourceBuilder());
                Assert.Throws<ApiWebAppConfigParserException>(() =>
                {
                    parser.Parse();
                });
                var missingTypeApp = new ApiWebAppData()
                {
                    Name = "validName",
                    State = ApiWebAppState.Disabled
                };
                serializedAppString = JsonConvert.SerializeObject(missingTypeApp);
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(serializedAppString);
                }
                ;
                Assert.Throws<ApiWebAppConfigParserException>(() =>
                {
                    parser.Parse();
                });
            }
            finally
            {
                Directory.Delete(dirPath, true);
            }

        }

        [Test]
        public void ValidApplicationAsExpected()
        {
            string dirPath = Path.Combine(CurrentExeDir.FullName, "tmp");
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var TypeApp = new ApiWebAppData()
                {
                    Name = "validName",
                    State = ApiWebAppState.Disabled,
                    Type = ApiWebAppType.User,
                    Redirect_mode = ApiWebAppRedirectMode.Redirect
                };
                var serializedAppString = JsonConvert.SerializeObject(TypeApp);
                string fileName = "webappconfig.json";
                string filePath = Path.Combine(dirPath, fileName);
                ApiWebAppConfigParser parser = new ApiWebAppConfigParser(dirPath, fileName, new ApiWebAppResourceBuilder());
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(serializedAppString);
                }
                ;
                var app = parser.Parse();
                if (app.PathToWebAppDirectory != dirPath)
                {
                    Assert.Fail($"Path to WebApp Directory not set as expected!:{Environment.NewLine}" +
                        $"{app.PathToWebAppDirectory}");
                }
                if (app.DirectoriesToIgnoreForUpload == null || app.DirectoriesToIgnoreForUpload.Count != 0)
                {
                    Assert.Fail($"DirectoriesToIgnore dont default to empty List!:{Environment.NewLine}" +
                        $"{string.Join(Environment.NewLine, app.DirectoriesToIgnoreForUpload)}");
                }
                if (app.FileExtensionsToIgnoreForUpload == null || app.FileExtensionsToIgnoreForUpload.Count != 0)
                {
                    Assert.Fail($"FileExtensionsToIgnoreForUpload dont default to empty List!:{Environment.NewLine}" +
                        $"{string.Join(Environment.NewLine, app.FileExtensionsToIgnoreForUpload)}");
                }
                if (app.ResourcesToIgnoreForUpload == null || app.ResourcesToIgnoreForUpload.Count != 0)
                {
                    Assert.Fail($"ResourcesToIgnoreForUpload dont default to empty List!:{Environment.NewLine}" +
                        $"{string.Join(Environment.NewLine, app.ResourcesToIgnoreForUpload)}");
                }
                if (app.ProtectedResources == null || app.ProtectedResources.Count != 0)
                {
                    Assert.Fail($"ProtectedResources dont default to empty List!:{Environment.NewLine}" +
                        $"{string.Join(Environment.NewLine, app.ProtectedResources)}");
                }
                if (app.ApplicationResources == null || app.ApplicationResources.Count != 0)
                {
                    Assert.Fail($"ApplicationResources dont default to empty List!:{Environment.NewLine}" +
                        $"null?: {app.ApplicationResources == null}, not null -> count: {app.ApplicationResources?.Count}," +
                        $"{app.ApplicationResources?.First()}");
                }
                if (app.Default_page != null)
                {
                    Assert.Fail($"Defaultpage doesnt default to null!:{Environment.NewLine}" +
                        $"{app.Default_page}");
                }
                if (app.Not_authorized_page != null)
                {
                    Assert.Fail($"Not_authorized_page doesnt default to null!:{Environment.NewLine}" +
                        $"{app.Not_authorized_page}");
                }
                if (app.Not_found_page != null)
                {
                    Assert.Fail($"Not_found_page doesnt default to null!:{Environment.NewLine}" +
                        $"{app.Not_found_page}");
                }
            }
            finally
            {
                Directory.Delete(dirPath, true);
            }
        }
    }
}
