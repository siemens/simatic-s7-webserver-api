// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse
{
    /// <summary>
    /// The current alarms of the PLC
    /// </summary>
    public class ApiAlarms
    {
        /// <summary>
        /// The actual language in which the text entries are returned. <br/> 
        ///If the provided language in the request was invalid, invalid must be returned by the API and all text attributes must be empty strings(if requested for the response).
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// The timestamp of the last modification of the alarming system at the time when the Read request was performed. <br/> 
        /// Checking this, the user could find out if new changes occurred on the alarming system without polling for the data.
        /// </summary>
        public DateTime Last_Modified { get; set; }
        /// <summary>
        /// The number of active alarms. <br/> 
        /// It must be the number of total available entries, not the entries returned by a filtered request.
        /// </summary>
        public uint Count_Current { get; set; }
        /// <summary>
        /// The maximum number of possible alarms. 
        /// </summary>
        public uint Count_Max { get; set; }
        /// <summary>
        /// The array of alarms where each object represents an individual alarm entry. <br/> 
        /// It must be omitted from the response if the requested count was 0. <br/> 
        /// Otherwise, it must contain either an array of alarms or an empty array in case that there are no active alarms.
        /// </summary>
        public List<ApiAlarms_Entry> Entries { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the ApiAlarmsBrowse are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiAlarms;
            if (structure == null)
            {
                return false;
            }
            if ((structure.Entries == null) != (this.Entries == null))
            {
                return false;
            }
            if (structure.Entries != null)
            {
                structure.Entries.SequenceEqual(this.Entries);
            }
            return structure.Language == this.Language &&
                   structure.Last_Modified == this.Last_Modified &&
                   structure.Count_Current == this.Count_Current &&
                   structure.Count_Max == this.Count_Max;
        }

        /// <summary>
        /// GetHashCode for ApiAlarmsBrowse
        /// </summary>
        /// <returns>Hash code of the ApiAlarms</returns>
        public override int GetHashCode()
        {
            int EntriesHashCode = 0;
            if (Entries != null)
            {
                foreach (var entry in Entries)
                {
                    EntriesHashCode ^= entry.GetHashCode();
                }
            }
            return (EntriesHashCode, Language, Last_Modified, Count_Current, Count_Max).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiAlarms
        /// </summary>
        /// <returns>ApiAlarms as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiAlarms
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
