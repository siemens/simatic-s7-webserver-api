// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Settings to be used when saving an ApiWebAppData
    /// </summary>
    public interface IApiWebAppDataSaveSetting
    {
        /// <summary>
        /// Defaults to "true" - will be used to control, wether the "basic" checks for the webapp(data) are performed 
        /// so that it is "saveable"
        /// </summary>
        bool CheckConsistency { get; set; }
        /// <summary>
        /// Defaults to "WebAppConfig" - will be the name of the .json File created by ApiWebAppDataSaver
        /// </summary>
        string ConfigurationName { get; set; }
        /// <summary>
        /// Defaults to "true" - will be used to determine wether a directory will be created for saving in case 
        /// it does not yet exist and is provided via DirectoryPath
        /// </summary>
        bool CreateDirectoryIfNotExists { get; set; }
        /// <summary>
        /// Defaults to null and if this value is null => ApiWebAppData.PathToWebAppDirectory
        /// </summary>
        string DirectoryPath { get; set; }
        /// <summary>
        /// JsonSerializerSetting defaults to 
        /// NullValueHandling = NullValueHandling.Ignore,
        /// ContractResolver = new CamelCasePropertyNamesContractResolver()
        /// </summary>
        JsonSerializerSettings JsonSerializerSetting { get; set; }
    }
}