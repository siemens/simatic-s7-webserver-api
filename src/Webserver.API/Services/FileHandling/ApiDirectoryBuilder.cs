// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MITusing Newtonsoft.Json;
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.FileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Used to Build a Directory that contains the WebApp Configuration (Html Files) e.g. a user wants to have on the plc
    /// </summary>
    public class ApiDirectoryBuilder : IApiDirectoryBuilder
    {
        /// <summary>
        /// Important to set the PathToLocalDirectory in case you want to use a deployer for example!(since the resources have to be built by it)
        /// </summary>
        public string PathToLocalDirectory { get; set; }

        /// <summary>
        /// the parser will set the bool IgnoreBOMDifference for all resources parsed to this value!
        /// </summary>
        public bool IgnoreBOMDifference { get; set; }

        private readonly IApiFileResourceBuilder ApiFileResourceBuilder;

        /// <summary>
        /// Constructing the class will not yet Build the ApiWebAppDatat - that'll happen once you call the Function Build() on the instance so a user can also adjust the paths and reuse an instance if he wants to
        /// </summary>
        /// <param name="pathToLocalDirectory">The Config File Name content needs to be according to the ApiWebAppData class (filling it)!</param>
        /// <param name="resourceBuilder">resource builder for resource handler, deployer...</param>
        /// <param name="ignoreBOMDifference"> a boolean that will be set for every resource that will be parsed! For details look at ApiFileResource ignoreBOMDifference
        /// for no value given IgnoreBOMDifference will default to false</param>
        public ApiDirectoryBuilder(string pathToLocalDirectory, IApiFileResourceBuilder resourceBuilder, bool ignoreBOMDifference = false)
        {
            this.PathToLocalDirectory = pathToLocalDirectory ?? throw new ArgumentNullException(nameof(pathToLocalDirectory));
            this.ApiFileResourceBuilder = resourceBuilder ?? throw new ArgumentNullException(nameof(resourceBuilder));
            this.IgnoreBOMDifference = ignoreBOMDifference;
        }

        /// <summary>
        /// Build the local directory
        /// </summary>
        /// <param name="configFilePath">filePath of the ApiDirectoryBuilderConfiguration configfile</param>
        /// <returns>ApiFileResource containing the informations for:
        /// ApiWebAppState, Name, Type, Resources (parsed from the directory) with their Properties
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the ParserConfiguration (you can also Ignore files or folders according to ParserConfiguration)
        /// </returns>
        public ApiFileResource Build(string configFilePath)
        {
            if(!File.Exists(configFilePath))
            {
                throw new FileNotFoundException(configFilePath);
            }
            string configFile = File.ReadAllText(configFilePath);
            var configuration = JsonConvert.DeserializeObject<ApiDirectoryBuilderConfiguration>(configFile);
            return Build(configuration);
        }

        /// <summary>
        /// Build the local directory
        /// </summary>
        /// <param name="parseConfiguration">Configuration for parsing a local directory</param>
        /// <returns>ApiFileResource containing the informations for:
        /// ApiWebAppState, Name, Type, Resources (parsed from the directory) with their Properties
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the ParserConfiguration (you can also Ignore files or folders according to ParserConfiguration)
        /// </returns>
        public ApiFileResource Build(ApiDirectoryBuilderConfiguration parseConfiguration)
        {
            if(!Directory.Exists(PathToLocalDirectory))
            {
                throw new DirectoryNotFoundException(PathToLocalDirectory);
            }
            var parseConfigurationToUse = parseConfiguration ?? new ApiDirectoryBuilderConfiguration();
            if (parseConfigurationToUse.FileExtensionsToIgnoreForUpload == null)
            {
                parseConfigurationToUse.FileExtensionsToIgnoreForUpload = new List<string>();
            }
            if (parseConfigurationToUse.DirectoriesToIgnoreForUpload == null)
            {
                parseConfigurationToUse.DirectoriesToIgnoreForUpload = new List<string>();
            }
            if (parseConfigurationToUse.ResourcesToIgnoreForUpload == null)
            {
                parseConfigurationToUse.ResourcesToIgnoreForUpload = new List<string>();
            }
            try
            {
                var dirInf = new DirectoryInfo(PathToLocalDirectory);
                ApiFileResource resource = new ApiFileResource()
                {
                    PathToLocalDirectory = PathToLocalDirectory,
                    State = Enums.ApiFileResourceState.Active,
                    Type = Enums.ApiFileResourceType.Dir, 
                    Last_Modified = dirInf.LastWriteTime,
                    Name = dirInf.Name,
                };
                
                resource.Parents = new List<ApiFileResource>();

                // get resources in Directory
                resource.Resources = RecursiveGetResources(resource, parseConfigurationToUse);
                resource.Resources = resource.Resources.OrderBy(el => el.Type).ThenBy(el => el.Size).ToList();
                return resource;
            }
            catch (Newtonsoft.Json.JsonSerializationException serializationException)
            {
                if (serializationException.Message.Contains("Error setting value to 'State'"))
                {
                    throw new ApiDirectoryParserException("Missing parameter 'State' or State was invalid => 'None' or 0 ", serializationException);
                }
                if (serializationException.Message.Contains("Error setting value to 'Type'"))
                {
                    throw new ApiDirectoryParserException("Missing parameter 'Type' or Type was invalid => 'None' or 0 ", serializationException);
                }
                throw serializationException;
            }
        }

        /// <summary>
        /// In Case you want to use this function -
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the webappconfig (you can also Ignore files or folders according to ApiWebAppData)
        /// </summary>
        /// <param name="resource">the root resource underneath which all the sub-resources will be loaded</param>
        /// <param name="parseConfiguration">the parse configuration (to know which sub-resources have to be ignored etc.)</param>
        /// <returns></returns>
        public List<ApiFileResource> RecursiveGetResources(ApiFileResource resource, ApiDirectoryBuilderConfiguration parseConfiguration)
        {
            List<ApiFileResource> resources = new List<ApiFileResource>();
            var dirsToIgnore = parseConfiguration.DirectoriesToIgnoreForUpload ?? new List<string>();
            var resToIgnore = parseConfiguration.ResourcesToIgnoreForUpload ?? new List<string>();
            var fileExtToIgnore = parseConfiguration.FileExtensionsToIgnoreForUpload ?? new List<string>();
            foreach (string dir in Directory.GetDirectories(resource.PathToLocalDirectory))
            {
                // reads a bit weird but translated: if directory name matches any of the directoriestoignore => ignore it - do nothing with that element (=> else)
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                bool directoryIsNotToBeIgnored = !(dirsToIgnore.Any(ign => dirInfo.Name == ign));
                if (directoryIsNotToBeIgnored)
                {
                    var subRes = ApiFileResourceBuilder.BuildResourceFromFile(dirInfo.FullName);
                    subRes.Resources = new List<ApiFileResource>();
                    subRes.Parents = new List<ApiFileResource>();
                    foreach (var parent in resource.Parents)
                    {
                        subRes.Parents.Add(parent);
                    }
                    subRes.Parents.Add(resource);
                    resources.Add(subRes);
                    foreach (ApiFileResource r in RecursiveGetResources(subRes, parseConfiguration))
                    {
                        subRes.Resources.Add(r);
                    }
                }
            }
            foreach (string f in Directory.GetFiles(resource.PathToLocalDirectory))
            {
                FileInfo fileInfo = new FileInfo(f);
                // ignore spec. file extensions (.git,...)
                bool fileExtensionIsNotToBeIgnored = !(fileExtToIgnore.Contains(fileInfo.Extension));
                if (fileExtensionIsNotToBeIgnored)
                {
                    var res = ApiFileResourceBuilder.BuildResourceFromFile(f);
                    bool resourceIsNotToBeIgnored = !resToIgnore.Contains(res.Name);
                    // if resource name is one to be ignored => dont do anything with that element.
                    if (resourceIsNotToBeIgnored)
                    {
                        res.Parents = new List<ApiFileResource>();
                        foreach (var parent in resource.Parents)
                        {
                            res.Parents.Add(parent);
                        }
                        res.Parents.Add(resource);
                        resources.Add(res);
                    }
                }
            }
            return resources;
        }
    }
}
