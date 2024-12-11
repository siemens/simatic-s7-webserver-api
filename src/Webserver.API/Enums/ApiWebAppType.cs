// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// ApiWebAppType - "origin" of the application
    /// </summary>
    public enum ApiWebAppType
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// User-Created Web Application
        /// </summary>
        User = 1,
        /// <summary>
        /// View-Of-Things Web Application (=>restricted access possible - no download,...)
        /// </summary>
        VoT = 2,
        /// <summary>
        /// System built in Web Application
        /// </summary>
        system_builtin = 3
    }
}
