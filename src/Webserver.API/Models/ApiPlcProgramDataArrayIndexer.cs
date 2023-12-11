// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiPlcProgramDataArrayIndexer: In case a ApiPlcProgramData contains the Informations of Start_index and Count => its Array_Dimensions will be filled accordingly
    /// </summary>
    public class ApiPlcProgramDataArrayIndexer
    {
        /// <summary>
        /// Array Dimensions Start_index of the ApiPlcProgramData => where does the Array start
        /// </summary>
        public int Start_index { get; set; }

        /// <summary>
        /// Array Dimensions Count of the ApiPlcProgramData => how many Elements does the Array have
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Compare Start_index and Count
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var indexer = obj as ApiPlcProgramDataArrayIndexer;
            return indexer != null &&
                   Start_index == indexer.Start_index &&
                   Count == indexer.Count;
        }

        /// <summary>
        /// get hashcode for StartIndex and Count
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Start_index, Count).GetHashCode();
        }
    }

}
