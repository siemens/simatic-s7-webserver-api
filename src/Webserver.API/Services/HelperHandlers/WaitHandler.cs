// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Diagnostics;
using System.Threading;

namespace Siemens.Simatic.S7.Webserver.API.Services.HelperHandlers
{
    /// <summary>
    /// Wait for Condition to be true
    /// </summary>
    internal class WaitHandler
    {
        /// <summary>
        /// Timeout
        /// </summary>
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// Cycle time
        /// </summary>
        public TimeSpan CycleTime { get; set; }

        /// <summary>
        /// Wait for Timeout
        /// </summary>
        /// <param name="timeOut"></param>
        /// <param name="cycleTime">time until next check if condition is met</param>
        public WaitHandler(TimeSpan timeOut, TimeSpan? cycleTime = null)
        {
            TimeOut = timeOut;
            CycleTime = cycleTime ?? TimeSpan.FromMilliseconds(50);
        }

        /// <summary>
        /// Wait for a condition to become true
        /// </summary>
        /// <param name="Value">Value that needs to become true</param>
        /// <param name="errorMessageForException">error message for the excption</param>
        /// <returns>Value true</returns>
        public TimeSpan ForTrue(Func<bool> Value, string errorMessageForException = "")
        {
            return WaitForCondition(() =>
            {
                if (!Value()) throw new Exception();
            }, TimeOut, CycleTime, errorMessageForException);
        }

        /// <summary>
        /// Wait for a custom condition to be met
        /// </summary>
        /// <param name="Condition">Custom condition to wait for</param>
        /// <param name="TimeOut">Timeout</param>
        /// <param name="CycleTime">Cycle time</param>
        /// <param name="errorMessageForException">error message for the excption</param>
        /// <returns></returns>
        public TimeSpan WaitForCondition(Action Condition, TimeSpan TimeOut, TimeSpan CycleTime, string errorMessageForException = "")
        {
            var sw = new Stopwatch();
            sw.Start();
            var start = DateTime.UtcNow;
            while (!(DateTime.UtcNow.Subtract(start) > TimeOut))
            {
                try
                {
                    // Condition
                    Condition.Invoke();
                    return sw.Elapsed;
                }
                catch (Exception) { }
                // Cylcle time
                Thread.Sleep(CycleTime);
            }
            throw new TimeoutException($"{DateTime.Now}: Could not successfully wait for the {nameof(Condition)} to be applied within {TimeOut}!{Environment.NewLine}Retried every {CycleTime}!{Environment.NewLine}{errorMessageForException}");
        }
    }
}