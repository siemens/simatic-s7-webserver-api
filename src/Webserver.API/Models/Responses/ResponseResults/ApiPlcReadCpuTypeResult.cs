// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Public product information such as the PLC product name and order number.
    /// </summary>
    public class ApiPlcReadCpuTypeResult
    {
        /// <summary>
        /// The product name of the PLC or R/H system on which the request is executed.
        /// </summary>
        public string Product_Name { get; set; }
        /// <summary>
        /// The order number of the PLC or R/H system on which the request is executed.
        /// </summary>
        public string Order_Number { get; set; }

        /// <summary>
        /// Compares this to input object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if objects are the same</returns>
        public override bool Equals(object obj)
        {
            return obj is ApiPlcReadCpuTypeResult cpuType &&
                   Product_Name == cpuType.Product_Name &&
                   Order_Number == cpuType.Order_Number;
        }

        /// <summary>
        /// Get Hash Code of PasswordExpiraton object
        /// </summary>
        /// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            return (Product_Name, Product_Name).GetHashCode();
        }

        /// <summary>
        /// ToString for ApiPlcCpuType
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Product_Name)}: {Product_Name} | " +
                   $"{nameof(Product_Name)}: {Product_Name}";
        }
    }
}
