// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse
{
    /// <summary>
    /// Optional parameter for ApiAlarms.Browse request
    /// </summary>
    public class ApiAlarms_RequestFilters
    {
        /// <summary>
        /// The mode if the attributes shall either be included or excluded. Can be either include or exclude.
        /// </summary>
        public ApiBrowseFilterMode Mode { get; set; }
        /// <summary>
        /// If present, the user has the option to include or exclude certain attributes, see mode parameter. <br/>
        /// Possible array entries are: "alarm_text", "info_text", "status", "timestamp", "acknowledgement", "alarm_number", "producer"
        /// </summary>
        public List<ApiAlarmsBrowseFilterAttribute> Attributes { get; set; }

        /// <summary>
        /// Base constructor for ApiAlarms_RequestFilters
        /// </summary>
        public ApiAlarms_RequestFilters() { }
        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="mode">The mode if the attributes shall be included or excluded. Can be either include or exclude.</param>
        /// <param name="attributes">List of possible filter attributes.</param>
        public ApiAlarms_RequestFilters(ApiBrowseFilterMode mode, List<ApiAlarmsBrowseFilterAttribute> attributes)
        {
            Mode = mode;
            Attributes = attributes;
        }
    }
}
