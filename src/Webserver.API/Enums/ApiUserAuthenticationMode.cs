// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible user authentication modes
    /// </summary>
    public enum ApiUserAuthenticationMode
    {
        /// <summary>
        /// Anonymous
        /// </summary>
        None = 0,
        /// <summary>
        /// Local User Management
        /// </summary>
        Local = 1,
        /// <summary>
        /// Central User Management (via Username and Password)
        /// </summary>
        Umc = 2,
        /// <summary>
        /// Central User Management (via Single-Sign-On)
        /// </summary>
        Umc_sso = 3,
    }
}
