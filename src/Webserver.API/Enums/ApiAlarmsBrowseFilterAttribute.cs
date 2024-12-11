// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Possible filters for ApiAlarmsBrowse request
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiAlarmsBrowseFilterAttribute
    {
        /// <summary>
        /// Filter for the alarm text.
        /// </summary>
        [EnumMember(Value = "alarm_text")]
        AlarmText = 1,
        /// <summary>
        /// Filter for the info text.
        /// </summary>
        [EnumMember(Value = "info_text")]
        InfoText = 2,
        /// <summary>
        /// Filter for the alarm status. The value will contain either "incoming" or "outgoing".
        /// </summary>
        [EnumMember(Value = "status")]
        Status = 3,
        /// <summary>
        /// Filter for the UTC timestamp on when the alarm went into incoming or outgoing state, provided as ISO 8601 string. <br/>
        /// This attribute does not consider the timestamp of acknowledgement. The precision will be in nanoseconds.
        /// </summary>
        [EnumMember(Value = "timestamp")]
        Timestamp = 4,
        /// <summary>
        /// Filter for the acknowledgement. The acknowledgement exist if the alarm is acknowledgeable. If the alarm was not configured as acknowledgeable alarm, then no acknowledgement will be returned regardless of this filter.
        /// </summary>
        [EnumMember(Value = "acknowledgement")]
        Acknowledgement = 5,
        /// <summary>
        /// Filter for the the alarm number.
        /// </summary>
        [EnumMember(Value = "alarm_number")]
        AlarmNumber = 6,
        /// <summary>
        /// Filter for the producer of the alarm.
        /// </summary>
        [EnumMember(Value = "producer")]
        Producer = 7
    }
}
