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
