// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
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

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
