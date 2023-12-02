// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Globalization;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Class containing one language (culture info)
    /// </summary>
    public class ApiLanguage
    {
        /// <summary>
        /// Language with built-in CultureInfo class
        /// </summary>
        public CultureInfo Language { get; set; }
    }
}
