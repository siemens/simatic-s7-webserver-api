// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Extensions
{
    /// <summary>
    /// Extensions on the ApiWebAppData that only some might need
    /// </summary>
    public static class ApiWebAppDataExtensions
    {
        /// <summary>
        /// Save the ApiWebAppData (as json) to the given Directory - if none is given: use the webApp.PathToWebAppDirectory - save as $"{configurationName}.json"
        /// Will override an existing configuration file!
        /// </summary>
        /// <param name="webApp">the ApiWebAppData that should be saved</param>
        /// <param name="saveSetting">Settings to apply - dont provide anything if you want to use default settings:
        /// DirectoryPath: null => using webApp.PathToWebAppDirectory
        /// ConfigurationName (fileName): "WebAppConfig.json"
        /// CheckConsistency: true
        /// </param>
        public static void Save(this ApiWebAppData webApp, ApiWebAppDataSaveSetting saveSetting = null)
        {
            ApiWebAppDataSaveSetting settingsToApply = saveSetting ?? new ApiWebAppDataSaveSetting();
            if (settingsToApply.CheckConsistency)
            {
                CheckConsistency_Saveable(webApp);
            }
            string dirToSaveTo = webApp.PathToWebAppDirectory;
            if (settingsToApply.DirectoryPath != null)
            {
                dirToSaveTo = settingsToApply.DirectoryPath;
            }
            if (!Directory.Exists(dirToSaveTo))
            {
                throw new DirectoryNotFoundException($"the given directory at {Environment.NewLine}{dirToSaveTo}{Environment.NewLine} has not been found!");
            }
            
            var configString = JsonConvert.SerializeObject(webApp,
                        new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
            string fileNameToSave = settingsToApply.ConfigurationName.EndsWith(".json") ? settingsToApply.ConfigurationName : settingsToApply.ConfigurationName + ".json";
            using (StreamWriter sw = File.CreateText(Path.Combine(dirToSaveTo, fileNameToSave)))
            {
                sw.Write(configString);
            }
        }

        /// <summary>
        /// Will check the wether the webApp is saveable or not
        /// </summary>
        /// <param name="webApp">the ApiWebAppData that should be checked</param>
        public static void CheckConsistency_Saveable(this ApiWebAppData webApp)
        {
            CheckConsistency_Minimum(webApp);
            if(string.IsNullOrEmpty(webApp.PathToWebAppDirectory))
            {
                throw new ApiInconsistentApiWebAppDataException("the path to the webappdirectory of the WebApp is null or empty!");
            }
            if(!Directory.Exists(webApp.PathToWebAppDirectory))
            {
                throw new DirectoryNotFoundException($"the directory at {Environment.NewLine}{webApp.PathToWebAppDirectory}{Environment.NewLine}was not found!");
            }
        }

        /// <summary>
        /// Will check the webApp minimum consistency => so that it can be created / is a "valid app on the plc"
        /// </summary>
        /// <param name="webApp">the ApiWebAppData that should be checked</param>
        public static void CheckConsistency_Minimum(this ApiWebAppData webApp)
        {
            if(string.IsNullOrEmpty(webApp.Name))
            {
                throw new ApiInconsistentApiWebAppDataException("the name of the WebApp is null or empty!");
            }
            if (webApp.Type == Enums.ApiWebAppType.None)
            {
                throw new ApiInconsistentApiWebAppDataException("the type of the WebApp has not been set!");
            }
            if(webApp.State == Enums.ApiWebAppState.None)
            {
                throw new ApiInconsistentApiWebAppDataException("the state of the WebApp has not been set!");
            }
        }
    }
}
