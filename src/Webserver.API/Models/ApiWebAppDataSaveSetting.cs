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
    public class ApiWebAppDataSaveSetting : IApiWebAppDataSaveSetting
    {
        /// <summary>
        /// Defaults to null and if this value is null => ApiWebAppData.PathToWebAppDirectory
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Defaults to "WebAppConfig"
        /// </summary>
        public string ConfigurationName { get; set; }
        /// <summary>
        /// Defaults to "true"
        /// </summary>
        public bool CheckConsistency { get; set; }

        /// <summary>
        /// Defaults to "true"
        /// </summary>
        public bool CreateDirectoryIfNotExists { get; set; }

        /// <summary>
        /// JsonSerializerSetting defaults to 
        /// NullValueHandling = NullValueHandling.Ignore,
        /// ContractResolver = new CamelCasePropertyNamesContractResolver()
        /// </summary>
        public JsonSerializerSettings JsonSerializerSetting { get; set; }

        /// <summary>
        /// Default c'tor for ApiWebAppDataSaveSetting
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
