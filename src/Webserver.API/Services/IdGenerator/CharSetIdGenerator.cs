﻿// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        public readonly string CharSet;

        /// <summary>
        /// Time to Sleep After Request Id Generation
        /// </summary>
        public readonly TimeSpan ThreadSleepTime;

        /// <summary>
        /// Time to Sleep After Request Id Generation
        /// </summary>
        public readonly TimeSpan DeterminedThreadSleepTime;
        /// <summary>
        /// Length for the request(s) generated
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Default value for Length (36)
        /// </summary>
        public int DefaultLength { get { return 36; } }

        /// <summary>
        /// Create a charsetgenerator with default values
        /// </summary>
        public CharSetIdGenerator()
        {
            CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Length = DefaultLength;
            DeterminedThreadSleepTime = DetermineThreadSleepTime();
            ThreadSleepTime = DeterminedThreadSleepTime;
        }

        /// <summary>
        /// Create a charsetgenerator with userdefined values
        /// </summary>
        /// <param name="charSet">charset to be used</param>
        /// <param name="length">Length of the Id to be generated</param>
        public CharSetIdGenerator(string charSet, int length) : this()
        {
            CheckCharSet(charSet);
            CharSet = charSet;
            CheckLength(length);
            Length = length;
        }

        /// <summary>
        /// Create a charsetgenerator with userdefined values
        /// </summary>
        /// <param name="charSet">charset to be used</param>
        /// <param name="threadSleepTime">time to sleep after generate calls</param>
        /// <param name="length">Length of the Id to be generated</param>
        public CharSetIdGenerator(string charSet, TimeSpan threadSleepTime,
            int length) : this()
        {
            ThreadSleepTime = threadSleepTime;
            CheckCharSet(charSet);
            CharSet = charSet;
            CheckLength(length);
            Length = length;
        }

        private void CheckCharSet(string charSet)
        {
            if (string.IsNullOrEmpty(charSet))
            {
                throw new ArgumentOutOfRangeException(nameof(charSet));
            }
        }

        private void CheckLength(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
        }



        /// <summary>
        /// Function to determine the ThreadSleepTime for your processor
        /// </summary>
        /// <returns>the timespan your processor "needs to sleep" in between requests so that a new 
        /// "random" request id will be generated</returns>
        public TimeSpan DetermineThreadSleepTime()
        {
            List<TimeSpan> MillisecondsDetermined = new List<TimeSpan>();
            for (int i = 0; i < 3; i++)
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
            foreach (var element in MillisecondsDetermined)
            {
                if (element > result)
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
        /// <returns>random string with given length</returns>
        public string Generate()
        {
            var random = new Random();
            var result = new string(Enumerable.Repeat(CharSet, Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            Thread.Sleep(ThreadSleepTime);
            return result;
        }
    }
}
