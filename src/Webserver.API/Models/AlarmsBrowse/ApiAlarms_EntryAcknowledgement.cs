// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse
{
    /// <summary>
    /// This exist if the alarm is an alarm that is generally acknowledgeable. 
    /// </summary>
    public class ApiAlarms_EntryAcknowledgement
    {
        /// <summary>
        /// Readable string that tells the acknowledgement state of the alarm:
        /// <list type="bullet">
        /// <item>not_acknowledged</item>
        /// <item>acknowledged</item>
        /// </list>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiAlarmAcknowledgementState State { get; set; }
        /// <summary>
        /// In case that the alarms current incoming/outgoing status has been acknowledged, the timestamp returns the corresponding acknowledgement time encoded as ISO timestamp.
        /// The precision must be in nanoseconds. <br/>
        /// The timestamp must not be present if the alarm has not been acknowledged yet.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the ApiAlarmsBrowse are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiAlarms_EntryAcknowledgement;
            if (structure == null) { return false; }
            return structure.State == this.State &&
                   structure.Timestamp == this.Timestamp;
        }

        /// <summary>
        /// GetHashCode for ApiAlarmsBrowse
        /// </summary>
        /// <returns>Hash code of ApiAlarm_EntryAcknowledgement</returns>
        public override int GetHashCode()
        {
            return (State, Timestamp).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiAlarms_EntryAcknowledgement
        /// </summary>
        /// <returns>ApiAlarms as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiAlarms_EntryAcknowledgement
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiAlarms as a multiline string</returns>
        public string ToString(NullValueHandling nullValueHandling)
        {
            JsonSerializerSettings options = new JsonSerializerSettings()
            {
                NullValueHandling = nullValueHandling,
            };
            string result = JsonConvert.SerializeObject(this, Formatting.Indented, options);
            return result;
        }
    }
}
