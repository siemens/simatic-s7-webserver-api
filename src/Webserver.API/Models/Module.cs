// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A node returned by Modules.Browse
    /// </summary>
    public class Module
    {
        /// <summary>
        /// the HWID of this node
        /// </summary>
        public uint Hwid { get; set; }

        /// <summary>
        /// The name of this node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The class of this node
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// The type of this node
        /// </summary>
        public ApiModulesNodeType Type { get; set; }

        /// <summary>
        /// The subtype of this node
        /// </summary>
        public ApiModulesNodeSubType SubType { get; set; }

        /// <summary>
        /// The list of capabilities of this node
        /// </summary>
        public List<ApiModulesNodeAttribute> Attributes { get; set; }

        /// <summary>
        /// If available and requested, the information if there are children available for this node
        /// </summary>
        [JsonProperty("has_children")]
        public bool HasChildren { get; set; }

        /// <summary>
        /// If available and requested, the children of this node
        /// </summary>
        public List<Module> Children { get; set; }

        /// <summary>
        /// If available, the parent nodes of this node
        /// </summary>
        public List<uint> Parent { get; set; }

        /// <summary>
        /// A node returned by Modules.Browse 
        /// </summary>
        public Module()
        {
            Attributes = new List<ApiModulesNodeAttribute>();
            Children = new List<Module>();
            Parent = new List<uint>();
        }

        /// <summary>
        /// Called after deserialization to ensure HasChildren is set correctly
        /// </summary>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // Ensure HasChildren is set correctly after deserialization
            if (Children?.Count > 0)
            {
                HasChildren = true;
            }
        }

        /// <summary>
        /// Returns true if the incoming object has the same property values as this
        /// </summary>
        /// <param name="obj">incoming object to compare to</param>
        /// <returns>true if the incoming object has the same property values as this</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is Module node)
            {
                return Hwid == node.Hwid &&
                       Name == node.Name &&
                       Class == node.Class &&
                       Type == node.Type &&
                       SubType == node.SubType &&
                       HasChildren == node.HasChildren &&
                       Attributes.Count == node.Attributes.Count &&
                       Attributes.SequenceEqual(node.Attributes) &&
                       Children.Count == node.Children.Count &&
                       Children.SequenceEqual(node.Children) &&
                       Parent.Count == node.Parent.Count &&
                       Parent.SequenceEqual(node.Parent);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode of this object for Equality comparison
        /// </summary>
        /// <returns>Hashcode of this object for Equality comparison</returns>
        public override int GetHashCode()
        {
            int hashCode = 332878323;
            hashCode = hashCode * -1521134295 + Hwid.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Class);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + SubType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ApiModulesNodeAttribute>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + HasChildren.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Module>>.Default.GetHashCode(Children);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<uint>>.Default.GetHashCode(Parent);
            return hashCode;
        }

        /// <summary>
        /// Json Serialized object
        /// </summary>
        /// <returns>Json Serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A response of Modules.Browse that represents one or more nodes
    /// </summary>
    public class ModulesBrowse
    {
        /// <summary>
        /// List of nodes that were returned for a requested HWID
        /// </summary>
        public List<Module> Nodes { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesBrowse browse &&
                   Nodes.Count == browse.Nodes.Count &&
                   Nodes.SequenceEqual(browse.Nodes);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Nodes).GetHashCode();
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
