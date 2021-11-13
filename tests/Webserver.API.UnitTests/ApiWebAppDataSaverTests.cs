// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    class ApiWebAppDataSaverTests : Base
    {
        [Test]
        public void SaveThrowsIfDirectoryDoesNotExistForSetting()
        {
            
        }

        [Test]
        public void ApiWebAppDataSave()
        {
            string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tmp");
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
                // later change of savesetting has impact
                saveSetting.CreateDirectoryIfNotExists = false;
                Assert.Throws<DirectoryNotFoundException>(() => saver.Save(testApp));
                testApp.PathToWebAppDirectory = dirPath;
                // works again
                saver.Save(testApp);
                Assert.That(File.Exists(Path.Combine(testApp.PathToWebAppDirectory, saveSetting.ConfigurationName + ".json")));
            }
            finally
            {
                Directory.Delete(dirPath, true);
            }
        }
    }
}
