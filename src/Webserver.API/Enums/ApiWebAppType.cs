// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        VoT = 2
    }
}
