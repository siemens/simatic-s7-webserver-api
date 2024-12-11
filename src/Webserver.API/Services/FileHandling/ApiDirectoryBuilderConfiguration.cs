// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Configuration for parsing a local directory
    /// </summary>
    public class ApiDirectoryBuilderConfiguration
    {
        /// <summary>
        /// DirectoriesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources inside that directory to be uploaded (not if directoryname is added)
        /// </summary>
        public List<string> DirectoriesToIgnoreForUpload;
        /// <summary>
        /// ResourcesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resourcename is added)
        /// </summary>
        public List<string> ResourcesToIgnoreForUpload;
        /// <summary>
        /// FileExtensionsToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resource fileextension is added)
        /// </summary>
        public List<string> FileExtensionsToIgnoreForUpload;
    }
}
