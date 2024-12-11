// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiPlcProgramBrowseCodeBlocksData: Data class representing a code block value in the ApiPlcProgramBrowseCodeBlocksRequest response.
    /// </summary>
    public class ApiPlcProgramBrowseCodeBlocksData : IEquatable<ApiPlcProgramBrowseCodeBlocksData>
    {
        /// <summary>
        /// Function to Memberwise clone an ApiPlcProgramData to another Object.
        /// </summary>
        /// <returns></returns>
        public ApiPlcProgramBrowseCodeBlocksData ShallowCopy()
        {
            return (ApiPlcProgramBrowseCodeBlocksData)this.MemberwiseClone();
        }

        /// <summary>
        /// Name of the code block.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Number of the code block.
        /// </summary>
        [JsonProperty("block_number")]
        public ushort BlockNumber { get; set; }

        /// <summary>
        /// Type of the code block.
        /// </summary>
        [JsonProperty("block_type")]
        public ApiPlcProgramBlockType BlockType { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        [JsonConstructor]
        public ApiPlcProgramBrowseCodeBlocksData(string name, ushort blockNumber, ApiPlcProgramBlockType blockType)
        {
            Name = name;
            BlockNumber = blockNumber;
            BlockType = blockType;
        }

        /// <summary>
        /// Returns the Name with Quotes => $"\"{Name}\""
        /// </summary>
        /// <returns></returns>
        public string GetNameWithQuotes()
        {
            return $"\"{Name}\"";
        }

        /// <summary>
        /// return the Serialized Object
        /// </summary>
        /// <returns>Json Object String of the Data</returns>
        public string GetObjectString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// Call GetNameWithQuotes for debugging comfort!
        /// </summary>
        /// <returns>Name with quotes</returns>
        public override string ToString()
        {
            return GetNameWithQuotes();
        }

        /// <summary>
        /// Check the object for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ApiPlcProgramBrowseCodeBlocksData);
        }

        /// <summary>
        /// Check the object for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(ApiPlcProgramBrowseCodeBlocksData obj)
        {
            return !(obj is null) && (obj.Name == Name) && (obj.BlockNumber == BlockNumber) && (obj.BlockType == BlockType);
        }

        /// <summary>
        /// Return the object's hash code.
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return (Name, BlockNumber, BlockType).GetHashCode();
        }
    }
}
