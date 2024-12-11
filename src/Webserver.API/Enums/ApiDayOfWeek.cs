// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Represents the days of the week
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(SnakeCaseNamingStrategy))]
    public enum ApiDayOfWeek
    {
        /// <summary>
        /// Sunday
        /// </summary>
        Sun = 1,
        /// <summary>
        /// Monday
        /// </summary>
        Mon = 2,
        /// <summary>
        /// Tuesday
        /// </summary>
        Tue = 3,
        /// <summary>
        /// Wednesday
        /// </summary>
        Wed = 4,
        /// <summary>
        /// Thursday
        /// </summary>
        Thu = 5,
        /// <summary>
        /// Friday
        /// </summary>
        Fri = 6,
        /// <summary>
        /// Saturday
        /// </summary>
        Sat = 7
    }
}
