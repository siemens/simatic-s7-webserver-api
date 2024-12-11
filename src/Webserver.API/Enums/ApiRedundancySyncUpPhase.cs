// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Enum containing all possible syncup phases.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiRedundancySyncupPhase
    {
        /// <summary>
        /// All other states that are not used anymore or shall never occur will be mapped to this value.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Precondition checks are performed. In this phase, the system checks for IP address duplicates, open ring etc.
        /// </summary>
        Checking_preconditions = 1,
        /// <summary>
        /// Copying memory card content to Backup PLC.
        /// </summary>
        Copying_memory_card = 2,
        /// <summary>
        /// Waiting for restart of the Backup PLC.
        /// </summary>
        Rebooting_backup = 3,
        /// <summary>
        /// Preparing for transfer of work memory.
        /// </summary>
        Preparing_work_memory = 4,
        /// <summary>
        /// Copying work memory to Backup PLC.
        /// </summary>
        Copying_work_memory = 5,
        /// <summary>
        /// The Backup PLC has completely received the snapshot and is now minimizing the delay between this PLC and the Backup PLC (catching up).
        /// </summary>
        Minimizing_delay = 6,
        /// <summary>
        /// Cancellation of the Syncup process is currently ongoing.
        /// </summary>
        Cancelling = 7,
        /// <summary>
        /// The system entered the redundant system state.
        /// </summary>
        Redundant = 8,
        /// <summary>
        /// The system is not synchronized.
        /// </summary>
        Not_redundant = 9,
    }
}
