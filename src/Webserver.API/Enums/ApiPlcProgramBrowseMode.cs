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
    /// ApiPlcProgramBrowseMode Values
    /// </summary>
    public enum ApiPlcProgramBrowseMode
    {
        /// <summary>
        ///  should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// Browse the Variable for further information (also dbs can be browsed with var)
        /// </summary>
        Var = 1,
        /// <summary>
        /// Browse all children of the given DB (etc.)
        /// </summary>
        Children = 2
    }
}
