// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.StaticHelpers
{
    /// <summary>
    /// Static String Methods to Get a random string e.g.
    /// </summary>
    public static class StaticStringMethods
    {
        /// <summary>
        /// Get A Random String with the length given, Random (optional)
        /// containing characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
        /// </summary>
        /// <param name="length">length of the string you want</param>
        /// <param name="random">if you want to provide a random yourself feel free</param>
        /// <returns></returns>
        public static string GetRandomString(int length, Random random = null)
        {
            if (random == null)
                random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
