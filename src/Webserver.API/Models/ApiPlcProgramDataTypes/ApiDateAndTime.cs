// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes
{
    /// <summary>
    /// Date and Time Class For Usage in PlcProgram.Write or as T in Read
    /// </summary>
    public class ApiDateAndTime
    {
        private int _year;
        /// <summary>
        /// Year of the Date And Time Tag
        /// </summary>
        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                CheckValidity(nameof(Year));
            }
        }
        private int _month;
        /// <summary>
        /// Month of the Date And Time Tag
        /// </summary>
        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                CheckValidity(nameof(Month));
            }
        }
        private int _day;
        /// <summary>
        /// Day of the Date And Time Tag
        /// </summary>
        public int Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                CheckValidity(nameof(Day));
            }
        }
        private int _hour;
        /// <summary>
        /// Hour of the Date And Time Tag
        /// </summary>
        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                CheckValidity(nameof(Hour));
            }
        }
        private int _minute;
        /// <summary>
        /// Minute of the Date And Time Tag
        /// </summary>
        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                _minute = value;
                CheckValidity(nameof(Minute));
            }
        }
        private double _second;
        /// <summary>
        /// Second of the Date And Time Tag
        /// </summary>
        public double Second
        {
            get
            {
                return _second;
            }
            set
            {
                _second = value;
                CheckValidity(nameof(Second));
            }
        }
        /// <summary>
        /// Default c'tor => 01.01.1990 00:00:00
        /// </summary>
        public ApiDateAndTime()
        {
            _year = 1990;
            _month = 1;
            _day = 1;
            _hour = 0;
            _minute = 0;
            _second = 0;
        }

        /// <summary>
        /// C'tor to provide year, month, day, hour, minute and second
        /// </summary>
        /// <param name="year">year to set</param>
        /// <param name="month">month to set</param>
        /// <param name="day">day to set</param>
        /// <param name="hour">hour to set</param>
        /// <param name="minute">minute to set</param>
        /// <param name="second">second to set</param>
        public ApiDateAndTime(int year, int month, int day, int hour, int minute, double second)
            : this((new DateTime(year, month, day, hour, minute, (int)second)).AddMilliseconds((second - (int)second) * 1000))
        {
        }

        /// <summary>
        /// Represents the biggest possible value of ApiDateAndTime. This field is constant.
        /// </summary>
        public static readonly ApiDateAndTime MaxValue = new ApiDateAndTime() { _year = 2089, _month = 12, _day = 31, _hour = 23, _minute = 59, _second = 59.999 };


        /// <summary>
        /// Represents the smallest possible value of ApiDateAndTime. This field is constant.
        /// </summary>
        public static readonly ApiDateAndTime MinValue = new ApiDateAndTime() { _year = 1990, _month = 1, _day = 1, _hour = 0, _minute = 0, _second = 0.0 };

        /// <summary>
        /// Set an ApiDateAndTime object according to the DateTime provided. 
        /// </summary>
        /// <param name="dateTime">Datetime that the values should be set from</param>
        public ApiDateAndTime(DateTime dateTime)
        {
            this._year = dateTime.Year;
            this._month = dateTime.Month;
            this._day = dateTime.Day;
            this._hour = dateTime.Hour;
            this._minute = dateTime.Minute;
            this._second = dateTime.Second + ((double)dateTime.Millisecond / 1000);
            CheckValidity(nameof(dateTime));
        }

        private void CheckValidity(string argumentName)
        {
            var thisDateTime = this.GetDateTime();
            if (thisDateTime > MaxValue.GetDateTime() || thisDateTime < MinValue.GetDateTime())
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Will compare the values: Year,Month,Day,Hour,Minute,Second and return true if they match!
        /// </summary>
        /// <param name="obj">will be used to call Equals with obj as ApiDateAndTime</param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiDateAndTime);

        /// <summary>
        /// Get the DateTime representative of this ApiDateAndTime object.
        /// </summary>
        /// <returns>DateTime representative of this ApiDateAndTime object.</returns>
        public DateTime GetDateTime()
        {
            var dateTimeToReturn = new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, (int)this.Second);
            return dateTimeToReturn.AddMilliseconds((this.Second - (int)this.Second) * 1000);
        }

        /// <summary>
        /// Will compare the values: Year,Month,Day,Hour,Minute,Second and return true if they match!
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ApiDateAndTime other)
        {
            if (other is null)
                return false;
            return this.Year == other.Year && this.Month == other.Month && this.Day == other.Day && this.Hour == other.Hour
                && this.Minute == other.Minute && this.Second == other.Second;
        }

        /// <summary>
        /// return (Year,Month,Day,Hour,Minute,Second).GetHashCode();
        /// </summary>
        /// <returns>(Year,Month,Day,Hour,Minute,Second).GetHashCode();</returns>
        public override int GetHashCode()
        {
            return (Year, Month, Day, Hour, Minute, Second).GetHashCode();
        }
    }
}
