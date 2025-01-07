// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiSyslog
{
    /// <summary>
    /// A single syslog message of the PLC-internal syslog ring buffer
    /// </summary>
    public class ApiPlcSyslog_Entry
    {
        /// <summary>
        /// A raw syslog entry as defined in the RFC.
        /// </summary>
        public string Raw { get; set; }
        /// <summary>
        /// Check whether properties match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the ApiSyslog_Entries are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiPlcSyslog_Entry;
            if (structure is null)
            {
                return false;
            }
            return structure.Raw == this.Raw;
        }

        /// <summary>
        /// GetHashCode for ApiSyslog_Entry
        /// </summary>
        /// <returns>The hash code of the ApiSyslog_Entry</returns>
        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        /// <summary>
        /// ToString for ApiSyslog_Entry
        /// </summary>
        /// <returns>Raw</returns>
        public override string ToString()
        {
            return Raw;
        }
    }
}
