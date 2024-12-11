// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// used to create an ApiFileResource from a given filename and localDirectoryPath
    /// </summary>
    public interface IApiFileResourceBuilder
    {
        /// <summary>
        /// used to create an ApiFileResource from a given filename and localDirectoryPath
        /// </summary>
        /// <param name="resourcePath">Path to the File/Directory to build the resource from</param>
        /// <returns>the ApiFileResource</returns>
        ApiFileResource BuildResourceFromFile(string resourcePath);

        ///// <param name="localDirectoryPath">Local Directory Path - used to determine the filename (Path!)</param>
    }
}