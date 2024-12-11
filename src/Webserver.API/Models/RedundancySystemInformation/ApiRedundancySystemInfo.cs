// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models.RedundancySystemInformation
{
    /// <summary>
    /// Basic information and the pairing status of the R/H system.
    /// </summary>
    public class ApiRedundancySystemInfo
    {
        /// <summary>
        /// The pairing status of the R/H system.
        /// </summary>
        public ApiPlcRedundancyPairingState Pairing_state { get; set; }

        /// <summary>
        /// Describes if the Syncup lock has been enabled via the user program.
        /// If the Syncup lock is enabled, the user will not be able to switch the system to Syncup state.
        /// </summary>
        public bool Syncup_lock { get; set; }

        /// <summary>
        /// Provides the redundancy ID of the PLC through which the HTTP connection has been established to.
        /// </summary>
        public ApiPlcRedundancyId Connected_redundancy_id { get; set; }

        /// <summary>
        /// Describes if the PLC is running in standalone operation or not.
        /// </summary>
        public bool Standalone_operation { get; set; }

        /// <summary>
        /// This object represents the R/H system and its both PLCs.
        /// </summary>
        public ApiRedundancySystemInfo_Plcs Plcs { get; set; }

        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj">ApiRedundancySystemInfo</param>
        /// <returns>Returns true if the ApiRedundancySystemInfo are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiRedundancySystemInfo;
            if (structure == null) { return false; }
            return structure.Pairing_state == this.Pairing_state &&
                   structure.Syncup_lock == this.Syncup_lock &&
                   structure.Standalone_operation == this.Standalone_operation &&
                   structure.Connected_redundancy_id == this.Connected_redundancy_id &&
                   structure.Plcs.Equals(this.Plcs);
        }
        /// <summary>
        /// GetHashCode for ApiRedundancySystemInfo
        /// </summary>
        /// <returns>hashcode for the ApiRedundancySystemInfo</returns>
        public override int GetHashCode()
        {
            return (Pairing_state, Syncup_lock, Standalone_operation, Connected_redundancy_id, Plcs.GetHashCode()).GetHashCode();
        }
        /// <summary>
        /// ToString for ApiRedundancySystemInfo
        /// </summary>
        /// <returns>ApiRedundancySystemInfo as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiRedundancySystemInfo
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiRedundancySystemInfo as a multiline string</returns>
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
