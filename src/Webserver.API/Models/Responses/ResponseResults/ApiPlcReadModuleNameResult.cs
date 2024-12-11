// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiPlcReadModuleNameResult
    {
        /// <summary>
        /// The module name of the PLC on which the request is executed.
        /// </summary>
        public string Module_name { get; set; }

        /// <summary>
        /// Compares this to input object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPlcReadModuleNameResult moduleName &&
                   Module_name == moduleName.Module_name;
        }

        /// <summary>
        /// Get Hash Code of PasswordExpiraton object
        /// </summary>
        /// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            return Module_name.GetHashCode();
        }

        /// <summary>
        /// ToString for ApiPlcCpuType
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Module_name)}: {Module_name}";
        }
    }
}
