// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;

namespace Siemens.Simatic.S7.Webserver.API.Services.Deployer
{
    public interface IApiWebAppResourceBuilder
    {
        ApiWebAppResource BuildResourceFromFile(string filePath, string webAppDirectoryPath, ApiWebAppResourceVisibility resourceVisibility);
    }
}