﻿// Copyright (c) 2021, Siemens AG
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
        /// Length for the request(s) generated
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Default value for Length (8)
        /// </summary>
        public int DefaultLength { get { return 8; } }

        /// <summary>
        /// Class to Generate a random string of the given length - by default containing the characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
        /// </summary>
        public CharSetIdGenerator()
        {
            CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Length = DefaultLength;
            DeterminedThreadSleepTime = DetermineThreadSleepTime();
            ThreadSleepTime = DeterminedThreadSleepTime;
        }
        /// <summary>
        /// Class to Generate a random string of the given length
        /// </summary>
        /// <param name="charSet">Character Set used for the Id Generation</param>
        public CharSetIdGenerator(string charSet) : this()
        {
            CharSet = charSet;
        }

        /// <summary>
        /// Class to Generate a random string of the given length
        /// </summary>
        /// <param name="charSet">Character Set used for the Id Generation</param>
        /// <param name="length">Length for the request ids to generate</param>
        public CharSetIdGenerator(string charSet, int length) : this()
        {
            CharSet = charSet;
            Length = length;
        }

        /// <summary>
        /// Function to determine the ThreadSleepTime for your processor
        /// </summary>
        /// <returns>the timespan your processor "needs to sleep" in between requests so that a new 
        /// "random" request id will be generated</returns>
        public TimeSpan DetermineThreadSleepTime()
        {
            List<TimeSpan> MillisecondsDetermined = new List<TimeSpan>();
            for(int i = 0; i < 3; i++)
            {
                DateTime start = DateTime.Now;
                var generated = Generate();
                var secGenerated = Generate();
                while (secGenerated == generated)
                {
                    secGenerated = Generate();
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
        /// Get A Random String with the length given, Random (optional)
        /// containing characters of CharSet
        /// </summary>
        /// <param name="length">length of the string you want</param>
        /// <returns>random string with given length</returns>
        public string Generate()
        {
            if (Length < 0)
            {
                throw new ArgumentException(nameof(Length) + " must be greater than 0!");
            }
            var random = new Random();
            var result = new string(Enumerable.Repeat(CharSet, Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            Thread.Sleep(ThreadSleepTime);
            return result;
        }
    }
}
