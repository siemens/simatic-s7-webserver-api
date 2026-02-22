// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Services.PlcProgram.Subscription
{
    /// <summary>
    /// Event arguments for monitored item value changes.
    /// </summary>
    public class MonitoredItemValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The monitored item that changed.
        /// </summary>
        public ApiPlcProgramData Item { get; }

        /// <summary>
        /// The previous value before the change.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// The new value after the change.
        /// </summary>
        public object NewValue { get; }

        /// <summary>
        /// Creates a new instance of MonitoredItemValueChangedEventArgs.
        /// </summary>
        public MonitoredItemValueChangedEventArgs(ApiPlcProgramData item, object oldValue, object newValue)
        {
            Item = item;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
