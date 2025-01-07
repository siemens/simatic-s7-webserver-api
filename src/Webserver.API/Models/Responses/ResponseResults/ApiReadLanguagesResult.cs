// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Read Languages result containing a List of ApiLanguageResults
    /// </summary>
    public class ApiReadLanguagesResult
    {
        /// <summary>
        /// List containing languages (CultureInfo) wrapped in an ApiLanguageResult
        /// </summary>
        public List<ApiLanguage> Languages { get; set; }
    }
}
