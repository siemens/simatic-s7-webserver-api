// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Class containing one language (culture info)
    /// </summary>
    public class ApiLanguage
    {
        /// <summary>
        /// Language of the project
        /// </summary>
        public CultureInfo Language { get; set; }
        /// <summary>
        /// The state tells if the project language is available for usage by API methods such as Alarms.Browse or not.
        /// Only present from V40 and onwards.
        /// </summary>
        public bool? Active { get; set; }
        /// <summary>
        /// Each element of the array represents one user interface language for which the given project language should be applied for as default.
        /// A client application may overrule this information.
        /// The setting is configured in TIA Portal or through the user program.
        /// Only present from V40 and onwards.
        /// </summary>
        public List<CultureInfo> User_interface_languages { get; set; }

        /// <summary>
        /// Compares input object to this ApiLanguage
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiLanguage language &&
                   language.Language.Equals(Language) &&
                   Active == language.Active && (language.User_interface_languages == null ? User_interface_languages == null : (User_interface_languages == null ? false : language.User_interface_languages.SequenceEqual(User_interface_languages)));
        }

        /// <summary>
        /// Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = (Language, Active).GetHashCode();
            if (User_interface_languages != null)
            {
                for (int i = 0; i < User_interface_languages.Count; i++)
                {
                    hash ^= User_interface_languages[i].GetHashCode();
                }
            }
            return hash;
        }
    }
}
