﻿// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    /// <summary>
    /// ApiWebAppResourceBuilder: used to create a webappresource from a given filename and webappdirectorypath - set resource protection to resourceVisibility given
    /// </summary>
    public static class ApiWebAppResourceBuilder
    {
        /// <summary>
        /// used to create a webappresource from a given filename and webappdirectorypath - set resource protection to resourceVisibility given
        /// => true: protected, false: public (as of 2.9)
        /// </summary>
        /// <param name="filePath">Path to the File to build the resource from</param>
        /// <param name="webAppDirectoryPath">WebAppDirectory Path - used to determine the filename (Path!)</param>
        /// <param name="resourceVisibility">resource.Visibility will be set to value given!</param>
        /// <returns></returns>
        public static ApiWebAppResource BuildResourceFromFile(string filePath, string webAppDirectoryPath, ApiWebAppResourceVisibility resourceVisibility)
        {
            if(!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File: at: {filePath} not found!");
            }
            FileInfo fileInfo = new FileInfo(filePath);
            ApiWebAppResource resource = new ApiWebAppResource();
            resource.Size = fileInfo.Length;
            if (fileInfo.Directory.FullName != webAppDirectoryPath)
                resource.Name = fileInfo.Directory.FullName.Substring(webAppDirectoryPath.Length + 1).Replace("\\", "/") + "/" + fileInfo.Name;
            else
                resource.Name = fileInfo.Name;
            resource.Visibility = resourceVisibility;
            resource.Last_modified = File.GetLastWriteTime(filePath);
            resource.Media_type = MimeMapping.MimeUtility.GetMimeMapping(fileInfo.Extension);
            return resource;
        }

    }
}
