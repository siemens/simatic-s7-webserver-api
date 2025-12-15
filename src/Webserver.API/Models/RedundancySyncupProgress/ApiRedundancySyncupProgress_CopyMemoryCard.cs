// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySyncupProgress
{
    /// <summary>
    /// This object provides information about syncup state 'copying_memory_card'.
    /// </summary>
    public class ApiRedundancySyncupProgress_CopyMemoryCard
    {
        /// <summary>
        /// The file name of the file that is currently transferred to the Backup PLC.
        /// </summary>
        public string Current_filename { get; set; }

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
        /// <param name="obj">ApiRedundancySyncupProgress_CopyMemoryCard</param>
        /// <returns>Returns true if the ApiRedundancySyncupProgress_CopyMemoryCard are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySyncupProgress_CopyMemoryCard;
            if (structure == null) { return false; }
            return structure.Current_filename == this.Current_filename &&
                   structure.Current == this.Current &&
                   structure.Total == this.Total;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySyncupProgress_CopyMemoryCard
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySyncupProgress_CopyMemoryCard</returns>
        public override int GetHashCode()
        {
            return (Current_filename, Current, Total).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_CopyMemoryCard
        /// </summary>
        /// <returns>ApiRedundancySyncupProgress_CopyMemoryCard as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySyncupProgress_CopyMemoryCard
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySyncupProgress_CopyMemoryCard as a multiline string</returns>
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
