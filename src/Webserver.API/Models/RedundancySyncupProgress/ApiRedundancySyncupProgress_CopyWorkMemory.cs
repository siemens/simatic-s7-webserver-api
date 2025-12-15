// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySyncupProgress
{
    /// <summary>
    /// This object provides information about syncup state 'copying_work_memory'.
    /// </summary>
    public class ApiRedundancySyncupProgress_CopyWorkMemory
    {
        /// <summary>
        /// The number of bytes of the copy work memory that have been transferred so far to the Backup PLC.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// The total number of bytes of the copy work memory to be processed.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySyncupProgress_CopyWorkMemory</param>
        /// <returns>Returns true if the ApiRedundancySyncupProgress_CopyWorkMemory are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySyncupProgress_CopyWorkMemory;
            if (structure == null) { return false; }
            return structure.Current == this.Current &&
                   structure.Total == this.Total;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySyncupProgress_CopyWorkMemory
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySyncupProgress_CopyWorkMemory</returns>
        public override int GetHashCode()
        {
            return (Current, Total).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_CopyWorkMemory
        /// </summary>
        /// <returns>ApiRedundancySyncupProgress_CopyWorkMemory as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_CopyWorkMemory
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySyncupProgress_CopyWorkMemory as a multiline string</returns>
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
