// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySyncupProgress
{
    /// <summary>
    /// Information of an ongoing Syncup phase of the R/H system.
    /// </summary>
    public class ApiRedundancySyncupProgress
    {
        /// <summary>
        /// The currently active Syncup phase.
        /// </summary>
        public ApiRedundancySyncupPhase Syncup_phase { get; set; }

        /// <summary>
        /// If the attribute syncup_phase is in state copying_work_memory, and both its attributes are valid, 
        /// this object will be present in the response, otherwise it will not.
        /// </summary>
        public ApiRedundancySyncupProgress_CopyWorkMemory Copying_work_memory { get; set; }

        /// <summary>
        /// If the attribute syncup_phase is in state copying_memory_card, and both its attributes are valid, 
        /// this object will be present in the response, otherwise it will not.
        /// </summary>
        public ApiRedundancySyncupProgress_CopyMemoryCard Copying_memory_card { get; set; }

        /// <summary>
        /// If the attribute syncup_phase is in state minimizing_delay, and both its attributes are valid, 
        /// this object will be present in the response, otherwise it will not.
        /// </summary>
        public ApiRedundancySyncupProgress_MinimizingDelay Minimizing_delay { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySyncupProgress</param>
        /// <returns>Returns true if the ApiRedundancySyncupProgress are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySyncupProgress;
            if (structure == null) { return false; }
            if (((structure.Copying_memory_card == null) != (this.Copying_memory_card == null)) ||
               ((structure.Copying_work_memory == null) != (this.Copying_work_memory == null)) ||
               ((structure.Minimizing_delay == null) != (this.Minimizing_delay == null)))
            {
                return false;
            }
            return structure.Syncup_phase == this.Syncup_phase &&
                   structure.Copying_memory_card == this.Copying_memory_card &&
                   structure.Copying_work_memory == this.Copying_work_memory &&
                   structure.Minimizing_delay == this.Minimizing_delay;
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySyncupProgress
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySyncupProgress</returns>
        public override int GetHashCode()
        {
            int hash = 1;
            if (Copying_memory_card != null)
            {
                hash *= Copying_memory_card.GetHashCode();
            }
            if (Copying_work_memory != null)
            {
                hash *= Copying_work_memory.GetHashCode();
            }
            if (Minimizing_delay != null)
            {
                hash *= Minimizing_delay.GetHashCode();
            }
            return (Syncup_phase, hash).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiRedundancySyncupProgress
        /// </summary>
        /// <returns>ApiRedundancySyncupProgress as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySyncupProgress
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySyncupProgress as a multiline string</returns>
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
