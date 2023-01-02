// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Plc.ApiPlcReadTimeSettingsResult result
    /// </summary>
    public class ApiPlcReadTimeSettingsResult
    {
        /// <summary>
        /// Offset to the current time => Utc_offset + daylight savings time offset if currently applied
        /// </summary>
        public int Current_offset { get; set; }


        /// <summary>
        /// ISO formatted Offset to the current time => Utc_offset + daylight savings time offset if currently applied
        /// </summary>
        public string ISO_Current_offset
        {
            get
            {
                return IsoFormatter.SetISOFormat(Current_offset);
            }
        }

        /// <summary>
        /// fixed offset to utc
        /// </summary>
        public int Utc_offset { get; set; }


        /// <summary>
        /// ISO formatted offset to utc
        /// </summary>
        public string ISO_Utc_offset
        {
            get
            {
                return IsoFormatter.SetISOFormat(Utc_offset);
            }
        }

        /// <summary>
        /// Current Daylight savings rule
        /// </summary>
        public DaylightSavingsRule Rule { get; set; } //optional

        /// <summary>
        /// Can be used to comfortly get the current time for a given timestampf of a Plc.ReadSystemTime result
        /// </summary>
        /// <param name="plcReadSystemTimeResult">Timestamp of result of Plc.ReadSystemTime (UTC)</param>
        /// <returns></returns>
        public DateTime GetCurrentTime(DateTime plcReadSystemTimeResult)
        {
            return (plcReadSystemTimeResult + TimeSpan.FromMinutes(Current_offset));
        }

        /// <summary>
        /// Can be used to comfortly get the current time for a given Plc.ReadSystemTime result
        /// </summary>
        /// <param name="plcReadSystemTimeResult">Result of Plc.ReadSystemTime (UTC)</param>
        /// <returns></returns>
        public DateTime GetCurrentTime(ApiPlcReadSystemTimeResult plcReadSystemTimeResult)
        {
            return (plcReadSystemTimeResult.Timestamp + TimeSpan.FromMinutes(Current_offset));
        }
    }

    /// <summary>
    /// Daylight savings rule
    /// </summary>
    public class DaylightSavingsRule
    {
        /// <summary>
        /// Start of Standard time
        /// </summary>
        public StandardTimeConfiguration Std { get; set; }

        /// <summary>
        /// Start of Daylight saving time
        /// </summary>
        public DaylightSavingsTimeConfiguration Dst { get; set; }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only) Equal!;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as DaylightSavingsRule);

        /// <summary>
        /// Equals => (Name,Has_children,Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only, Value, Children)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(DaylightSavingsRule obj)
        {
            if (obj is null)
                return false;
            var toReturn = this.Std.Equals(obj.Std);
            toReturn &= this.Dst.Equals(obj.Dst);
            return toReturn;
        }

        /// <summary>
        /// (Name, Has_children, Db_number, Datatype, Array_dimensions, Max_length, Address, Area, Read_only).GetHashCode();
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Std, Dst).GetHashCode();
        }
    }

    /// <summary>
    /// Standard time configuration containing the start date of the standard time
    /// </summary>
    public class StandardTimeConfiguration
    {
        /// <summary>
        /// PlcDate of the StartTime of the standard time or daylight savings time
        /// </summary>
        public PlcDate Start { get; set; }

        /// <summary>
        /// PlcDate to determine the start of the standard or daylight savings time configuration
        /// </summary>
        public class PlcDate
        {
            /// <summary>
            /// Minute
            /// </summary>
            public int Minute { get; set; }
            /// <summary>
            /// Hour
            /// </summary>
            public int Hour { get; set; }
            /// <summary>
            /// Week in month;
            /// Speciality: Week 5 always stands for the last Week in the according month.
            /// </summary>
            public int Week { get; set; }
            /// <summary>
            /// Month
            /// </summary>
            public int Month { get; set; }
            /// <summary>
            /// first three letters of the according day of week (lowercase)
            /// </summary>
            public string Day_of_week { get; set; }
        }
    }

    /// <summary>
    /// Daylight savings time configuration containing the start date of the daylight savings time and the offset during the daylight savings time
    /// </summary>
    public class DaylightSavingsTimeConfiguration : StandardTimeConfiguration
    {
        /// <summary>
        /// Offset during the daylight savings time
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// ISO formatted Offset during the daylight savings time
        /// </summary>
        public string ISO_Offset
        {
            get
            {
                return IsoFormatter.SetISOFormat(Offset);
            }
        }
    }

    /// <summary>
    /// Helper class for get the ISO formatted offsets.
    /// </summary>
    public static class IsoFormatter
    {
        /// <summary>
        /// Get the ISO formatted offset
        /// </summary>
        /// <param name="minute"></param>
        /// <param name="hour"></param>
        /// <returns>ISO formatted offset if minutes and/or hours bigger than 0 - otherwise ""</returns>
        public static string SetISOFormat(int minute = 0, int hour = 0)
        {
            string iso = "";
            if (hour > 0 || minute > 0)
            {
                iso = "PT";
                if (minute >= 60)
                {
                    hour += (minute / 60);
                    if (minute % 60 == 0)
                    {
                        minute = 0;
                    }
                    else
                    {
                        minute = (minute % 60);
                    }
                }
                if (hour > 0)
                {
                    iso += hour + "H";
                }

                if (minute > 0)
                {
                    iso += minute + "M";
                }
            }
            return iso;
        }
    }
}