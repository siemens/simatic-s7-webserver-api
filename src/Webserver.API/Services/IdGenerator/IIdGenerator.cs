// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.IdGenerator
{
    /// <summary>
    /// Class to Generate a random string containing the characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Get A Random String with the length given, Random (optional)
        /// containing characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
        /// </summary>
        /// <param name="length">length of the string you want</param>
        /// <returns>random string with given length</returns>
        string Generate(int length);
    }
}