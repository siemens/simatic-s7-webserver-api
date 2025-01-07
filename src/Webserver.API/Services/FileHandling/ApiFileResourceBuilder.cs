// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.IO;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// used to create an ApiFileResource from a given filename and localDirectoryPath
    /// </summary>
    public class ApiFileResourceBuilder : IApiFileResourceBuilder
    {
        private void CheckResourcePath(string resourcePath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(resourcePath);
            FileInfo fileInfo = new FileInfo(resourcePath);
            if (dirInfo.Exists)
            {
                if (fileInfo.Exists)
                {
                    throw new InvalidOperationException($"Fileinfo {fileInfo.FullName} and dirInfo: {dirInfo.FullName} both exist (!)");
                }
            }
            else
            {
                if (!fileInfo.Exists)
                {
                    throw new ArgumentException($"at {resourcePath} no file/directory has been found");
                }
            }
        }

        /// <summary>
        /// used to create an ApiFileResource from a given filename and localDirectoryPath
        /// </summary>
        /// <param name="resourcePath">Path to the File/Directory to build the resource from</param>
        /// <returns>the ApiFileResource</returns>
        public ApiFileResource BuildResourceFromFile(string resourcePath)
        {
            CheckResourcePath(resourcePath);
            DirectoryInfo dirInfo = new DirectoryInfo(resourcePath);
            FileInfo fileInfo = new FileInfo(resourcePath);
            ApiFileResource resource = new ApiFileResource();
            resource.State = Enums.ApiFileResourceState.Active;
            if (dirInfo.Exists)
            {
                resource.PathToLocalDirectory = resourcePath;
                resource.Type = Enums.ApiFileResourceType.Dir;
                resource.Last_Modified = Directory.GetLastWriteTime(resourcePath);
                resource.Name = dirInfo.Name;
            }
            else if (fileInfo.Exists)
            {
                resource.Type = Enums.ApiFileResourceType.File;
                resource.Size = fileInfo.Length;
                resource.PathToLocalDirectory = fileInfo.Directory.FullName;
                resource.Name = fileInfo.Name;
                resource.Last_Modified = File.GetLastWriteTime(resourcePath);
            }
            return resource;
        }

        //if (fileInfo.Directory.FullName != localDirectoryPath)
        //{
        //    // maybe this should never be the case - throw an Exception for now
        //    //resource.Name = fileInfo.Directory.FullName.Substring(localDirectoryPath.Length + 1).Replace("\\", "/") + "/" + fileInfo.Name;
        //    throw new NotImplementedException("didnt expect to land here!");
        //}
        //else
        //{
        //    resource.Name = fileInfo.Name;
        //}

    }
}
