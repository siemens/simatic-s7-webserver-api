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
        public void ApiWebAppResource_NullEqualAndSequenceEqual_AsExpected()
        {
            var resource = new ApiWebAppResource();
            resource = null;
            if(resource == null)
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource not null altough its null!");
            }
            if(resource != null)
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource not null altough its null!");
            }
            resource = new ApiWebAppResource();
            if (resource.Equals(null))
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource null altough its not null!");
            }
            if(!resource.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource null altough its not null!");
            }
            var resource2 = new ApiWebAppResource();
            Assert.That(resource.Equals(resource2));
            List<ApiWebAppResource> resources = new List<ApiWebAppResource>()
            {
                new ApiWebAppResource(),
                new ApiWebAppResource()
            };
            List<ApiWebAppResource> resources2 = new List<ApiWebAppResource>()
            {
                new ApiWebAppResource(),
                new ApiWebAppResource()
            };
            Assert.That(resources.SequenceEqual(resources2));
        }

        [Test]
        public void WebApp_NullEqualAndSequenceEqual_AsExpected()
        {
            var app = new ApiWebAppData();
            app = null;
            if (app == null)
            {
                ;
            }
            else
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource not null altough its null!");
            }
            if (app != null)
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource not null altough its null!");
            }
            app = new ApiWebAppData();
            if (app.Equals(null))
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource null altough its not null!");
            }
            if (!app.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource null altough its not null!");
            }
            var app2 = new ApiWebAppData();
            Assert.That(app.Equals(app2));
            List<ApiWebAppData> apps = new List<ApiWebAppData>()
            {
                new ApiWebAppData(),
                new ApiWebAppData()
            };
            List<ApiWebAppData> apps2 = new List<ApiWebAppData>()
            {
                new ApiWebAppData(),
                new ApiWebAppData()
            };
            Assert.That(apps.SequenceEqual(apps2));
        }

        [Test]
        public void ApiS5Time_NullEqualAndSequenceEqual_AsExpected()
        {
            var s5Time = new ApiS5Time(10,1);
            s5Time = null;
            if (s5Time == null)
            {
                ;
            }
            else
            {
                Assert.Fail("S5TimeNullEqualAndSequenceEqual - s5Time not null altough its null!");
            }
            if (s5Time != null)
            {
                Assert.Fail("S5TimeNullEqualAndSequenceEqual - s5Time not null altough its null!");
            }
            s5Time = new ApiS5Time(10,1);
            if (s5Time.Equals(null))
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - s5Time not null altough its null!");
            }
            if (!s5Time.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - s5Time not null altough its null!");
            }
            var s5Time2 = new ApiS5Time(10,1);
            Assert.That(s5Time.Equals(s5Time2));
            List<ApiS5Time> s5Times = new List<ApiS5Time>()
            {
                new ApiS5Time(),
                new ApiS5Time()
            };
            List<ApiS5Time> s5Times2 = new List<ApiS5Time>()
            {
                new ApiS5Time(),
                new ApiS5Time()
            };
            Assert.That(s5Times.SequenceEqual(s5Times2));
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
        public void ApiDateAndTime_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiDateAndTime = new ApiDateAndTime();
            apiDateAndTime = null;
            if (apiDateAndTime == null)
            {
                ;
            }
            else
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }

            if (apiDateAndTime != null)
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }
            apiDateAndTime = new ApiDateAndTime();
            if (apiDateAndTime.Equals(null))
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null altough its null!");
            }
            if (!apiDateAndTime.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - apiDateAndTime not null altough its null!");
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

            var dt1 = new ApiDateAndTime();
            var dt2 = new ApiDateAndTime();
            Assert.That(dt1.Equals(dt2));
            var dts = new List<ApiDateAndTime>()
            {
                new ApiDateAndTime(),
                new ApiDateAndTime()
            };
            var dts2 = new List<ApiDateAndTime>()
            {
                new ApiDateAndTime(),
                new ApiDateAndTime()
            };
            Assert.That(dts.SequenceEqual(dts2));
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

        [Test]
        public void ApiClass_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiClass = new ApiClass();
            apiClass = null;
            if (apiClass == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiClass)} not null altough its null!");
            }

            if (apiClass != null)
            {
                Assert.Fail($"{nameof(ApiClass)} not null altough its null!");
            }
            apiClass = new ApiClass();
            if (apiClass.Equals(null))
            {
                Assert.Fail($"{nameof(ApiClass)} not null altough its null!");
            }
            if (!apiClass.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiClass)} not null altough its null!");
            }

            var apiClass2 = new ApiClass();
            Assert.That(apiClass.Equals(apiClass2));
            var apiClasses = new List<ApiClass>()
            {
                new ApiClass(),
                new ApiClass()
            };
            var apiClasses2 = new List<ApiClass>()
            {
                new ApiClass(),
                new ApiClass()
            };
            Assert.That(apiClasses.SequenceEqual(apiClasses2));
        }

        [Test]
        public void ApiError_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiError = new ApiError();
            apiError = null;
            if (apiError == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiError)} not null altough its null!");
            }

            if (apiError != null)
            {
                Assert.Fail($"{nameof(ApiError)} not null altough its null!");
            }
            apiError = new ApiError();
            if (apiError.Equals(null))
            {
                Assert.Fail($"{nameof(ApiError)} not null altough its null!");
            }
            if (!apiError.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiError)} not null altough its null!");
            }

            var apiError2 = new ApiError();
            Assert.That(apiError.Equals(apiError2));
            var apiErrors = new List<ApiError>()
            {
                new ApiError(),
                new ApiError()
            };
            var apiErrors2 = new List<ApiError>()
            {
                new ApiError(),
                new ApiError()
            };
            Assert.That(apiErrors.SequenceEqual(apiErrors2));
        }

        [Test]
        public void ApiPlcProgramData_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiPlcProgramData = new ApiPlcProgramData();
            apiPlcProgramData = null;
            if (apiPlcProgramData == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null altough its null!");
            }

            if (apiPlcProgramData != null)
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null altough its null!");
            }
            apiPlcProgramData = new ApiPlcProgramData();
            if (apiPlcProgramData.Equals(null))
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null altough its null!");
            }
            if (!apiPlcProgramData.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null altough its null!");
            }

            var apiPlcProgramData2 = new ApiPlcProgramData();
            Assert.That(apiPlcProgramData.Equals(apiPlcProgramData2));
            var apiPlcProgramDatas = new List<ApiPlcProgramData>()
            {
                new ApiPlcProgramData(),
                new ApiPlcProgramData()
            };
            var apiPlcProgramDatas2 = new List<ApiPlcProgramData>()
            {
                new ApiPlcProgramData(),
                new ApiPlcProgramData()
            };
            Assert.That(apiPlcProgramDatas.SequenceEqual(apiPlcProgramDatas2));
        }

        [Test]
        public void ApiPlcProgramDataArrayIndexer_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiPlcProgramDataArrayIndexer = new ApiPlcProgramDataArrayIndexer();
            apiPlcProgramDataArrayIndexer = null;
            if (apiPlcProgramDataArrayIndexer == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null altough its null!");
            }

            if (apiPlcProgramDataArrayIndexer != null)
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null altough its null!");
            }
            apiPlcProgramDataArrayIndexer = new ApiPlcProgramDataArrayIndexer();
            if (apiPlcProgramDataArrayIndexer.Equals(null))
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null altough its null!");
            }
            if (!apiPlcProgramDataArrayIndexer.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null altough its null!");
            }

            var apiPlcProgramDataArrayIndexer2 = new ApiPlcProgramDataArrayIndexer();
            Assert.That(apiPlcProgramDataArrayIndexer.Equals(apiPlcProgramDataArrayIndexer2));
            var apiPlcProgramDataArrayIndexers = new List<ApiPlcProgramDataArrayIndexer>()
            {
                new ApiPlcProgramDataArrayIndexer(),
                new ApiPlcProgramDataArrayIndexer()
            };
            var apiPlcProgramDataArrayIndexers2 = new List<ApiPlcProgramDataArrayIndexer>()
            {
                new ApiPlcProgramDataArrayIndexer(),
                new ApiPlcProgramDataArrayIndexer()
            };
            Assert.That(apiPlcProgramDataArrayIndexers.SequenceEqual(apiPlcProgramDataArrayIndexers2));
        }

        [Test]
        public void ApiTicket_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiTicket = new ApiTicket();
            apiTicket = null;
            if (apiTicket == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiTicket)} not null altough its null!");
            }

            if (apiTicket != null)
            {
                Assert.Fail($"{nameof(ApiTicket)} not null altough its null!");
            }
            apiTicket = new ApiTicket();
            if (apiTicket.Equals(null))
            {
                Assert.Fail($"{nameof(ApiTicket)} not null altough its null!");
            }
            if (!apiTicket.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiTicket)} not null altough its null!");
            }
            var ticket2 = new ApiTicket();
            Assert.That(apiTicket.Equals(ticket2));
            List<ApiTicket> tickets = new List<ApiTicket>()
            {
                new ApiTicket(),
                new ApiTicket()
            };
            List<ApiTicket> tickets2 = new List<ApiTicket>()
            {
                new ApiTicket(),
                new ApiTicket()
            };
            Assert.That(tickets.SequenceEqual(tickets2));
        }

        [Test]
        public void ApiWebAppDataSaveSetting_NullEqualAndSequenceEqual_AsExpected()
        {
            var apiWebAppDataSaveSetting = new ApiWebAppDataSaveSetting();
            apiWebAppDataSaveSetting = null;
            if (apiWebAppDataSaveSetting == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null altough its null!");
            }

            if (apiWebAppDataSaveSetting != null)
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null altough its null!");
            }
            apiWebAppDataSaveSetting = new ApiWebAppDataSaveSetting();
            if (apiWebAppDataSaveSetting.Equals(null))
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null altough its null!");
            }
            if (!apiWebAppDataSaveSetting.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null altough its null!");
            }
        }

        [Test]
        public void HttpClientAndWebAppCookie_NullEqualAndSequenceEqual_AsExpected()
        {
            HttpClientAndWebAppCookie httpClientAndWebAppCookie = null;
            if (httpClientAndWebAppCookie == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null altough its null!");
            }

            if (httpClientAndWebAppCookie != null)
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null altough its null!");
            }
            httpClientAndWebAppCookie = new HttpClientAndWebAppCookie(null, null);
            if (httpClientAndWebAppCookie.Equals(null))
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null altough its null!");
            }
            if (!httpClientAndWebAppCookie.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null altough its null!");
            }
            var httpClientAndWebAppCookie2 = new HttpClientAndWebAppCookie(null, null);
            Assert.That(httpClientAndWebAppCookie2.Equals(httpClientAndWebAppCookie));
            List<HttpClientAndWebAppCookie> httpClientConnections = new List<HttpClientAndWebAppCookie>()
            {
                new HttpClientAndWebAppCookie(null, null),
                new HttpClientAndWebAppCookie(null, null)
            };
            List<HttpClientAndWebAppCookie> httpClientConnections2 = new List<HttpClientAndWebAppCookie>()
            {
                new HttpClientAndWebAppCookie(null, null),
                new HttpClientAndWebAppCookie(null, null)
            };
            Assert.That(httpClientConnections.SequenceEqual(httpClientConnections2));
        }

        [Test]
        public void HttpClientConnectionConfiguration_NullEqualAndSequenceEqual_AsExpected()
        {
            HttpClientConnectionConfiguration httpClientConnectionConfiguration = null;
            if (httpClientConnectionConfiguration == null)
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null altough its null!");
            }

            if (httpClientConnectionConfiguration != null)
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null altough its null!");
            }
            httpClientConnectionConfiguration = new HttpClientConnectionConfiguration(null,null,null);
            if (httpClientConnectionConfiguration.Equals(null))
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null altough its null!");
            }
            if (!httpClientConnectionConfiguration.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null altough its null!");
            }
            var httpClientConnectionConfiguration2 = new HttpClientConnectionConfiguration(null, null, null);
            Assert.That(httpClientConnectionConfiguration2.Equals(httpClientConnectionConfiguration));
            List<HttpClientConnectionConfiguration> httpClientConnections = new List<HttpClientConnectionConfiguration>()
            {
                new HttpClientConnectionConfiguration(null, null, null),
                new HttpClientConnectionConfiguration(null, null, null)
            };
            List<HttpClientConnectionConfiguration> httpClientConnections2 = new List<HttpClientConnectionConfiguration>()
            {
                new HttpClientConnectionConfiguration(null, null, null),
                new HttpClientConnectionConfiguration(null, null, null)
            };
            Assert.That(httpClientConnections.SequenceEqual(httpClientConnections2));
        }
    }
}
