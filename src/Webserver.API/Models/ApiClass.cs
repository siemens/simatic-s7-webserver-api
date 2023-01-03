// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// A Class that will be returned in some ApiFunctions - contains the information of the Name! - Equals uses that Name to determine wether they are equal or not1
    /// </summary>
    public class ApiClass
    {
        /// <summary>
        /// ApiClassName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A Class that will be returned in some ApiFunctions - contains the information of the Name! - Equals uses that Name to determine wether they are equal or not1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the name matches the "other".Name</returns>
        public override bool Equals(object obj)
        {
            var @class = obj as ApiClass;
            return @class != null &&
                   Name == @class.Name;
        }

        /// <summary>
        /// Will get the HashCode for the only Property which is Name
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (Name).GetHashCode();

        /// <summary>
        /// Return the Name
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
