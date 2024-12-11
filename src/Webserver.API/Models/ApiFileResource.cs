// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// File resource 
    /// </summary>
    public class ApiFileResource : ICloneable
    {
        /// <summary>
        /// Name of the resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Size in bytes of the resource (dirs don't have a size themselves)
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Last modified date of the resource (UTC)(as string according to RFC3339)
        /// </summary>
        public DateTime Last_Modified { get; set; }

        private ApiFileResourceType type;
        /// <summary>
        /// File resource type: is the resource a file or a directory or ...?
        /// </summary>
        public ApiFileResourceType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value == ApiFileResourceType.None)
                {
                    throw new ApiInvalidResponseException($"Returned from api was:{value.ToString()} - which is not valid! contact Siemens");
                }
                type = value;
            }
        }

        /// <summary>
        /// File resource state: is the resource (datalog) active/inactive/...? otherwise: none
        /// </summary>
        public ApiFileResourceState State { get; set; } = ApiFileResourceState.Active;

        /// <summary>
        /// May be null Parentsarray - used e.g. for the Namebuilding
        /// </summary>
        [JsonIgnore]
        public List<ApiFileResource> Parents { get; set; }

        /// <summary>
        /// PathToLocalDirectory: Path to the local Directory containing all the resources
        /// </summary>
        public string PathToLocalDirectory { get; set; }

        /// <summary>
        /// The built Resources on the local machine
        /// </summary>
        [JsonIgnore]
        public List<ApiFileResource> Resources { get; set; } = new List<ApiFileResource>();

        /// <summary>
        /// Get Var Name for Methods => all parents: Name with Quotes ("DB".) and dot
        /// </summary>
        /// <returns></returns>
        public string GetVarNameForMethods()
        {
            string varName = "/";
            if (this.Parents != null)
            {
                foreach (var parent in this.Parents)
                {
                    varName += parent.GetNameWithSlash();
                }
            }
            varName += this.Name;
            return varName;
        }

        /// <summary>
        /// Return 
        /// </summary>
        /// <returns></returns>
        public string GetNameWithSlash()
        {
            return this.Name + "/";
        }

        /// <summary>
        /// (Name,Type,State,Size,Parents,Children) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiFileResource);

        /// <summary>
        /// Equals => (Name,Type,State,Size,Parents,Children)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ApiFileResource obj)
        {
            if (obj is null)
                return false;
            var toReturn = this.Name == obj.Name;
            toReturn &= this.Type == obj.Type;
            toReturn &= this.State == obj.State;
            if (this.Size != null && obj.Size != null)
            {
                toReturn &= this.Size == obj.Size;
            }
            else
            {
                toReturn &= this.Size == null && obj.Size == null;
            }
            if (this.Parents != null && obj.Parents != null)
            {
                //toReturn &= this.Parents.Count == obj.Parents.Count ? this.Parents.SequenceEqual(obj.Parents) : false;
                // do not compare when parents are not null => otherwise: Loop by children compare
                ;
            }
            else
            {
                toReturn &= this.Parents == null && obj.Parents == null;
            }
            if (this.Resources != null && obj.Resources != null)
            {
                toReturn &= this.Resources.Count == obj.Resources.Count ? this.Resources.SequenceEqual(obj.Resources) : false;
            }
            else
            {
                toReturn &= this.Resources == null && obj.Resources == null;
            }
            return toReturn;
        }

        /// <summary>
        /// Call GetNameWithSlash for debugging comfort!
        /// </summary>
        /// <returns>Name with slash</returns>
        public override string ToString()
        {
            return GetNameWithSlash();
        }

        /// <summary>
        /// return (Name, Type, State, Size, Parents, Children).GetHashCode();
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return (Name, Type, State, Size, Parents, Resources).GetHashCode();
        }

        /// <summary>
        /// Get a Clone of the ApiFileResource
        /// </summary>
        /// <returns>the cloned ApiFileResource</returns>
        public object Clone()
        {
            var resource = (ApiFileResource)MemberwiseClone();
            return resource;
        }
    }
}
