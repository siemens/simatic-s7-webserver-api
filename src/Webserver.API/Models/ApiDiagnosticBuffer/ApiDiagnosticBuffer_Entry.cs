// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer
{
    /// <summary>
    /// Containes one entry from the Diagnosticbuffer.
    /// </summary>
    public class ApiDiagnosticBuffer_Entry
    {
        /// <summary>
        /// This attribute is provided as UTC time. The local time of the PLC is not considered for the returned timestamp. <br/> 
        /// The timestamp of the diagnostic buffer entry in UTC Time, expressed as ISO 8601 timestamp. Precision must be nanoseconds.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Either "incoming" or "outgoing". 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiObjectDirectoryStatus Status { get; set; }
        /// <summary>
        /// The long diagnostic buffer text. <br/> Must be empty if the text parsing failed due to an internal error.
        /// </summary>
        public string Long_Text { get; set; }
        /// <summary>
        /// The short diagnostic buffer text. <br/> Must be empty if the text parsing failed due to an internal error.
        /// </summary>
        public string Short_Text { get; set; }
        /// <summary>
        /// The help text message in case of an incoming event. <br/> Must be empty if the text parsing failed due to an internal error.
        /// </summary>
        public string Help_Text { get; set; }
        /// <summary>
        /// Contains the event ID of the diagnostic buffer entry which consists of the text list ID and text ID of the event. <br/> 
        /// On a client, the event ID is usually shown in hexadecimal representation, for example "16# 02:426A".
        /// </summary>
        public ApiDiagnosticBuffer_EntryEvent Event { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiDiagnosticBuffer_Entry</param>
        /// <returns>Returns true if the ApiDiagnosticBuffer_Entries are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiDiagnosticBuffer_Entry;
            if (structure == null) { return false; }
            if ((structure.Short_Text == null) != (this.Short_Text == null))
            {
                return false;
            }
            if ((structure.Long_Text == null) != (this.Long_Text == null))
            {
                return false;
            }
            if ((structure.Help_Text == null) != (this.Help_Text == null))
            {
                return false;
            }
            return structure.Timestamp == this.Timestamp &&
                   structure.Status == this.Status &&
                   (structure.Long_Text ?? "") == (this.Long_Text ?? "") &&
                   (structure.Short_Text ?? "") == (this.Short_Text ?? "") &&
                   (structure.Help_Text ?? "") == (this.Help_Text ?? "") &&
                   structure.Event.Equals(this.Event);
        }
        /// <summary>
        /// GetHashCode for DiagnosticBufferEvent
        /// </summary>
        /// <returns>hashcode for the DiagnosticBufferEvent</returns>
        public override int GetHashCode()
        {
            return (Timestamp, Status, Long_Text ?? "", Short_Text ?? "", Help_Text ?? "", Event.GetHashCode()).GetHashCode();
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer_Entry
        /// </summary>
        /// <returns>ApiDiagnosticBuffer_Entry as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer_Entry
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiDiagnosticBuffer_Entry as a multiline string</returns>
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
