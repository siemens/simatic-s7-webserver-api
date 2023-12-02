// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// PlcProgram read or write mode => simple/raw
    /// </summary>
    public enum ApiPlcProgramReadOrWriteMode
    {
        /// <summary>
        /// Should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// "Simple" - comfortable format
        /// </summary>
        Simple = 1,
        /// <summary>
        /// "Raw" format to get/write the bytearray
        /// </summary>
        Raw = 2
    }
}
