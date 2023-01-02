// Copyright (c) 2023, Siemens AG
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
    /// Resourcevisibility => determines access rights needed for resource
    /// </summary>
    public enum ApiWebAppResourceVisibility
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// The User does not need Permissions to "Access User-Defined Pages"
        /// </summary>
        Public = 1,
        /// <summary>
        /// The User needs Permissions to "Access User-Defined Pages" to find the resource on the webapp.
        /// </summary>
        Protected = 2
    }
}
