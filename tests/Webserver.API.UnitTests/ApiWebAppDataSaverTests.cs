// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.IO;

namespace Webserver.API.UnitTests
{
    class ApiWebAppDataSaverTests : Base
    {
        [Test]
        public void ApiWebAppDataSave()
        {
            string dirPath = Path.Combine(CurrentExeDir.FullName, "tmp");
            try
            {
                Directory.CreateDirectory(dirPath);
                ApiWebAppData testApp = new ApiWebAppData() { Type = ApiWebAppType.User };
                var saveSetting = new ApiWebAppDataSaveSetting();
                var saver = new ApiWebAppDataSaver(saveSetting);
                Assert.Throws<ApiInconsistentApiWebAppDataException>(() => saver.Save(testApp));
                testApp = new ApiWebAppData() { State = ApiWebAppState.Enabled };
                Assert.Throws<ApiInconsistentApiWebAppDataException>(() => saver.Save(testApp));
                testApp.Type = ApiWebAppType.User;
                Assert.Throws<ApiInconsistentApiWebAppDataException>(() => saver.Save(testApp));
                testApp.Name = "validName";
                Assert.Throws<ApiInconsistentApiWebAppDataException>(() => saver.Save(testApp));
                testApp.PathToWebAppDirectory = Path.Combine(dirPath, "createdBySaver");
                // works
                saver.Save(testApp);
                Assert.That(Directory.Exists(testApp.PathToWebAppDirectory));
                Assert.That(File.Exists(Path.Combine(testApp.PathToWebAppDirectory, saveSetting.ConfigurationName + ".json")));
                Directory.Delete(testApp.PathToWebAppDirectory, true);
                var jsonSerSetting = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                // new savesetting to not save directory if it does not exist will throw exception
                saveSetting = new ApiWebAppDataSaveSetting(null, "WebAppConfig", true, false, jsonSerSetting);
                saver = new ApiWebAppDataSaver(saveSetting);
                Assert.Throws<DirectoryNotFoundException>(() => saver.Save(testApp));
                testApp.PathToWebAppDirectory = dirPath;
                // works again if the directory does already exist!
                saver.Save(testApp);
                Assert.That(File.Exists(Path.Combine(testApp.PathToWebAppDirectory, saveSetting.ConfigurationName + ".json")));
            }
            finally
            {
                Directory.Delete(dirPath, true);
            }
        }

        [Test]
        public void ApiWebAppDataSave_RejectsTraversalInConfigurationName()
        {
            string dirPath = Path.Combine(CurrentExeDir.FullName, "tmp", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dirPath);

            var testApp = new ApiWebAppData()
            {
                Name = "validName",
                Type = ApiWebAppType.User,
                State = ApiWebAppState.Enabled,
                PathToWebAppDirectory = dirPath
            };

            var saveSetting = new ApiWebAppDataSaveSetting(dirPath, "..\\outside", true, true, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var saver = new ApiWebAppDataSaver(saveSetting);
            var escapedPath = Path.Combine(Directory.GetParent(dirPath).FullName, "outside.json");

            try
            {
                Assert.Throws<IOException>(() => saver.Save(testApp));
                Assert.That(File.Exists(escapedPath), Is.False);
            }
            finally
            {
                if (File.Exists(escapedPath))
                {
                    File.Delete(escapedPath);
                }

                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }
            }
        }

        [Test]
        public void ApiWebAppDataSave_AllowsSimpleConfigurationName()
        {
            string dirPath = Path.Combine(CurrentExeDir.FullName, "tmp", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dirPath);

            var testApp = new ApiWebAppData()
            {
                Name = "validName",
                Type = ApiWebAppType.User,
                State = ApiWebAppState.Enabled,
                PathToWebAppDirectory = dirPath
            };

            var saveSetting = new ApiWebAppDataSaveSetting(dirPath, "CustomConfig", true, true, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var saver = new ApiWebAppDataSaver(saveSetting);
            var expectedPath = Path.Combine(dirPath, "CustomConfig.json");

            try
            {
                saver.Save(testApp);
                Assert.That(File.Exists(expectedPath), Is.True);
            }
            finally
            {
                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }
            }
        }
    }
}
