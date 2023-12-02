// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse
{
    /// <summary>
    /// Represents an individual alarm entry
    /// </summary>
    public class ApiAlarms_Entry
    {
        /// <summary>
        /// The ID of the alarm. It is a 64-bit value that must be encoded as a string.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Contains the alarm number. 
        /// </summary>
        public int? Alarm_Number { get; set; }
        /// <summary>
        /// Contains the alarm status. The value must contain either "incoming" or "outgoing". 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiObjectDirectoryStatus? Status { get; set; }
        /// <summary>
        /// The UTC timestamp on when the alarm went into incoming or outgoing state, provided as ISO 8601 string. <br/> 
        /// This attribute does not consider the timestamp of acknowledgement. The precision must be in nanoseconds.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The producer of the alarm.
        /// </summary>
        public string Producer { get; set; }
        /// <summary>
        /// Contains the hardware identifier in case that the alarm producer is system_diagnostics. <br/>
        /// If any other case, the attribute will not be returned.
        /// </summary>
        public int? Hwid { get; set; }
        /// <summary>
        /// This exist if the alarm is an alarm that is generally acknowledgeable. <br/> 
        /// If the alarm was not configured as acknowledgeable alarm, then this object must not be returned.
        /// </summary>
        public ApiAlarms_EntryAcknowledgement Acknowledgement { get; set; }
        /// <summary>
        /// The alarm text in the language returned by attribute language. <br/>
        /// Must be empty if the text parsing failed due to an internal error.
        /// </summary>
        public string Alarm_Text { get; set; }
        /// <summary>
        /// The info text in the language returned by attribute language. <br/> 
        /// Must be empty if the text parsing failed due to an internal error.
        /// </summary>
        public string Info_Text { get; set; }
        /// <summary>
        /// This attribute must be present if either alarm_text or info_text is part of the response. <br/> 
        /// If any of the two texts is inconsistent, then this flag must return true.
        /// </summary>
        public bool? Text_Inconsistent { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the ApiAlarmsBrowse are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiAlarms_Entry;
            if (structure is null)
            {
                return false;
            }
            if ((structure.Acknowledgement == null) != (this.Acknowledgement == null))
            {
                return false;
            }
            if (structure.Acknowledgement != null)
            {
                if (!structure.Acknowledgement.Equals(this.Acknowledgement))
                {
                    return false;
                }
            }
            return structure != null &&
                   structure.Id == this.Id &&
                   structure.Alarm_Number == this.Alarm_Number &&
                   structure.Status == this.Status &&
                   structure.Timestamp == this.Timestamp &&
                   structure.Hwid == this.Hwid &&
                   structure.Alarm_Text == this.Alarm_Text &&
                   structure.Info_Text == this.Info_Text &&
                   structure.Text_Inconsistent == this.Text_Inconsistent;
        }

        /// <summary>
        /// GetHashCode for ApiAlarm_Entry
        /// </summary>
        /// <returns>Hash code of ApiAlarm_Entry</returns>
        public override int GetHashCode()
        {
            int acknowledgementHash = 0;
            if (Acknowledgement != null)
            {
                acknowledgementHash ^= Acknowledgement.GetHashCode();
            }
            return (Id, Alarm_Number, Status, Timestamp, Hwid, acknowledgementHash, Alarm_Text, Info_Text, Text_Inconsistent).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiAlarms_Entry
        /// </summary>
        /// <returns>ApiAlarms as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiAlarms_Entry
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
