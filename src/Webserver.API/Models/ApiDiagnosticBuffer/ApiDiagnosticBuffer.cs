// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer
{
    /// <summary>
    /// Containes information of the diagnosticbuffer and it's entires: <br/>
    /// List of entries, Time of last modification, Count of current entries, Count of maximum entires, Language of the entries
    /// </summary>
    public class ApiDiagnosticBuffer
    {
        /// <summary>
        /// The language of the response in which the texts are returned. <br/>
        /// If the provided language in the request was invalid, invalid must be returned by the API.
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// The timestamp of the system time of the last change to the diagnostic buffer (same as the timestamp of the last entry), expressed as ISO 8601 timestamp. <br/>
        /// Precision must be nanoseconds. 
        /// </summary>
        public DateTime Last_Modified { get; set; }
        /// <summary>
        /// The number of available diagnostic buffer entries. It must be the number of total available entries, not the entries returned by a filtered request.
        /// </summary>
        public int Count_Current { get; set; }
        /// <summary>
        /// The maximum number of possible diagnostic buffer entries.
        /// </summary>
        public int Count_Max { get; set; }
        /// <summary>
        /// The array of diagnostic buffer entries where each object represents an individual diagnostic buffer entry. <br/> 
        /// It must be omitted from the response if the requested count was 0. Otherwise, it must contain an array of entries.
        /// </summary>
        public List<ApiDiagnosticBuffer_Entry> Entries { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj">ApiDiagnosticBuffer</param>
        /// <returns>Returns true if the ApiDiagnosticBuffer are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiDiagnosticBuffer;
            if (structure == null) { return false; }
            if ((structure.Entries == null) != (this.Entries == null))
            {
                return false;
            }
            if (structure.Entries != null)
            {
                if (structure.Entries.Count != this.Entries.Count) { return false; }
                for (int i = 0; i < structure.Entries.Count; i++)
                {
                    if (!structure.Entries[i].Equals(this.Entries[i])) { return false; }
                }
            }
            return structure.Last_Modified == this.Last_Modified &&
                   structure.Count_Current == this.Count_Current &&
                   structure.Count_Max == this.Count_Max &&
                   structure.Language == this.Language;
        }
        /// <summary>
        /// GetHashCode for ApiDiagnosticBuffer etc.
        /// </summary>
        /// <returns>hashcode for the ApiDiagnosticBuffer</returns>
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
            return (EntriesHashCode, Last_Modified, Count_Current, Count_Max, Language).GetHashCode();
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer
        /// </summary>
        /// <returns>ApiDiagnosticBuffer as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiDiagnosticBuffer as a multiline string</returns>
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
