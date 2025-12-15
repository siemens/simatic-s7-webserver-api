// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public ApiPlcProgramDataType Datatype
        {
            get { return dataType; }
            set
            {
                if (value == ApiPlcProgramDataType.None)
                    throw new ApiInvalidResponseException($"Api PlcProgramDataType:{value} was invalid!");
                dataType = value;
            }
        }

        private List<ApiPlcProgramDataArrayIndexer> _arrayDimensions;
        /// <summary>
        /// Array Dimensions - in case the Data is an array this fills the indexers.
        /// </summary>
        [JsonProperty(Order = 1)]
        public List<ApiPlcProgramDataArrayIndexer> Array_dimensions
        {
            get
            {
                return _arrayDimensions;
            }
            set
            {
                _arrayDimensions = value ?? throw new ArgumentNullException(nameof(Array_dimensions));
                this.ArrayElements = BuildChildrenFromArrayDimensions(_arrayDimensions);
            }
        }

        /// <summary>
        /// Build Children from Array Dimensions of the given Array
        /// </summary>
        /// <param name="arrayDimensions">Array Dimensions of the given Array</param>
        /// <returns>Children for the given Array</returns>
        public List<ApiPlcProgramData> BuildChildrenFromArrayDimensions(List<ApiPlcProgramDataArrayIndexer> arrayDimensions)
        {
            List<ApiPlcProgramData> result = new List<ApiPlcProgramData>();
            var amountOfElements = GetAmountOfElements(arrayDimensions);
            Queue<ApiPlcProgramDataArrayIndexer> queue = new Queue<ApiPlcProgramDataArrayIndexer>();
            foreach (var arrayDimension in arrayDimensions)
            {
                queue.Enqueue(arrayDimension);
            }
            var arrayDimensionStringsToAdd = BuildArrayDimensionStringWithBraces(queue);
            foreach (var childString in arrayDimensionStringsToAdd)
            {
                var toAdd = this.ShallowCopy();
                toAdd.Name = toAdd.Name + childString;
                toAdd.IsArrayElement = true;
                toAdd.Parents = this.Parents;
                toAdd.Children = this.Children;
                // dont! toAdd.ArrayElements = this.ArrayElements;
                result.Add(toAdd);
            }
            if (amountOfElements != result.Count)
            {
                throw new Exception($"Array Element count: {result.Count} does not match calculated count:{amountOfElements}");
            }
            return result;
        }
        /// <summary>
        /// Build the Array Dimension string for all children from the given ArrayDimensions
        /// </summary>
        /// <param name="arrayDimensions"></param>
        /// <returns></returns>
        public List<string> BuildArrayDimensionStringWithBraces(Queue<ApiPlcProgramDataArrayIndexer> arrayDimensions) => (BuildArrayDimensionString(arrayDimensions, new List<string>())).Select(
            returnString =>
            {
                return returnString = "[" + returnString.Substring(0, returnString.Length - 1) + "]";
            })
            .ToList();
        /// <summary>
        /// Build a List of string containing the array dimension Fields
        /// </summary>
        /// <param name="arrayDimensions">Array Dimensions for which the String should be built</param>
        /// <param name="currentReturnString">The function works recursively so the "progress" will be stored in this parameter</param>
        /// <returns></returns>
        private List<string> BuildArrayDimensionString(Queue<ApiPlcProgramDataArrayIndexer> arrayDimensions, List<string> currentReturnString)
        {
            List<string> toReturnString = new List<string>();
            var arrDim = arrayDimensions.Dequeue();
            if (currentReturnString.Count != 0)
            {
                foreach (var currentStr in currentReturnString)
                {
                    for (int j = 0; j < arrDim.Count; j++)
                    {
                        toReturnString.Add(currentStr + (arrDim.Start_index + j).ToString() + ",");
                    }
                }
            }
            else
            {
                for (int j = 0; j < arrDim.Count; j++)
                {
                    toReturnString.Add((arrDim.Start_index + j).ToString() + ",");
                }
            }
            if (arrayDimensions.Count != 0)
            {
                return BuildArrayDimensionString(arrayDimensions, toReturnString);
            }
            else
            {
                return toReturnString;
            }
        }
        /// <summary>
        /// Get Amount of Elements with given array Dimensions
        /// </summary>
        /// <param name="arrayDimensions">Array Dimensions of the Array</param>
        /// <returns>Amount of Elements overall</returns>
        public int GetAmountOfElements(List<ApiPlcProgramDataArrayIndexer> arrayDimensions)
        {
            int result = 1;
            foreach (var arrayDimension in arrayDimensions)
            {
                result = arrayDimension.Count * result;
            }
            return result;
        }

        /// <summary>
        /// Bool that indicates whether "this" was set by ArrayDimensions
        /// </summary>
        public bool IsArrayElement { get; set; }
        /// <summary>
        /// Elements build from the given Array Dimensions
        /// </summary>
        public List<ApiPlcProgramData> ArrayElements { get; set; }

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
            this.ArrayElements = new List<ApiPlcProgramData>();
        }

        /// <summary>
        /// Returns the Name with Quotes => $"\"{Name}\""
        /// </summary>
        /// <returns></returns>
        public string GetNameWithQuotes()
        {
            return IsArrayElement ?
                             $"\"{Name.Substring(0, (Name.LastIndexOf('[')))}\"{Name.Substring(Name.LastIndexOf("["))}" :
                             $"\"{Name}\"";
        }

        /// <summary>
        /// Get Var Name for Methods => all parents: Name with Quotes ("DB".) and dot
        /// </summary>
        /// <returns></returns>
        public string GetVarNameForMethods()
        {
            StringBuilder varName = new StringBuilder();
            if (this.Parents != null)
            {
                foreach (var parent in this.Parents)
                {
                    varName.Append(parent.GetNameWithQuotes() + ".");
                }
            }
            varName.Append(this.GetNameWithQuotes());
            return varName.ToString();
        }

        /// <summary>
        /// return the Serialized Object
        /// </summary>
        /// <returns>Json Object String of the Data</returns>
        public string GetObjectString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                ,
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
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiPlcProgramData);

        /// <summary>
        /// Equals => (Name,Has_children,Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only, Value, Children)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ApiPlcProgramData obj)
        {
            if (obj is null)
                return false;
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
                toReturn &= this.Children.Count == obj.Children.Count && this.Children.SequenceEqual(obj.Children);
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
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only, Value, Children).GetHashCode();
        }
    }
}
