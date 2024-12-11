// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.TimeSettings
{
    /// <summary>
    /// PlcDate to determine the start of the standard or daylight savings time configuration
    /// </summary>
    public class PlcDate
    {
        private int _minute;
        /// <summary>
        /// Minute
        /// </summary>
        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                if (value >= 0 && value <= 59)
                {
                    _minute = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Valid values of {nameof(Minute)} are 0 through 59. Input value was {value}");
                }
            }
        }
        private int _hour;
        /// <summary>
        /// Hour
        /// </summary>
        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                if (value >= 0 && value <= 23)
                {
                    _hour = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Valid values of {nameof(Hour)} are 0 through 23. Input value was {value}");
                }
            }
        }
        private int _week;
        /// <summary>
        /// Week in month;
        /// Speciality: Week 5 always stands for the last Week in the according month.
        /// </summary>
        public int Week
        {
            get
            {
                return _week;
            }
            set
            {
                if (value >= 1 && value <= 5)
                {
                    _week = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Valid values of {nameof(Week)} are 1 through 5. Input value was {value}");
                }
            }
        }
        private int _month;
        /// <summary>
        /// Month
        /// </summary>
        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
                if (value >= 1 && value <= 12)
                {
                    _month = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Valid values of {nameof(Month)} are 1 through 12. Input value was {value}");
                }
            }
        }
        /// <summary>
        /// First three letters of the according day of week
        /// </summary>
        public ApiDayOfWeek Day_of_week { get; set; }
        /// <summary>
        /// Constructor to create PlcDate object
        /// </summary>
        /// <param name="month">Month, from 1 to 12</param>
        /// <param name="week">Week, from 1 to 5</param>
        /// <param name="day_of_week">Weekday, 1 to 7</param>
        /// <param name="hour">Hour, 0 to 24</param>
        /// <param name="minute">Minute, 0 to 60</param>
        public PlcDate(int month, int week, ApiDayOfWeek day_of_week, int hour, int minute)
        {
            Minute = minute;
            Hour = hour;
            Week = week;
            Month = month;
            Day_of_week = day_of_week;
        }
        /// <summary>
        /// Checks whether incoming object is same as this PlcDate
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they're the same</returns>
        public override bool Equals(object obj)
        {
            return obj is PlcDate date &&
                   Minute == date.Minute &&
                   Hour == date.Hour &&
                   Week == date.Week &&
                   Month == date.Month &&
                   Day_of_week == date.Day_of_week;
        }
        /// <summary>
        /// Get HashCode of PlcDate
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Minute, Hour, Week, Month, Day_of_week).GetHashCode();
        }
    }
}
