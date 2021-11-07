// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.IdGenerator
{
    /// <summary>
    /// Class to Generate a random string of the given length - by default containing the characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
    /// </summary>
    public class CharSetIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Character Set used for the Id Generation
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// Time to Sleep After Request Id Generation
        /// </summary>
        public TimeSpan ThreadSleepTime { get; set; }

        /// <summary>
        /// Time to Sleep After Request Id Generation
        /// </summary>
        public TimeSpan DeterminedThreadSleepTime { get; }

        /// <summary>
        /// Class to Generate a random string of the given length - by default containing the characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
        /// </summary>
        public CharSetIdGenerator()
        {
            CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            DeterminedThreadSleepTime = DetermineThreadSleepTime();
            ThreadSleepTime = DeterminedThreadSleepTime;
        }

        public TimeSpan DetermineThreadSleepTime()
        {
            List<TimeSpan> MillisecondsDetermined = new List<TimeSpan>();
            for(int i = 0; i < 3; i++)
            {
                DateTime start = DateTime.Now;
                var generated = Generate(8);
                var secGenerated = Generate(8);
                while (secGenerated == generated)
                {
                    secGenerated = Generate(8);
                }
                var end = DateTime.Now;
                MillisecondsDetermined.Add(end - start);
            }
            TimeSpan result = TimeSpan.FromSeconds(0);
            foreach(var element in MillisecondsDetermined)
            {
                if(element > result)
                {
                    result = element;
                }
            }
            return result;
        }
        /// <summary>
        /// Class to Generate a random string of the given length
        /// </summary>
        /// <param name="charSet"></param>
        public CharSetIdGenerator(string charSet)
        {
            CharSet = charSet;
        }
        /// <summary>
        /// Get A Random String with the length given, Random (optional)
        /// containing characters of CharSet
        /// </summary>
        /// <param name="length">length of the string you want</param>
        /// <returns>random string with given length</returns>
        public string Generate(int length)
        {
            if (length < 0)
            {
                throw new ArgumentException(nameof(length) + " must be greater than 0!");
            }
            var random = new Random();
            var result = new string(Enumerable.Repeat(CharSet, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            Thread.Sleep(ThreadSleepTime);
            return result;
        }
    }
}
