// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// The pairing status of the R/H system.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiPlcRedundancyPairingState
    {
        /// <summary>
        /// Default value if the pairing status could not be retrieved or was 0.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        /// <summary>
        /// Redundant connection between both PLCs.
        /// </summary>
        [EnumMember(Value = "paired")]
        Paired = 1,
        /// <summary>
        /// Only single pairing via port 1 (or 2) is available.
        /// </summary>
        [EnumMember(Value = "paired_single")]
        Paired_single = 2,
        /// <summary>
        /// There are more than 2 R/H PLCs in the network. Cannot determine partner PLC.
        /// </summary>
        [EnumMember(Value = "not_paired_too_many_partners")]
        Not_paired_too_many_partners = 3,
        /// <summary>
        /// The partner PLC is not reachable or internal error, represented as unspecified error to the user.
        /// </summary>
        [EnumMember(Value = "not_paired")]
        Not_paired = 4,
        /// <summary>
        /// The order IDs of both PLCs of the R/H system do not match.
        /// </summary>
        [EnumMember(Value = "not_paired_order_id_mismatch")]
        Not_paired_order_id_mismatch = 5,
        /// <summary>
        /// The firmware versions of both PLCs of the R/H system do not match.
        /// </summary>
        [EnumMember(Value = "not_paired_firmware_mismatch")]
        Not_paired_firmware_mismatch = 6,

    }
}
