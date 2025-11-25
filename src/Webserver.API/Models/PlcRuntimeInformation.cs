// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Services.Converters.JsonConverters;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Actual PLC runtime load information
    /// </summary>
    public class PlcRuntimeInformationLoadActual
    {
        /// <summary>
        /// The percentage of the program load caused by cyclic program OBs
        /// </summary>
        [JsonProperty("program_load_cyclic_program_obs_percentage")]
        public uint ProgramLoadCyclicProgramObsPercentage { get; set; }

        /// <summary>
        /// The percentage of the program load caused by high priority OBs
        /// </summary>
        [JsonProperty("program_load_high_priority_obs_percentage")]
        public uint ProgramLoadHighPriorityObsPercentage { get; set; }

        /// <summary>
        /// The percentage of the communication load
        /// </summary>
        [JsonProperty("current_communication_load_percentage")]
        public uint CurrentCommunicationLoadPercentage { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationLoadActual runtimeInformationLoadActual &&
                   ProgramLoadCyclicProgramObsPercentage == runtimeInformationLoadActual.ProgramLoadCyclicProgramObsPercentage &&
                   ProgramLoadHighPriorityObsPercentage == runtimeInformationLoadActual.ProgramLoadHighPriorityObsPercentage &&
                   CurrentCommunicationLoadPercentage == runtimeInformationLoadActual.CurrentCommunicationLoadPercentage;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (ProgramLoadCyclicProgramObsPercentage, ProgramLoadHighPriorityObsPercentage, CurrentCommunicationLoadPercentage).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Configured PLC runtime load information
    /// </summary>
    public class PlcRuntimeInformationLoadConfigured
    {
        /// <summary>
        /// The maximum commmunication load percentage as configured in the loaded project
        /// </summary>
        [JsonProperty("max_communication_load_percentage")]
        public uint MaxCommunicationLoadPercentage { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationLoadConfigured runtimeInformationLoadConfigured &&
                   MaxCommunicationLoadPercentage == runtimeInformationLoadConfigured.MaxCommunicationLoadPercentage;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (MaxCommunicationLoadPercentage).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// PLC load information
    /// </summary>
    public class PlcRuntimeInformationLoad
    {
        /// <summary>
        /// Actual PLC runtime load information
        /// </summary>
        public PlcRuntimeInformationLoadActual Actual { get; set; }

        /// <summary>
        /// Configured PLC runtime load information
        /// </summary>
        public PlcRuntimeInformationLoadConfigured Configured { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationLoad runtimeInformationLoad &&
                   EqualityComparer<PlcRuntimeInformationLoadActual>.Default.Equals(Actual, runtimeInformationLoad.Actual) &&
                   EqualityComparer<PlcRuntimeInformationLoadConfigured>.Default.Equals(Configured, runtimeInformationLoad.Configured);
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Actual, Configured).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Information on the actual cycle time
    /// </summary>
    public class PlcRuntimeInformationCycleTimeActual
    {
        /// <summary>
        /// The shortest actual cycle time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Shortest { get; set; }

        /// <summary>
        /// The latest actual cycle time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Current { get; set; }

        /// <summary>
        /// The longest actual cycle time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Longest { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationCycleTimeActual runtimeInformationCycleTimeActual &&
                   Shortest.Equals(runtimeInformationCycleTimeActual.Shortest) &&
                   Current.Equals(runtimeInformationCycleTimeActual.Current) &&
                   Longest.Equals(runtimeInformationCycleTimeActual.Longest);
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Shortest, Current, Longest).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Configured PLC runtime cycle time information
    /// </summary>
    public class PlcRuntimeInformationCycleTimeConfigured
    {
        /// <summary>
        /// The minimum configured cycle time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Min { get; set; }

        /// <summary>
        /// The maximum configured cycle time
        /// </summary>
        [JsonConverter(typeof(TimeSpanISO8601Converter))]
        public TimeSpan Max { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationCycleTimeConfigured configured &&
                   Min.Equals(configured.Min) &&
                   Max.Equals(configured.Max);
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Min, Max).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// PLC cycle time information
    /// </summary>
    public class PlcRuntimeInformationCycleTime
    {
        /// <summary>
        /// Actual PLC runtime cycle time information
        /// </summary>
        public PlcRuntimeInformationCycleTimeActual Actual { get; set; }

        /// <summary>
        /// Configured PLC runtime cycle time information
        /// </summary>
        public PlcRuntimeInformationCycleTimeConfigured Configured { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformationCycleTime time &&
                   EqualityComparer<PlcRuntimeInformationCycleTimeActual>.Default.Equals(Actual, time.Actual) &&
                   EqualityComparer<PlcRuntimeInformationCycleTimeConfigured>.Default.Equals(Configured, time.Configured);
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Actual, Configured).GetHashCode();
        }
        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A set of criteria defined for passwords.
    /// </summary>
    public class PlcRuntimeInformation
    {
        /// <summary>
        /// PLC load information
        /// </summary>
        public PlcRuntimeInformationLoad Load { get; set; }

        /// <summary>
        ///  PLC cycle time information
        /// </summary>
        [JsonProperty("cycle_time")]
        public PlcRuntimeInformationCycleTime CycleTime { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcRuntimeInformation runtimeInformation &&
                   Load.Equals(runtimeInformation.Load) &&
                   CycleTime.Equals(runtimeInformation.CycleTime);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Load, CycleTime).GetHashCode();
        }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
