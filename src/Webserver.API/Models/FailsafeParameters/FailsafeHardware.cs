// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.FailsafeParameters
{
    /// <summary>
    /// Base class for Failsafe Hardware Types
    /// </summary>
    [JsonConverter(typeof(FailsafeHardwareConverter))]
    public class FailsafeHardware
    {
    }

    /// <summary>
    /// Failsafe CPU
    /// </summary>
    public class FailsafeCPU : FailsafeHardware
    {
        /// <summary>
        /// The timestamp of the last failsafe program modification.
        /// </summary>
        public DateTime Last_f_program_modification { get; set; }
        /// <summary>
        /// The collective signature encoded as string containing a hexadecimal representation of the 32-bit signature. 
        /// </summary>
        public string Collective_signature { get; set; }

        /// <summary>
        /// The remaining was introduced with Safety system version V2.4. Older versions do not support the remaining time.
        /// The version can be configured in the TIA Portal Safety Administration. 
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan? Remaining_time { get; set; }

        /// <summary>
        /// Checks if the incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they are the same object</returns>
        public override bool Equals(object obj)
        {
            return obj is FailsafeCPU cpu &&
                   Last_f_program_modification == cpu.Last_f_program_modification &&
                   Remaining_time == cpu.Remaining_time &&
                   Collective_signature == cpu.Collective_signature;
        }

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns>(LastFProgramModification, CollectiveSignature).GetHashCode()</returns>
        public override int GetHashCode()
        {
            return (Last_f_program_modification, Collective_signature, Remaining_time).GetHashCode();
        }
    }

    /// <summary>
    /// Failsafe Module
    /// </summary>
    public class FailsafeModule : FailsafeHardware
    {
        /// <summary>
        /// F-monitoring time.
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan F_monitoring_time { get; set; }
        /// <summary>
        /// Contains the F-source address.
        /// </summary>
        public int F_source_address { get; set; }
        /// <summary>
        /// Contains the F-destination address 
        /// </summary>
        public int F_destination_address { get; set; }
        /// <summary>
        /// The F-Par CRC encoded as string containing a hexadecimal representation of the 32-bit signature. 
        /// </summary>
        public string F_par_crc { get; set; }

        /// <summary>
        /// Checks if the incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they are the same object</returns>
        public override bool Equals(object obj)
        {
            return obj is FailsafeModule module &&
                   F_monitoring_time.Equals(module.F_monitoring_time) &&
                   F_source_address == module.F_source_address &&
                   F_destination_address == module.F_destination_address &&
                   F_par_crc == module.F_par_crc;
        }

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns>(FMonitoringTime, FParCrc, FDestinationAddress, FSourceAddress).GetHashCode()</returns>
        public override int GetHashCode()
        {
            return (F_monitoring_time, F_par_crc, F_destination_address, F_source_address).GetHashCode();
        }
    }
}
