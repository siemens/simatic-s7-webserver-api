// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes
{
    /// <summary>
    /// Helper class for special PLC S5Time DataType
    /// </summary>
    public class ApiS5Time
    {
        private int _basis;
        /// <summary>
        /// Base of the displayment in the response
        /// </summary>
        public int Basis
        {
            get
            {
                return _basis;
            }
            set
            {
                if (value == 10 || value == 100 || value == 1000 || value == 10000)
                {
                    _basis = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Basis));
                }
            }
        }

        private int _value;
        /// <summary>
        /// Value of the requested var
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value >= 0 && value <= 999)
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Value));
                }
            }
        }

        /// <summary>
        /// C'tor for Helperclass
        /// </summary>
        /// <param name="basis">Base of the displayment in the response</param>
        /// <param name="value">Value of the requested var</param>
        public ApiS5Time(int basis, int value)
        {
            this.Basis = basis;
            this.Value = value;
        }

        /// <summary>
        /// Basic C'tor => Basis, Value = 0
        /// </summary>
        public ApiS5Time()
        {
            this.Basis = 10;
            this.Value = 0;
        }

        /// <summary>
        /// Represents the smallest possible value of S5Time. This field is constant.
        /// </summary>
        public static readonly ApiS5Time MinValue = new ApiS5Time(10, 0);

        /// <summary>
        /// Represents the biggest possible value of S5Time. This field is constant.
        /// </summary>
        public static readonly ApiS5Time MaxValue = new ApiS5Time(10000, 999);

        /// <summary>
        /// Get the According TimeSpan to the ApiS5Time
        /// </summary>
        /// <returns>According TimeSpan to the ApiS5Time</returns>
        public TimeSpan GetTimeSpan()
        {
            return TimeSpan.FromMilliseconds(this.Basis * this.Value);
        }

        /// <summary>
        /// C'tor that takes a TimeSpan and converts it to an S5Time value
        /// </summary>
        /// <param name="timeSpan"></param>
        public ApiS5Time(TimeSpan timeSpan)
        {
            // Basis 10 val 1 => 10*1 => 10ms, 10ms schritte minimum        bis 9S990MS: Bais 10 Value 999, dann
            // Basis 100 Val100: 10S; Value 101: 10S 100MS 100ms Schritte   bis 99S_900MS : Basis 100 Value 999, dann
            // Basis 1000 Val 100: 100S (1Min40S); 1S Schritte              bis 16M_39S = 999S: Basis 1000 Value 999, dann
            // Basis 10000 Val 100: 16M_40S (1000S); 10S Schritte           bis 2H_46M_30S = 9990Sek: Basis 10000 Value 999, dann Limit
            //int completeMs = (int)timeSpan.TotalMilliseconds;
            if (timeSpan >= TimeSpan.FromMilliseconds(0) && timeSpan < TimeSpan.FromSeconds(10))
            {
                Basis = 10;
                Value = (int)(timeSpan.TotalMilliseconds / (double)Basis);
            }
            else if (timeSpan >= TimeSpan.FromSeconds(10) && timeSpan < TimeSpan.FromSeconds(100))
            {
                Basis = 100;
                Value = (int)((timeSpan.TotalMilliseconds / (double)Basis));
            }
            else if (timeSpan >= TimeSpan.FromSeconds(100) && timeSpan < TimeSpan.FromSeconds(1000))
            {
                Basis = 1000;
                Value = (int)(timeSpan.TotalMilliseconds / (double)Basis);
            }
            else if (timeSpan >= TimeSpan.FromSeconds(1000) && timeSpan < TimeSpan.FromSeconds(10000))
            {
                Basis = 10000;
                Value = (int)(timeSpan.TotalMilliseconds / (double)Basis);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(timeSpan));
            }
            if (GetTimeSpan() != timeSpan)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSpan));
            }
        }

        /// <summary>
        /// Used for the comparison => if basis and value match => true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiS5Time);

        /// <summary>
        /// Used for the comparison => if basis and value match => true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ApiS5Time other)
        {
            if (other == null)
                return false;
            return Basis == other.Basis && Value == other.Value;
        }

        /// <summary>
        /// GetHashCode => (Basis,Value).GetHashCode();
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Basis, Value).GetHashCode();
        }
    }
}
