// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes;
using Siemens.Simatic.S7.Webserver.API.Services.WebApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.API.UnitTests
{
    public class ModelTests : Base
    {
        [Test]
        public void ResourceNullChecks()
        {
            var resource = new ApiWebAppResource();
            resource = null;
            if(resource == null)
            {
                Console.WriteLine("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            if(resource != null)
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            resource = new ApiWebAppResource();
            if (resource.Equals(null))
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            if(!resource.Equals(null))
            {
                Console.Write("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
        }

        [Test]
        public void WebAppNullChecks()
        {
            var app = new ApiWebAppData();
            app = null;
            if (app == null)
            {
                Console.WriteLine("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            if (app != null)
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            app = new ApiWebAppData();
            if (app.Equals(null))
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            if (!app.Equals(null))
            {
                Console.Write("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - resource not null altough its null!");
            }
        }

        [Test]
        public void ApiS5TimeNullChecks()
        {
            var s5Time = new ApiS5Time(10,1);
            s5Time = null;
            if (s5Time == null)
            {
                Console.WriteLine("works!");
            }
            else
            {
                Assert.Fail("S5TimeNullChecks - s5Time not null altough its null!");
            }
            if (s5Time != null)
            {
                Assert.Fail("S5TimeNullChecks - s5Time not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            s5Time = new ApiS5Time(10,1);
            if (s5Time.Equals(null))
            {
                Assert.Fail("ResourcNullChecks - s5Time not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            if (!s5Time.Equals(null))
            {
                Console.Write("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - s5Time not null altough its null!");
            }
        }

        [Test]
        public void ApiS5TimeEqualAndOperatorsCheck()
        {
            var s5Time = new ApiS5Time(10, 1);
            var secondS5Time = new ApiS5Time(10, 1);
            if(!s5Time.Equals(secondS5Time))
            {
                Assert.Fail("same values S5time are not equal!");
            }
            if(s5Time == secondS5Time)
            {
                Assert.Fail("unexpectedly s5Time == secondS5Time");
            }
            if(!(s5Time != secondS5Time))
            {
                Assert.Fail("unexpectedly s5Time != secondS5Time is false");
            }
            s5Time = new ApiS5Time(10, 2);
            if (s5Time.Equals(secondS5Time))
            {
                Assert.Fail("other values S5time are equal!");
            }
            s5Time = new ApiS5Time(100, 1);
            if (s5Time.Equals(secondS5Time))
            {
                Assert.Fail("other values S5time are equal!");
            }
        }

        [Test]
        public void ApiDateAndTime_MinMaxVal()
        {
            var MaxValDT = ApiDateAndTime.MaxValue.GetDateTime();
            if (MaxValDT != new DateTime(2089, 12, 31, 23, 59, 59).AddMilliseconds(999))
            {
                Assert.Fail("MaxValue unexpected!");
            }
            var MinValDT = ApiDateAndTime.MinValue.GetDateTime();
            if (MinValDT != new DateTime(1990, 1,1,0,0,0))
            {
                Assert.Fail("MinValue unexpected!");
            }

        }

        [Test]
        public void ApiDateAndTimeEqualAndOperatorsCheck()
        {
            var dateAndTime = new ApiDateAndTime() { Second = 1 };
            var secondDateAndTime = new ApiDateAndTime() { Second = 1 };
            if (!dateAndTime.Equals(secondDateAndTime))
            {
                Assert.Fail("same values ApiDateAndTime are not equal!");
            }
            if (dateAndTime == secondDateAndTime)
            {
                Assert.Fail("unexpectedly ApiDateAndTime == secondDateAndTime");
            }
            if (!(dateAndTime != secondDateAndTime))
            {
                Assert.Fail("unexpectedly ApiDateAndTime != secondDateAndTime is false");
            }
            dateAndTime = new ApiDateAndTime() {Second = 2 };
            if (dateAndTime.Equals(secondDateAndTime))
            {
                Assert.Fail("other values ApiDateAndTime are equal!");
            }
        }

        [Test]
        public void ApiDateAndTimeNullChecks()
        {
            var apiDateAndTime = new ApiDateAndTime();
            apiDateAndTime = null;
            if (apiDateAndTime == null)
            {
                Console.WriteLine("works!");
            }
            else
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }

            if (apiDateAndTime != null)
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            apiDateAndTime = new ApiDateAndTime();
            if (apiDateAndTime.Equals(null))
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }
            else
            {
                Console.Write("works!");
            }
            if (!apiDateAndTime.Equals(null))
            {
                Console.Write("works!");
            }
            else
            {
                Assert.Fail("ResourcNullChecks - apiDateAndTime not null altough its null!");
            }
        }

        


        

        [Test]
        public void ApiDateAndTime_ValuesAreAsExpected()
        {
            var myDate = new ApiDateAndTime();
            if (!(myDate.Year == 1990 && myDate.Month == 1 && myDate.Day == 1 && myDate.Hour == 0 && myDate.Minute == 0 && myDate.Second == 0.0))
            {
                Assert.Fail($"Unexpected date: {myDate}");
            }
            if(!myDate.Equals(ApiDateAndTime.MinValue))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myDate} and {ApiDateAndTime.MinValue}");
            }
            myDate.Month = 12;
            myDate.Day = 31;
            myDate.Year = 2021;
            myDate.Hour = 23;
            myDate.Minute = 59;
            myDate.Second = 59.999;
            var compare = new DateTime(2021, 12, 31, 23, 59, 59).AddMilliseconds(999);
            var myDT = myDate.GetDateTime();
            if (myDT != compare)
            {
                Assert.Fail($"Unexpectedly dates dont match: {myDate} and {ApiDateAndTime.MinValue}");
            }
            var anotherDt = new ApiDateAndTime(new DateTime(2021, 12, 31, 23, 59, 59).AddMilliseconds(999));
            if(!anotherDt.Equals(myDate))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myDate} and {anotherDt}");
            }
        }

        [Test]
        public void ApiDateAndTime_InvalidValuesNotAccepted()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(1989,1,1,0,0,0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(1930, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(2300, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(2090, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0,0, 0.0);
                mydT.Year = 2090;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Year = 1989;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Month = 13;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Month = -1;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Day = 32;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Day = -1;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Second = 61;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Second = -1;
            });
        }

        [Test]
        public void ApiS5Time_ValuesAreAsExpected()
        {
            var myS5Time = new ApiS5Time(10,0);
            if (myS5Time.GetTimeSpan() != TimeSpan.FromMilliseconds(0))
            {
                Assert.Fail($"Unexpected myS5Time: {myS5Time}");
            }
            if (!myS5Time.Equals(ApiS5Time.MinValue))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myS5Time} and {ApiDateAndTime.MinValue}");
            }
            var secondS5Time = new ApiS5Time(TimeSpan.FromMilliseconds(0));
            if(!myS5Time.Equals(secondS5Time))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myS5Time} and {secondS5Time}");
            }
        }

        [Test]
        public void ApiS5Time_InvalidValuesNotAccepted()
        {
            // Basis 10 val 1 => 10*1 => 10ms, 10ms schritte minimum        bis 9S990MS: Bais 10 Value 999, dann
            // Basis 100 Val100: 10S; Value 101: 10S 100MS 100ms Schritte   bis 99S_900MS : Basis 100 Value 999, dann
            // Basis 1000 Val 100: 100S (1Min40S); 1S Schritte              bis 16M_39S = 999S: Basis 1000 Value 999, dann
            // Basis 10000 Val 100: 16M_40S (1000S); 10S Schritte           bis 2H_46M_30S = 9990Sek: Basis 10000 Value 999, dann Limit
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromMilliseconds(5)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(10) + TimeSpan.FromMilliseconds(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(10) + TimeSpan.FromMilliseconds(80)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(100) + TimeSpan.FromMilliseconds(100)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(100) + TimeSpan.FromMilliseconds(800)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(1000) + TimeSpan.FromMilliseconds(1000)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(1000) + TimeSpan.FromMilliseconds(8000)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(TimeSpan.FromSeconds(10000)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(10, 1000));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(5, 50));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiS5Time(100000, 999));
        }

    }
}
