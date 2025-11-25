// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents information on the usage of HMI subscriptions
    /// </summary>
    public class CommunicationProtocolResourcesProtocolsHmiSubscriptions
    {
        /// <summary>
        /// The number of free HMI subscriptions
        /// </summary>
        public uint Free { get; set; }

        /// <summary>
        /// The maximum number of HMI subscriptions
        /// </summary>
        public uint Max { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResourcesProtocolsHmiSubscriptions subscriptions &&
                   Free == subscriptions.Free &&
                   Max == subscriptions.Max;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Free, Max).GetHashCode();
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
    /// Represents information on the usage of HMI subscription attributes
    /// </summary>
    public class CommunicationProtocolResourcesProtocolsHmiSubscriptionAttributes
    {
        /// <summary>
        /// The number of free HMI subscription attributes
        /// </summary>
        public uint Free { get; set; }

        /// <summary>
        /// The maximum number of HMI subscription attributes
        /// </summary>
        public uint Max { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResourcesProtocolsHmiSubscriptionAttributes attributes &&
                   Free == attributes.Free &&
                   Max == attributes.Max;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Free, Max).GetHashCode();
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
    /// Represents information on the usage of HMI subscription memory
    /// </summary>
    public class CommunicationProtocolResourcesProtocolsHmiSubscriptionMemory
    {
        /// <summary>
        /// The number of bytes of the free HMI subscription memory
        /// </summary>
        public uint Free_Bytes { get; set; }

        /// <summary>
        /// The maximum number of bytes of the HMI subscription memory
        /// </summary>
        public uint Max_Bytes { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResourcesProtocolsHmiSubscriptionMemory memory &&
                   Free_Bytes == memory.Free_Bytes &&
                   Max_Bytes == memory.Max_Bytes;
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Free_Bytes, Max_Bytes).GetHashCode();
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
    /// Protocol resource information for HMI communication
    /// </summary>
    public class CommunicationProtocolResourcesProtocolsHmi
    {
        /// <summary>
        /// Represents information on the usage of HMI subscriptions
        /// </summary>
        public CommunicationProtocolResourcesProtocolsHmiSubscriptions Subscriptions { get; set; }

        /// <summary>
        /// Represents information on the usage of HMI subscription attributes
        /// </summary>
        public CommunicationProtocolResourcesProtocolsHmiSubscriptionAttributes SubscriptionAttributes { get; set; }

        /// <summary>
        /// Represents information on the usage of HMI subscription memory
        /// </summary>
        public CommunicationProtocolResourcesProtocolsHmiSubscriptionMemory SubscriptionMemory { get; set; }
        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResourcesProtocolsHmi hmi &&
                   EqualityComparer<CommunicationProtocolResourcesProtocolsHmiSubscriptions>.Default.Equals(Subscriptions, hmi.Subscriptions) &&
                   EqualityComparer<CommunicationProtocolResourcesProtocolsHmiSubscriptionAttributes>.Default.Equals(SubscriptionAttributes, hmi.SubscriptionAttributes) &&
                   EqualityComparer<CommunicationProtocolResourcesProtocolsHmiSubscriptionMemory>.Default.Equals(SubscriptionMemory, hmi.SubscriptionMemory);
        }
        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Subscriptions, SubscriptionAttributes, SubscriptionMemory).GetHashCode();
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
    /// A list of protocols for which resource information is available
    /// </summary>
    public class CommunicationProtocolResourcesProtocols
    {
        /// <summary>
        /// Protocol resource information for HMI communication
        /// </summary>
        public CommunicationProtocolResourcesProtocolsHmi Hmi { get; set; }


        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResourcesProtocols protocols &&
                   EqualityComparer<CommunicationProtocolResourcesProtocolsHmi>.Default.Equals(Hmi, protocols.Hmi);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Hmi).GetHashCode();
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
    /// Contains various protocol resource information
    /// </summary>
    public class CommunicationProtocolResources
    {
        /// <summary>
        /// A list of protocols for which resource information is available
        /// </summary>
        public CommunicationProtocolResourcesProtocols Protocols { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is CommunicationProtocolResources resources &&
                   Protocols.Equals(resources.Protocols);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Protocols).GetHashCode();
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
