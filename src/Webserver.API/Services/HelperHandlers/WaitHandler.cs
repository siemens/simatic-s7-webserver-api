// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
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
        /// Logger for the WaitHandler
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Wait for Timeout
        /// </summary>
        /// <param name="timeOut"></param>
        /// <param name="cycleTime">time until next check if condition is met</param>
        /// <param name="logger">Logger for the wait handler</param>
        public WaitHandler(TimeSpan timeOut, TimeSpan? cycleTime = null, ILogger logger = null)
        {
            TimeOut = timeOut;
            CycleTime = cycleTime ?? TimeSpan.FromMilliseconds(50);
            Logger = logger;
        }

        /// <summary>
        /// Wait for a condition to become true
        /// </summary>
        /// <param name="Value">Value that needs to become true</param>
        /// <param name="errorMessageForException">error message for the excption</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value true</returns>
        public TimeSpan ForTrue(Func<bool> Value, string errorMessageForException = "", CancellationToken cancellationToken = default)
        {
            return WaitForCondition(() =>
            {
                if (!Value()) throw new ConditionNotYetReachedException();
            }, TimeOut, CycleTime, errorMessageForException, cancellationToken);
        }

        /// <summary>
        /// Wait for a custom condition to be met
        /// </summary>
        /// <param name="Condition">Custom condition to wait for</param>
        /// <param name="timeOut">Timeout</param>
        /// <param name="cycleTime">Cycle time</param>
        /// <param name="errorMessageForException">error message for the excption</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public TimeSpan WaitForCondition(Action Condition, TimeSpan timeOut, TimeSpan cycleTime, string errorMessageForException = "", CancellationToken cancellationToken = default)
        {
            var sw = new Stopwatch();
            sw.Start();
            var start = DateTime.UtcNow;
            Exception lastException = null;
            while (!(DateTime.UtcNow.Subtract(start) > timeOut))
            {
                cancellationToken.ThrowIfCancellationRequested();
                // https://github.com/nunit/nunit/issues/2040
                try
                {
                    // Condition
                    Condition.Invoke();
                    return sw.Elapsed;
                }
                catch (ConditionNotYetReachedException) { }
                catch (Exception e)
                {
                    if (e is OutOfMemoryException || e is StackOverflowException || e is ThreadAbortException)
                        throw;
                    Logger?.LogDebug(e, $"While waiting for a condition!");
                    lastException = e;
                }
                // skip last sleep when not necessary - avoid overshooting the timeout, also good for unit tests
                var timeSpentSinceStartingWaitForCondition = DateTime.UtcNow.Subtract(start);
                var timeSpentAfterSleeping = timeSpentSinceStartingWaitForCondition.Add(cycleTime);
                if (timeSpentAfterSleeping > timeOut)
                {
                    Logger?.LogTrace($"Skipping sleep since {timeSpentAfterSleeping} will be bigger than configured timeout: {timeOut}!");
                    break;
                }
                // Cylcle time
                Thread.Sleep(cycleTime);
            }
            var exc = new TimeoutException($"{DateTime.Now}: Could not successfully wait for the {nameof(Condition)} to be applied within {timeOut}!{Environment.NewLine}Retried every {cycleTime}!{Environment.NewLine}{errorMessageForException}", lastException);
            Logger?.LogError(exc, $"trying to {nameof(WaitForCondition)}!");
            throw exc;
        }

        private class ConditionNotYetReachedException : Exception { }
    }
}