// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MITusing Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Used to Build a Directory that contains the WebApp Configuration (Html Files) e.g. a user wants to have on the plc
    /// </summary>
    public interface IApiDirectoryBuilder
    {
        /// <summary>
        /// Important to set the PathToLocalDirectory in case you want to use a deployer for example!(since the resources have to be built by it)
        /// </summary>
        string PathToLocalDirectory { get; set; }
        /// <summary>
        /// the parser will set the bool IgnoreBOMDifference for all resources parsed to this value!
        /// </summary>
        bool IgnoreBOMDifference { get; set; }

        /// <summary>
        /// Build the local directory
        /// </summary>
        /// <param name="parseConfiguration">Configuration for parsing a local directory</param>
        /// <returns>ApiFileResource containing the informations for:
        /// ApiWebAppState, Name, Type, Resources (parsed from the directory) with their Properties
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the ParserConfiguration (you can also Ignore files or folders according to ParserConfiguration)
        /// </returns>
        ApiFileResource Build(ApiDirectoryBuilderConfiguration parseConfiguration);
        /// <summary>
        /// Build the local directory
        /// </summary>
        /// <param name="configFilePath">filePath of the ApiDirectoryBuilderConfiguration configfile</param>
        /// <returns>ApiFileResource containing the informations for:
        /// ApiWebAppState, Name, Type, Resources (parsed from the directory) with their Properties
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the ParserConfiguration (you can also Ignore files or folders according to ParserConfiguration)
        /// </returns>
        ApiFileResource Build(string configFilePath);
        /// <summary>
        /// In Case you want to use this function -
        /// will recursively go through all subdirectories of the given Directory, create resources from files depending on the webappconfig (you can also Ignore files or folders according to ApiWebAppData)
        /// </summary>
        /// <param name="resource">the root resource underneath which all the sub-resources will be loaded</param>
        /// <param name="parseConfiguration">the parse configuration (to know which sub-resources have to be ignored etc.)</param>
        /// <returns></returns>
        List<ApiFileResource> RecursiveGetResources(ApiFileResource resource, ApiDirectoryBuilderConfiguration parseConfiguration);
    }
}