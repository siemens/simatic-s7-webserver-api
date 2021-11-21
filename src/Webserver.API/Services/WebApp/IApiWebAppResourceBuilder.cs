// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// ApiWebAppResourceBuilder: used to create a webappresource from a given filename and webappdirectorypath - set resource protection to resourceVisibility given
    /// </summary>
    public interface IApiWebAppResourceBuilder
    {
        /// <summary>
        /// used to create a webappresource from a given filename and webappdirectorypath - set resource protection to resourceVisibility given
        /// => true: protected, false: public
        /// </summary>
        /// <param name="filePath">Path to the File to build the resource from</param>
        /// <param name="webAppDirectoryPath">WebAppDirectory Path - used to determine the filename (Path!)</param>
        /// <param name="resourceVisibility">resource.Visibility will be set to value given!</param>
        /// <returns></returns>
        ApiWebAppResource BuildResourceFromFile(string filePath, string webAppDirectoryPath, ApiWebAppResourceVisibility resourceVisibility);
    }
}