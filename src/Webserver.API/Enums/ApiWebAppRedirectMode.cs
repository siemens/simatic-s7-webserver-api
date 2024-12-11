// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible Application Redirect Modes
    /// </summary>
    public enum ApiWebAppRedirectMode
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// Redirect mode
        /// </summary>
        Redirect = 1,
        /// <summary>
        /// Forward mode
        /// </summary>
        Forward = 2
    }
}
