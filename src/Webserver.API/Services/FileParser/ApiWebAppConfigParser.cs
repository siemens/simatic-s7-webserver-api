// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileParser
{
    /// <summary>
    /// Used to Build a Directory that contains the WebApp Configuration (Html Files) e.g. a user wants to have on the plc
    /// </summary>
    public class ApiWebAppConfigParser
    {
        /// <summary>
        /// Important to set the PathToWebAppDirectory in case you want to use a deployer for example!(since the resources have to be built by it)
        /// </summary>
        public string PathToWebAppDirectory;

        /// <summary>
        /// The Config File Name content needs to be according to the ApiWebAppData class (filling it)!
        /// </summary>
        public string WebAppConfigFileName;

        /// <summary>
        /// the parser will set the bool IgnoreBOMDifference for all resources parsed to this value!
        /// </summary>
        public bool IgnoreBOMDifference { get; set; }

        private readonly IApiWebAppResourceBuilder ApiWebAppResourceBuilder;

        /// <summary>
        /// Constructing the class will not yet Build the ApiWebAppDatat - that'll happen once you call the Function Build() on the instance so a user can also adjust the paths and reuse an instance if he wants to
        /// </summary>
        /// <param name="pathToWebAppDirectory">The Config File Name content needs to be according to the ApiWebAppData class (filling it)!</param>
        /// <param name="webAppConfigFileName">Important to set the PathToWebAppDirectory in case you want to use a deployer for example!(since the resources have to be built by it)</param>
        /// <param name="webAppResourceBuilder">resource builder for resource handler, deployer...</param>
        /// <param name="ignoreBOMDifference"> a boolean that will be set for every resource that will be parsed! For details look at ApiwebAppResources ignoreBOMDifference
        /// for no value given IgnoreBOMDifference will default to false</param>
        public ApiWebAppConfigParser(string pathToWebAppDirectory, string webAppConfigFileName, IApiWebAppResourceBuilder webAppResourceBuilder, bool ignoreBOMDifference = false)
        {
            this.PathToWebAppDirectory = pathToWebAppDirectory ?? throw new ArgumentNullException(nameof(pathToWebAppDirectory));
            this.WebAppConfigFileName = webAppConfigFileName ?? throw new ArgumentNullException(nameof(webAppConfigFileName));
            this.ApiWebAppResourceBuilder = webAppResourceBuilder ?? throw new ArgumentNullException(nameof(webAppResourceBuilder));
            this.IgnoreBOMDifference = ignoreBOMDifference;
        }


        /// <summary>
        /// Build the WebAppDirectory with the WebAppConfigFile that are currently set!
        /// </summary>
        /// <returns>ApiWebAppData containing the informations for:
        /// ApiWebAppState, Name, Type, DefaultPage, NotAuthorizedPage, NotFoundPage, ApplicationResources (parsed from the directory) with their Properties
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the webappconfig (you can also Ignore files or folders according to ApiWebAppData)
        /// </returns>
        public ApiWebAppData Parse()
        {
            string configFilePath = Path.Combine(PathToWebAppDirectory, WebAppConfigFileName);
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException($"Webapp config file at {configFilePath} not found!");
            }
            string configFile = File.ReadAllText(configFilePath);
            try
            {
                ApiWebAppData webApp = JsonConvert.DeserializeObject<ApiWebAppData>(configFile);
                webApp.PathToWebAppDirectory = PathToWebAppDirectory;
                if (webApp.FileExtensionsToIgnoreForUpload == null)
                {
                    webApp.FileExtensionsToIgnoreForUpload = new List<string>();
                }
                if (webApp.DirectoriesToIgnoreForUpload == null)
                {
                    webApp.DirectoriesToIgnoreForUpload = new List<string>();
                }
                if (webApp.ResourcesToIgnoreForUpload == null)
                {
                    webApp.ResourcesToIgnoreForUpload = new List<string>();
                }
                if (webApp.ProtectedResources == null)
                {
                    webApp.ProtectedResources = new List<string>();
                }
                // get resources in Directory
                webApp.ApplicationResources = RecursiveGetResources(PathToWebAppDirectory, webApp);
                return webApp;
            }
            catch(Newtonsoft.Json.JsonSerializationException serializationException)
            {
                if(serializationException.Message.Contains("Error setting value to 'State'"))
                {
                    throw new ApiWebAppConfigParserException("Missing parameter 'State' or State was invalid => 'None' or 0 ", serializationException);
                }
                if(serializationException.Message.Contains("Error setting value to 'Type'"))
                {
                    throw new ApiWebAppConfigParserException("Missing parameter 'Type' or Type was invalid => 'None' or 0 ", serializationException);
                }
                throw serializationException;
            }
        }

        /// <summary>
        /// In Case you want to use this function -
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the webappconfig (you can also Ignore files or folders according to ApiWebAppData)
        /// </summary>
        /// <param name="pathToDir"></param>
        /// <param name="webApp"></param>
        /// <returns></returns>
        public List<ApiWebAppResource> RecursiveGetResources(string pathToDir, ApiWebAppData webApp)
        {
            List<ApiWebAppResource> resources = new List<ApiWebAppResource>();
            var dirsToIgnore = webApp.DirectoriesToIgnoreForUpload ?? new List<string>();
            var resToIgnore = webApp.ResourcesToIgnoreForUpload ?? new List<string>();
            var fileExtToIgnore = webApp.FileExtensionsToIgnoreForUpload ?? new List<string>();
            var protRes = webApp.ProtectedResources ?? new List<string>();
            foreach (string dir in Directory.GetDirectories(pathToDir))
            {
                // reads a bit weird but translated: if directory name matches any of the directoriestoignore => ignore it - do nothing with that element (=> else)
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                bool directoryIsNotToBeIgnored = !(dirsToIgnore.Any(ign => dirInfo.Name == ign));
                if (directoryIsNotToBeIgnored)
                {
                    foreach (ApiWebAppResource r in RecursiveGetResources(dir, webApp))
                    {
                        resources.Add(r);
                    }
                }
            }
            foreach (string f in Directory.GetFiles(pathToDir))
            {
                FileInfo fileInfo = new FileInfo(f);
                // ignore spec. file extensions (.git,...)
                bool fileExtensionIsNotToBeIgnored = !(fileExtToIgnore.Contains(fileInfo.Extension));
                //ignore config file by default!
                bool fileIsNotConfigurationFile = !(fileInfo.FullName == pathToDir + @"\" + WebAppConfigFileName);
                if (fileExtensionIsNotToBeIgnored && fileIsNotConfigurationFile)
                {
                    var res = ApiWebAppResourceBuilder.BuildResourceFromFile(f, PathToWebAppDirectory, ApiWebAppResourceVisibility.Public);
                    bool resourceIsNotToBeIgnored = !resToIgnore.Contains(res.Name);
                    // if resource name is one to be ignored => dont do anything with that element.
                    if (resourceIsNotToBeIgnored)
                    {
                        if (protRes.Contains(res.Name))
                        {
                            res.Visibility = ApiWebAppResourceVisibility.Protected;
                        }
                        res.IgnoreBOMDifference = this.IgnoreBOMDifference;
                        resources.Add(res);
                    }
                }
            }
            return resources;
        }
    }
}
