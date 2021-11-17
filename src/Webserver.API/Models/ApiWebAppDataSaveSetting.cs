// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Settings to be used when saving an ApiWebAppData
    /// </summary>
    public class ApiWebAppDataSaveSetting
    {
        /// <summary>
        /// Defaults to null and if this value is null => ApiWebAppData.PathToWebAppDirectory
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Defaults to "WebAppConfig" - will be the name of the .json File created by ApiWebAppDataSaver
        /// </summary>
        public string ConfigurationName { get; set; }
        
        /// <summary>
        /// Defaults to "true" - will be used to control, wether the "basic" checks for the webapp(data) are performed 
        /// so that it is "saveable"
        /// </summary>
        public bool CheckConsistency { get; set; }

        /// <summary>
        /// Defaults to "true" - will be used to determine wether a directory will be created for saving in case 
        /// it does not yet exist and is provided via DirectoryPath
        /// </summary>
        public bool CreateDirectoryIfNotExists { get; set; }

        /// <summary>
        /// JsonSerializerSetting defaults to 
        /// NullValueHandling = NullValueHandling.Ignore,
        /// ContractResolver = new CamelCasePropertyNamesContractResolver()
        /// </summary>
        public JsonSerializerSettings JsonSerializerSetting { get; set; }

        /// <summary>
        /// Default c'tor for ApiWebAppDataSaveSetting - Check properties' summary for defaults.
        /// </summary>
        public ApiWebAppDataSaveSetting()
        {
            DirectoryPath = null;
            ConfigurationName = "WebAppConfig";
            CheckConsistency = true;
            CreateDirectoryIfNotExists = true;
            JsonSerializerSetting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}
