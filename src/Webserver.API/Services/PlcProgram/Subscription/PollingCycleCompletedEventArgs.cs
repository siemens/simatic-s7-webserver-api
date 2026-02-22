// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Services.PlcProgram.Subscription
{
    /// <summary>
    /// Event arguments for polling cycle completion.
    /// </summary>
    public class PollingCycleCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// List of all items that changed during this polling cycle.
        /// </summary>
        public IReadOnlyList<(ApiPlcProgramData item, object oldValue, object newValue)> ChangedItems { get; }

        /// <summary>
        /// Duration of the polling cycle in milliseconds.
        /// </summary>
        public double PollingDurationMs { get; }

        /// <summary>
        /// Timestamp when the polling cycle started.
        /// </summary>
        public DateTime PollingStartTime { get; }

        /// <summary>
        /// Creates a new instance of PollingCycleCompletedEventArgs.
        /// </summary>
        public PollingCycleCompletedEventArgs(List<(ApiPlcProgramData item, object oldValue, object newValue)> changedItems, double pollingDurationMs, DateTime pollingStartTime)
        {
            ChangedItems = changedItems;
            PollingDurationMs = pollingDurationMs;
            PollingStartTime = pollingStartTime;
        }
    }
}
