// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Models
{

    /// <summary>
    /// ApiPlcProgramData: Helper Class for easier handling of the PlcProgram functionalities of the plc => represents a db/var,...
    /// </summary>
    public class ApiPlcProgramData
    {
        /// <summary>
        /// Function to MemberwiseClone an ApiPlcProgramData to another Object.
        /// </summary>
        /// <returns></returns>
        public ApiPlcProgramData ShallowCopy()
        {
            return (ApiPlcProgramData)this.MemberwiseClone();
        }

        /// <summary>
        /// Name of the ApiPlcProgramData
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bool to indicate that the ApiPlcProgramData Has Children
        /// </summary>
        public bool? Has_children { get; set; }

        /// <summary>
        /// Db_number => either the DB is the ApiPlcProgramData and its itselfs number or the number the data is contained in
        /// </summary>
        public int Db_number { get; set; }

        private ApiPlcProgramDataType dataType;
        /// <summary>
        /// ApiPlcProgramDataType => helper class to determine the TIA DataTypes and map them to the byte size,...
        /// </summary>
        public ApiPlcProgramDataType Datatype { get { return dataType; }
            set {
                if (value == ApiPlcProgramDataType.None)
                    throw new ApiInvalidResponseException("Api PlcProgramDataType:" + value.ToString() + " was invalid!");
                dataType = value;
            } }

        /// <summary>
        /// Array Dimensions - in case the Data is an array this fills the indexers.
        /// </summary>
        [JsonProperty(Order = 1)]
        public List<ApiPlcProgramDataArrayIndexer> Array_dimensions { get; set; }

        /// <summary>
        /// May be null - Max Length of the Data
        /// </summary>
        public int? Max_length { get; set; }

        /// <summary>
        /// May be null Address of the Data
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// May be null Area of the Data
        /// </summary>
        public char? Area { get; set; }

        /// <summary>
        /// May be null True when Resource is Read_only
        /// </summary>
        public bool? Read_only { get; set; }

        /// <summary>
        /// May be null - can be set to the resource using PlcProgram.Read,...
        /// </summary>
        public object Value { get; set; }


        /// <summary>
        /// May be null Parentsarray - used e.g. for the Namebuilding
        /// </summary>
        [JsonIgnore]
        public List<ApiPlcProgramData> Parents { get; set; }

        /// <summary>
        /// may be null - used to search for some specific data, ...
        /// </summary>
        public List<ApiPlcProgramData> Children { get; set; }

        /// <summary>
        /// Default c'tor to instanciate Parents and Children
        /// </summary>
        public ApiPlcProgramData()
        {
            this.Parents = new List<ApiPlcProgramData>();
            this.Children = new List<ApiPlcProgramData>();
        }

        /// <summary>
        /// Returns the Name with Quotes => $"\"{Name}\""
        /// </summary>
        /// <returns></returns>
        public string GetNameWithQuotes()
        {
            return "\"" + Name + "\"";
        }

        /// <summary>
        /// Get Var Name for Methods => all parents: Name with Quotes ("DB".) and dot
        /// </summary>
        /// <returns></returns>
        public string GetVarNameForMethods()
        {
            string varName = "";
            if (this.Parents != null)
            {
                foreach (var parent in this.Parents)
                {
                    varName += parent.GetNameWithQuotes() + ".";
                }
            }
            varName += this.GetNameWithQuotes();
            return varName;
        }

        /// <summary>
        /// return the Serialized Object
        /// </summary>
        /// <returns>Json Object String of the Data</returns>
        public string GetObjectString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver()
                , NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is ApiPlcProgramData)
            {
                return Equals(obj as ApiPlcProgramData);
            }
            return false;


        }

        /// <summary>
        /// Equals => (Name,Has_children,Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only, Value, Children)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ApiPlcProgramData obj)
        {
            var toReturn = this.Name == obj.Name;
            toReturn &= this.Has_children == obj.Has_children;
            toReturn &= this.Db_number == obj.Db_number;
            toReturn &= this.Datatype == obj.Datatype;
            toReturn &= this.Array_dimensions == obj.Array_dimensions;
            toReturn &= this.Max_length == obj.Max_length;
            toReturn &= this.Address == obj.Address;
            toReturn &= this.Area == obj.Area;
            toReturn &= this.Value?.ToString().Equals(obj.Value?.ToString()) ?? this.Value == null && obj.Value == null;
            if (this.Children != null && obj.Children != null)
            {
                toReturn &= this.Children.Count == obj.Children.Count ? this.Children.SequenceEqual(obj.Children) : false;
            }
            else
            {
                toReturn &= this.Children == null && obj.Children == null;
            }
            return toReturn;
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only).GetHashCode();
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only, Value, Children).GetHashCode();
        }
    }
}
