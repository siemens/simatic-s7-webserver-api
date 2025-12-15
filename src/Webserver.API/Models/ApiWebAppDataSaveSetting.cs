// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        public readonly string DirectoryPath;
        /// <summary>
        /// Defaults to "WebAppConfig" - will be the name of the .json File created by ApiWebAppDataSaver
        /// </summary>
        public readonly string ConfigurationName;

        /// <summary>
        /// Defaults to "true" - will be used to control, whether the "basic" checks for the webapp(data) are performed 
        /// so that it is "saveable"
        /// </summary>
        public readonly bool CheckConsistency;

        /// <summary>
        /// Defaults to "true" - will be used to determine whether a directory will be created for saving in case 
        /// it does not yet exist and is provided via DirectoryPath
        /// </summary>
        public readonly bool CreateDirectoryIfNotExists;

        /// <summary>
        /// JsonSerializerSetting defaults to 
        /// NullValueHandling = NullValueHandling.Ignore,
        /// ContractResolver = new CamelCasePropertyNamesContractResolver()
        /// </summary>
        public readonly JsonSerializerSettings JsonSerializerSetting;

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

        /// <summary>
        /// C'tor to configure the savesetting on your own
        /// </summary>
        /// <param name="directoryPath">Path where the ApiWebAppData should be saved to</param>
        /// <param name="configurationName">name for the file to be saved</param>
        /// <param name="checkConsistency">will be used to control, whether the "basic" checks for the webapp(data) are performed </param>
        /// <param name="createDirectoryIfNotExists">will be used to determine whether a directory will be created 
        /// for saving in case it does not yet exist and is provided via DirectoryPath</param>
        /// <param name="jsonSerializerSetting">Setting for the Serialization</param>
        public ApiWebAppDataSaveSetting(string directoryPath, string configurationName,
            bool checkConsistency, bool createDirectoryIfNotExists, JsonSerializerSettings jsonSerializerSetting)
        {
            DirectoryPath = directoryPath;
            ConfigurationName = configurationName;
            CheckConsistency = checkConsistency;
            CreateDirectoryIfNotExists = createDirectoryIfNotExists;
            JsonSerializerSetting = jsonSerializerSetting;
        }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
