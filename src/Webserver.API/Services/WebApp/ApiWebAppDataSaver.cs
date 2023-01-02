// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// Implementation to comfortly check for the savesettings and save the given ApiWebAppData
    /// </summary>
    public class ApiWebAppDataSaver
    {
        private readonly ApiWebAppDataSaveSetting ApiWebAppSaveSetting;

        /// <summary>
        /// Create ApiWebAppDataSaver with the apiWebAppSaveSetting
        /// </summary>
        /// <param name="apiWebAppSaveSetting">Settings to apply</param>
        public ApiWebAppDataSaver(ApiWebAppDataSaveSetting apiWebAppSaveSetting)
        {
            ApiWebAppSaveSetting = apiWebAppSaveSetting ?? throw new ArgumentNullException(nameof(apiWebAppSaveSetting));
        }

        /// <summary>
        /// Save the ApiWebAppData (as json) to the given Directory - if none is given: use the webApp.PathToWebAppDirectory - save as $"{configurationName}.json"
        /// Will override an existing configuration file!
        /// </summary>
        /// <param name="apiWebApp">the ApiWebAppData that should be saved</param>
        public void Save(IApiWebAppData apiWebApp)
        {
            if(apiWebApp == null)
            {
                throw new ArgumentNullException(nameof(apiWebApp));
            }
            if (ApiWebAppSaveSetting.CheckConsistency)
            {
                CheckConsistency_Saveable(apiWebApp);
            }
            string dirToSaveTo = apiWebApp.PathToWebAppDirectory;
            if (ApiWebAppSaveSetting.DirectoryPath != null)
            {
                dirToSaveTo = ApiWebAppSaveSetting.DirectoryPath;
            }
            if (!Directory.Exists(dirToSaveTo))
            {
                if(!ApiWebAppSaveSetting.CreateDirectoryIfNotExists)
                {
                    throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{dirToSaveTo}{Environment.NewLine} has not been found!");
                }
                else
                {
                    Directory.CreateDirectory(dirToSaveTo);
                }
            }
            var configString = JsonConvert.SerializeObject(apiWebApp,
                        ApiWebAppSaveSetting.JsonSerializerSetting);
            string fileNameToSave = ApiWebAppSaveSetting.ConfigurationName.EndsWith(".json") ? 
                ApiWebAppSaveSetting.ConfigurationName : ApiWebAppSaveSetting.ConfigurationName + ".json";
            using (StreamWriter sw = File.CreateText(Path.Combine(dirToSaveTo, fileNameToSave)))
            {
                sw.Write(configString);
            }
        }

        /// <summary>
        /// Will check the wether the webApp is saveable or not
        /// </summary>
        /// <param name="apiWebApp">the ApiWebAppData that should be checked</param>
        public void CheckConsistency_Saveable(IApiWebAppData apiWebApp)
        {
            if (apiWebApp == null)
            {
                throw new ArgumentNullException(nameof(apiWebApp));
            }
            CheckConsistency_Minimum(apiWebApp);
            if (string.IsNullOrEmpty(apiWebApp.PathToWebAppDirectory))
            {
                throw new ApiInconsistentApiWebAppDataException("the path to the webappdirectory of the WebApp is null or empty!");
            }
            if (!Directory.Exists(apiWebApp.PathToWebAppDirectory) && !ApiWebAppSaveSetting.CreateDirectoryIfNotExists)
            {
                throw new DirectoryNotFoundException($"the directory at {Environment.NewLine}{apiWebApp.PathToWebAppDirectory}{Environment.NewLine}was not found!");
            }
        }

        /// <summary>
        /// Will check the webApp minimum consistency => so that it can be created / is a "valid app on the plc"
        /// </summary>
        /// <param name="apiWebApp">the ApiWebAppData that should be checked</param>
        public void CheckConsistency_Minimum(IApiWebAppData apiWebApp)
        {
            if (apiWebApp == null)
            {
                throw new ArgumentNullException(nameof(apiWebApp));
            }
            if (string.IsNullOrEmpty(apiWebApp.Name))
            {
                throw new ApiInconsistentApiWebAppDataException("the name of the WebApp is null or empty!");
            }
            if (apiWebApp.Type == Enums.ApiWebAppType.None)
            {
                throw new ApiInconsistentApiWebAppDataException("the type of the WebApp has not been set!");
            }
            if (apiWebApp.State == Enums.ApiWebAppState.None)
            {
                throw new ApiInconsistentApiWebAppDataException("the state of the WebApp has not been set!");
            }
        }
    }
}
