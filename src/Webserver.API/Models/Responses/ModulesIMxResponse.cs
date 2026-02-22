// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// Generic interface for strongly-typed IM results  
    /// </summary>
    /// <typeparam name="T">The concrete data type</typeparam>
    public interface IModulesIMxResult<T>
    {
        /// <summary>
        /// The strongly-typed IM data
        /// </summary>
        T Data { get; }
    }

    /// <summary>
    /// Response of Modules.ReadIdentificationMaintenance with IMx data (base class)
    /// </summary>
    public class ModulesIMxResult<T> : IModulesIMxResult<T> where T : class
    {
        /// <summary>
        /// The IM data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesIMxResult<T> result &&
                   EqualityComparer<T>.Default.Equals(Data, result.Data);
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Data).GetHashCode();
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
    /// Response of Modules.ReadIdentificationMaintenance with IMx data (base class)
    /// </summary>
    /// <typeparam name="T">concrete Data class</typeparam>
    public class ModulesIMxResponse<T> : ApiResultResponse<ModulesIMxResult<T>> where T : class
    {

    }
}
