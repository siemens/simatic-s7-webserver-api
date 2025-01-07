// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiSyslog
{
    /// <summary>
    /// Content of the PLC-internal syslog ring buffer
    /// </summary>
    public class ApiPlcSyslog
    {
        /// <summary>
        /// Holds an array of objects where each object represents a single syslog message of the PLC-internal syslog ring buffer.
        /// </summary>
        public List<ApiPlcSyslog_Entry> Entries { get; set; }
        /// <summary>
        /// This attribute contains the total number of insertions into the syslog buffer since PLC booted up.
        /// </summary>
        public uint Count_Total { get; set; }
        /// <summary>
        /// This attribute contains the number of insertions into the syslog buffer that were lost, <br/> 
        /// meaning the number of the entries which were overwritten by new entries, and which were not saved to a syslog server.
        /// </summary>
        public uint Count_Lost { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the ApiSyslogs are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPlcSyslog syslog &&
                   Entries.SequenceEqual(syslog.Entries) &&
                   Count_Total == syslog.Count_Total &&
                   Count_Lost == syslog.Count_Lost;
        }

        /// <summary>
        /// GetHashCode for ApiSyslog
        /// </summary>
        /// <returns>The hash code of the apiSyslog</returns>
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
            return (EntriesHashCode, Count_Total, Count_Lost).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiPlcSyslog
        /// </summary>
        /// <returns>ApiPlcSyslog as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiPlcSyslog
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiPlcSyslog as a multiline string</returns>
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
