// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.AlarmsBrowse;
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes;
using Siemens.Simatic.S7.Webserver.API.Models.ApiSyslog;
using Siemens.Simatic.S7.Webserver.API.Models.TimeSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class ModelTests : Base
    {
        [Test]
        public void ApiWebAppResource_NullEqualAndSequenceEqual_AsExpected()
        {
            var resource = new ApiWebAppResource();
            resource = null;
            if (resource == null)
            {
                // Expected case: resource is null - no action needed.
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource not null although its null!");
            }
            if (resource != null)
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource not null although its null!");
            }
            resource = new ApiWebAppResource();
            if (resource.Equals(null))
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource null although its not null!");
            }
            if (!resource.Equals(null))
            {
                // Expected case: resource is null - no action needed.
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - resource null although its not null!");
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
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource not null although its null!");
            }
            if (app != null)
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource not null although its null!");
            }
            app = new ApiWebAppData();
            if (app.Equals(null))
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource null although its not null!");
            }
            if (!app.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("WebAppNullEqualAndSequenceEqual - resource null although its not null!");
            }
            var app2 = new ApiWebAppData();
            Assert.That(app.Equals(app2));
            app.Redirect_mode = ApiWebAppRedirectMode.Redirect;
            // changed redirect mode - not equal
            Assert.That(!app.Equals(app2));
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

            apps.First().Default_page = "";
            apps2.First().Default_page = null;
            Assert.That(apps.SequenceEqual(apps2));
            Assert.That(apps.First().Equals(apps2.First()));
            apps.First().Not_found_page = "";
            apps2.First().Not_found_page = null;
            Assert.That(apps.SequenceEqual(apps2));
            Assert.That(apps.First().Equals(apps2.First()));
            apps.First().Version = "";
            apps2.First().Version = null;
            Assert.That(apps.SequenceEqual(apps2));
            Assert.That(apps.First().Equals(apps2.First()));
            apps.First().Not_authorized_page = "";
            apps2.First().Not_authorized_page = null;
            Assert.That(apps.SequenceEqual(apps2));
            Assert.That(apps.First().Equals(apps2.First()));

            apps.First().Redirect_mode = ApiWebAppRedirectMode.Redirect;
            // changed redirect mode - not equal
            Assert.That(!apps.SequenceEqual(apps2));
            apps2.First().Redirect_mode = ApiWebAppRedirectMode.Redirect;
            Assert.That(apps.SequenceEqual(apps2));
            apps2.First().Redirect_mode = ApiWebAppRedirectMode.Forward;
            // when both have value -> not equal
            Assert.That(!apps.SequenceEqual(apps2));
            // but exact equal wont ...
            Assert.That(!apps.First().Equals(apps2.First()));

        }

        [Test]
        public void ApiS5Time_NullEqualAndSequenceEqual_AsExpected()
        {
            var s5Time = new ApiS5Time(10, 1);
            s5Time = null;
            if (s5Time == null)
            {
                ;
            }
            else
            {
                Assert.Fail("S5TimeNullEqualAndSequenceEqual - s5Time not null although its null!");
            }
            if (s5Time != null)
            {
                Assert.Fail("S5TimeNullEqualAndSequenceEqual - s5Time not null although its null!");
            }
            s5Time = new ApiS5Time(10, 1);
            if (s5Time.Equals(null))
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - s5Time not null although its null!");
            }
            if (!s5Time.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - s5Time not null although its null!");
            }
            var s5Time2 = new ApiS5Time(10, 1);
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
            if (!s5Time.Equals(secondS5Time))
            {
                Assert.Fail("same values S5time are not equal!");
            }
            if (s5Time == secondS5Time)
            {
                Assert.Fail("unexpectedly s5Time == secondS5Time");
            }
            if (!(s5Time != secondS5Time))
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
            if (MinValDT != new DateTime(1990, 1, 1, 0, 0, 0))
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
            dateAndTime = new ApiDateAndTime() { Second = 2 };
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
                Assert.Fail("apiDateAndTime - apiDateAndTime not null although its null!");
            }

            if (apiDateAndTime != null)
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null although its null!");
            }
            apiDateAndTime = new ApiDateAndTime();
            if (apiDateAndTime.Equals(null))
            {
                Assert.Fail("apiDateAndTime - apiDateAndTime not null although its null!");
            }
            if (!apiDateAndTime.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail("ResourcNullEqualAndSequenceEqual - apiDateAndTime not null although its null!");
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
            if (!myDate.Equals(ApiDateAndTime.MinValue))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myDate} and {ApiDateAndTime.MinValue}");
            }
            myDate.Month = 12;
            myDate.Day = 31;
            myDate.Year = 2022;
            myDate.Hour = 23;
            myDate.Minute = 59;
            myDate.Second = 59.999;
            var compare = new DateTime(2022, 12, 31, 23, 59, 59).AddMilliseconds(999);
            var myDT = myDate.GetDateTime();
            if (myDT != compare)
            {
                Assert.Fail($"Unexpectedly dates dont match: {myDate} and {ApiDateAndTime.MinValue}");
            }
            var anotherDt = new ApiDateAndTime(new DateTime(2022, 12, 31, 23, 59, 59).AddMilliseconds(999));
            if (!anotherDt.Equals(myDate))
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
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(1989, 1, 1, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(1930, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(2300, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ApiDateAndTime(new DateTime(2090, 1, 1)));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Year = 2090;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Year = 1989;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Month = 13;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Month = -1;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Day = 32;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Day = -1;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Second = 61;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mydT = new ApiDateAndTime(2020, 1, 1, 0, 0, 0.0);
                mydT.Second = -1;
            });
        }

        [Test]
        public void ApiS5Time_ValuesAreAsExpected()
        {
            var myS5Time = new ApiS5Time(10, 0);
            if (myS5Time.GetTimeSpan() != TimeSpan.FromMilliseconds(0))
            {
                Assert.Fail($"Unexpected myS5Time: {myS5Time}");
            }
            if (!myS5Time.Equals(ApiS5Time.MinValue))
            {
                Assert.Fail($"Unexpectedly dates dont match: {myS5Time} and {ApiDateAndTime.MinValue}");
            }
            var secondS5Time = new ApiS5Time(TimeSpan.FromMilliseconds(0));
            if (!myS5Time.Equals(secondS5Time))
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
                Assert.Fail($"{nameof(ApiClass)} not null although its null!");
            }

            if (apiClass != null)
            {
                Assert.Fail($"{nameof(ApiClass)} not null although its null!");
            }
            apiClass = new ApiClass();
            if (apiClass.Equals(null))
            {
                Assert.Fail($"{nameof(ApiClass)} not null although its null!");
            }
            if (!apiClass.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiClass)} not null although its null!");
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
                Assert.Fail($"{nameof(ApiError)} not null although its null!");
            }

            if (apiError != null)
            {
                Assert.Fail($"{nameof(ApiError)} not null although its null!");
            }
            apiError = new ApiError();
            if (apiError.Equals(null))
            {
                Assert.Fail($"{nameof(ApiError)} not null although its null!");
            }
            if (!apiError.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiError)} not null although its null!");
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
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null although its null!");
            }

            if (apiPlcProgramData != null)
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null although its null!");
            }
            apiPlcProgramData = new ApiPlcProgramData();
            if (apiPlcProgramData.Equals(null))
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null although its null!");
            }
            if (!apiPlcProgramData.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramData)} not null although its null!");
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
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null although its null!");
            }

            if (apiPlcProgramDataArrayIndexer != null)
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null although its null!");
            }
            apiPlcProgramDataArrayIndexer = new ApiPlcProgramDataArrayIndexer();
            if (apiPlcProgramDataArrayIndexer.Equals(null))
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null although its null!");
            }
            if (!apiPlcProgramDataArrayIndexer.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiPlcProgramDataArrayIndexer)} not null although its null!");
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
                Assert.Fail($"{nameof(ApiTicket)} not null although its null!");
            }

            if (apiTicket != null)
            {
                Assert.Fail($"{nameof(ApiTicket)} not null although its null!");
            }
            apiTicket = new ApiTicket();
            if (apiTicket.Equals(null))
            {
                Assert.Fail($"{nameof(ApiTicket)} not null although its null!");
            }
            if (!apiTicket.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiTicket)} not null although its null!");
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
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null although its null!");
            }

            if (apiWebAppDataSaveSetting != null)
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null although its null!");
            }
            apiWebAppDataSaveSetting = new ApiWebAppDataSaveSetting();
            if (apiWebAppDataSaveSetting.Equals(null))
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null although its null!");
            }
            if (!apiWebAppDataSaveSetting.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(ApiWebAppDataSaveSetting)} not null although its null!");
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
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null although its null!");
            }

            if (httpClientAndWebAppCookie != null)
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null although its null!");
            }
            httpClientAndWebAppCookie = new HttpClientAndWebAppCookie(null, null);
            if (httpClientAndWebAppCookie.Equals(null))
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null although its null!");
            }
            if (!httpClientAndWebAppCookie.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientAndWebAppCookie)} not null although its null!");
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
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null although its null!");
            }

            if (httpClientConnectionConfiguration != null)
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null although its null!");
            }
            httpClientConnectionConfiguration = new HttpClientConnectionConfiguration(null, null, null, TimeSpan.Zero, false, false, false);
            if (httpClientConnectionConfiguration.Equals(null))
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null although its null!");
            }
            if (!httpClientConnectionConfiguration.Equals(null))
            {
                ;
            }
            else
            {
                Assert.Fail($"{nameof(HttpClientConnectionConfiguration)} not null although its null!");
            }
            var httpClientConnectionConfiguration2 = new HttpClientConnectionConfiguration(null, null, null, TimeSpan.Zero, false, false, false);
            Assert.That(httpClientConnectionConfiguration2.Equals(httpClientConnectionConfiguration));
            List<HttpClientConnectionConfiguration> httpClientConnections = new List<HttpClientConnectionConfiguration>()
            {
                new HttpClientConnectionConfiguration(null, null, null,TimeSpan.Zero,false,false,false),
                new HttpClientConnectionConfiguration(null, null, null,TimeSpan.Zero,false,false,false)
            };
            List<HttpClientConnectionConfiguration> httpClientConnections2 = new List<HttpClientConnectionConfiguration>()
            {
                new HttpClientConnectionConfiguration(null, null, null,TimeSpan.Zero,false,false,false),
                new HttpClientConnectionConfiguration(null, null, null,TimeSpan.Zero,false,false,false)
            };
            Assert.That(httpClientConnections.SequenceEqual(httpClientConnections2));
        }

        [Test]
        public void PlcDate_InvalidValuesNotAccepted()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(0, 1, ApiDayOfWeek.Sun, 12, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(13, 1, ApiDayOfWeek.Mon, 12, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(1, 0, ApiDayOfWeek.Tue, 12, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(12, 6, ApiDayOfWeek.Wed, 12, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(1, 1, ApiDayOfWeek.Thu, -1, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(12, 5, ApiDayOfWeek.Fri, 24, 43));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(1, 1, ApiDayOfWeek.Sat, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlcDate(12, 5, ApiDayOfWeek.Sun, 23, 60));
        }

        [Test]
        public void PlcDate_EqualsCheck()
        {
            var equalsPD1 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var equalsPD2 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var notEqualsPD1 = new PlcDate(12, 5, ApiDayOfWeek.Sun, 2, 24);

            Assert.That(equalsPD1.Equals(equalsPD2));
            Assert.That(equalsPD2.Equals(equalsPD1));

            Assert.That(notEqualsPD1, Is.Not.EqualTo(equalsPD1));
            Assert.That(notEqualsPD1, Is.Not.EqualTo(equalsPD2));
        }

        [Test]
        public void DaylightSavingTimeConfiguration_InvalidValuesNotAccepted()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DaylightSavingsTimeConfiguration(new PlcDate(11, 1, ApiDayOfWeek.Sun, 12, 43),
                                                                                                  TimeSpan.FromMinutes(-181)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DaylightSavingsTimeConfiguration(new PlcDate(11, 1, ApiDayOfWeek.Sun, 12, 43),
                                                                                                  TimeSpan.FromMinutes(181)));
        }

        [Test]
        public void DaylightSavingTimeConfiguration_EqualsCheck()
        {
            var equalsPD1 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var equalsPD2 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var notEqualsPD2 = new PlcDate(10, 5, ApiDayOfWeek.Wed, 22, 10);

            var equalsDST1 = new DaylightSavingsTimeConfiguration(equalsPD1, TimeSpan.FromMinutes(60));
            var equalsDST2 = new DaylightSavingsTimeConfiguration(equalsPD2, TimeSpan.FromMinutes(60));
            var notequalsDST1 = new DaylightSavingsTimeConfiguration(notEqualsPD2, TimeSpan.FromMinutes(60));
            var notequalsDST2 = new DaylightSavingsTimeConfiguration(equalsPD1, TimeSpan.FromMinutes(120));

            Assert.That(equalsDST1.Equals(equalsDST2));
            Assert.That(equalsDST2.Equals(equalsDST1));

            Assert.That(notequalsDST1, Is.Not.EqualTo(equalsDST1));
            Assert.That(notequalsDST2, Is.Not.EqualTo(equalsDST2));
        }

        [Test]
        public void StandardTimeConfiguration_EqualsCheck()
        {
            var equalsPD1 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var equalsPD2 = new PlcDate(11, 5, ApiDayOfWeek.Sun, 2, 24);
            var notEqualsPD2 = new PlcDate(10, 5, ApiDayOfWeek.Wed, 22, 10);

            var equalsSTD1 = new StandardTimeConfiguration(equalsPD1);
            var equalsSTD2 = new StandardTimeConfiguration(equalsPD2);
            var notequalsSTD1 = new StandardTimeConfiguration(notEqualsPD2);

            Assert.That(equalsSTD1.Equals(equalsSTD2));
            Assert.That(equalsSTD2.Equals(equalsSTD1));

            Assert.That(notequalsSTD1, Is.Not.EqualTo(equalsSTD1));
        }

        [Test]
        public void ApiSyslog_EqualsCheck()
        {
            List<ApiPlcSyslog_Entry> apiSyslog_Entries = new List<ApiPlcSyslog_Entry>() { new ApiPlcSyslog_Entry() { Raw = "hello word" } };
            List<ApiPlcSyslog_Entry> apiSyslog_Entries_other = new List<ApiPlcSyslog_Entry>() { new ApiPlcSyslog_Entry() { Raw = "goodbye word" } };
            ApiPlcSyslog equal1 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog equal2 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal1 = new ApiPlcSyslog() { Count_Lost = 4, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal2 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 99, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal3 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries_other };
            Assert.That(equal1.Equals(equal2), $"{equal1} \nnot equal\n{equal2}");
            Assert.That(equal2.Equals(equal1), $"{equal2} \nnot equal\n{equal1}");

            Assert.That(equal1, Is.Not.EqualTo(not_equal1));
            Assert.That(equal1, Is.Not.EqualTo(not_equal2));
            Assert.That(equal1, Is.Not.EqualTo(not_equal3));
        }

        [Test]
        public void ApiSyslog_Entry_EqualsCheck()
        {
            ApiPlcSyslog_Entry equal1 = new ApiPlcSyslog_Entry() { Raw = "random text" };
            ApiPlcSyslog_Entry equal2 = new ApiPlcSyslog_Entry() { Raw = "random text" };
            ApiPlcSyslog_Entry not_equal1 = new ApiPlcSyslog_Entry() { Raw = "random txet" };
            Assert.That(equal1.Equals(equal2), $"{equal1} \nnot equal\n{equal2}");
            Assert.That(equal2.Equals(equal1), $"{equal2} \nnot equal\n{equal1}");

            Assert.That(equal1, Is.Not.EqualTo(not_equal1));
        }
        [Test]
        public void ApiSyslog_HashCodeCheck()
        {
            List<ApiPlcSyslog_Entry> apiSyslog_Entries = new List<ApiPlcSyslog_Entry>() { new ApiPlcSyslog_Entry() { Raw = "hello word" } };
            List<ApiPlcSyslog_Entry> apiSyslog_Entries_other = new List<ApiPlcSyslog_Entry>() { new ApiPlcSyslog_Entry() { Raw = "goodbye word" } };
            ApiPlcSyslog equal1 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog equal2 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal1 = new ApiPlcSyslog() { Count_Lost = 4, Count_Total = 100, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal2 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 101, Entries = apiSyslog_Entries };
            ApiPlcSyslog not_equal3 = new ApiPlcSyslog() { Count_Lost = 5, Count_Total = 100, Entries = apiSyslog_Entries_other };
            Assert.That(equal1.GetHashCode(), Is.EqualTo(equal1.GetHashCode()), "GetHashcode was not equal although it should be");
            Assert.That(equal1.GetHashCode(), Is.EqualTo(equal2.GetHashCode()), "GetHashcode was not equal although it should be");

            Assert.That(equal1.GetHashCode(), Is.Not.EqualTo(not_equal1.GetHashCode()), "GetHashcode was equal although it should not be");
            Assert.That(equal1.GetHashCode(), Is.Not.EqualTo(not_equal2.GetHashCode()), "GetHashcode was equal although it should not be");
            Assert.That(equal1.GetHashCode(), Is.Not.EqualTo(not_equal3.GetHashCode()), "GetHashcode was equal although it should not be");
        }
        [Test]
        public void ApiSyslog_Entry_HashCodeCheck()
        {
            ApiPlcSyslog_Entry equal1 = new ApiPlcSyslog_Entry() { Raw = "random text" };
            ApiPlcSyslog_Entry equal2 = new ApiPlcSyslog_Entry() { Raw = "random text" };
            ApiPlcSyslog_Entry not_equal1 = new ApiPlcSyslog_Entry() { Raw = "random txet" };
            Assert.That(equal1.GetHashCode(), Is.EqualTo(equal1.GetHashCode()), $"equal1.GetHashCode() not equal equal1.GetHashCode()");
            Assert.That(equal1.GetHashCode(), Is.EqualTo(equal2.GetHashCode()), $"equal1.GetHashCode() not equal equal2.GetHashCode()");

            Assert.That(equal1.GetHashCode(), Is.Not.EqualTo(not_equal1.GetHashCode()), "GetHashcode was equal although it should not be");
        }

        [Test]
        public void ApiAlarms_EqualsCheck()
        {
            ApiAlarms equal1 = new ApiAlarms() { Count_Current = 10, Count_Max = 1000, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiAlarms equal2 = new ApiAlarms() { Count_Current = 10, Count_Max = 1000, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiAlarms not_equal = new ApiAlarms() { Count_Current = 11, Count_Max = 1000, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            Assert.That(equal1.Equals(equal2), $"{equal1} not equal\n{equal2}");
            Assert.That(equal2.Equals(equal1), $"{equal2} not equal\n{equal1}");

            Assert.That(equal1, Is.Not.EqualTo(not_equal), "was equal although it should not be");
        }

        [Test]
        public void ApiAlarms_Entry_EqualsCheck()
        {
            ApiAlarms_Entry equal1 = new ApiAlarms_Entry()
            {
                Id = "1",
                Alarm_Number = 518,
                Status = ApiObjectDirectoryStatus.Incoming,
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Producer = "hardcoded_testcase",
                Hwid = null,
                Alarm_Text = "hi",
                Info_Text = "word",
                Text_Inconsistent = false
            };
            ApiAlarms_Entry equal2 = new ApiAlarms_Entry()
            {
                Id = "1",
                Alarm_Number = 518,
                Status = ApiObjectDirectoryStatus.Incoming,
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Producer = "hardcoded_testcase",
                Hwid = null,
                Alarm_Text = "hi",
                Info_Text = "word",
                Text_Inconsistent = false
            };
            ApiAlarms_Entry not_equal = new ApiAlarms_Entry()
            {
                Id = "1",
                Alarm_Number = 519,
                Status = ApiObjectDirectoryStatus.Incoming,
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Producer = "hardcoded_testcase",
                Hwid = null,
                Alarm_Text = "hi",
                Info_Text = "word",
                Text_Inconsistent = false
            };
            Assert.That(equal1.Equals(equal2), $"{equal1} not equal\n{equal2}");
            Assert.That(equal2.Equals(equal1), $"{equal2} not equal\n{equal1}");
            Assert.That(equal1, Is.Not.EqualTo(not_equal));
        }

        [Test]
        public void ApiAlarms_EntryAcknowledgement_EqualsCheck()
        {
            ApiAlarms_EntryAcknowledgement equal1 = new ApiAlarms_EntryAcknowledgement() { State = ApiAlarmAcknowledgementState.Not_Acknowledged };
            ApiAlarms_EntryAcknowledgement equal2 = new ApiAlarms_EntryAcknowledgement() { State = ApiAlarmAcknowledgementState.Not_Acknowledged };
            ApiAlarms_EntryAcknowledgement not_equal = new ApiAlarms_EntryAcknowledgement() { State = ApiAlarmAcknowledgementState.Acknowledged, Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            Assert.That(equal1.Equals(equal2), $"{equal1}  \nnot equal\n {equal2}");
            Assert.That(equal2.Equals(equal1), $"{equal2}  \nnot equal\n {equal1}");

            Assert.That(equal1, Is.Not.EqualTo(not_equal));
        }

        [Test]
        public void ApiDiagnosticBuffer_EqualsCheck()
        {
            ApiDiagnosticBuffer equals1 = new ApiDiagnosticBuffer() { Count_Current = 10, Count_Max = 2400, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiDiagnosticBuffer equals2 = new ApiDiagnosticBuffer() { Count_Current = 10, Count_Max = 2400, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiDiagnosticBuffer not_equals1 = new ApiDiagnosticBuffer() { Count_Current = 11, Count_Max = 2400, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiDiagnosticBuffer not_equals2 = new ApiDiagnosticBuffer() { Count_Current = 10, Count_Max = 1400, Language = "en-US", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiDiagnosticBuffer not_equals3 = new ApiDiagnosticBuffer() { Count_Current = 10, Count_Max = 2400, Language = "hun", Last_Modified = new DateTime(2023, 6, 15, 10, 33, 24, 123) };
            ApiDiagnosticBuffer not_equals4 = new ApiDiagnosticBuffer() { Count_Current = 10, Count_Max = 2400, Language = "en-US", Last_Modified = new DateTime(2021, 6, 15, 10, 33, 24, 123) };
            Assert.That(equals1.Equals(equals2), $"{equals1} not equal to\n {equals2}");
            Assert.That(equals2.Equals(equals1), $"{equals2} not equal to\n {equals1}");


            Assert.That(equals1, Is.Not.EqualTo(not_equals1));
            Assert.That(equals1, Is.Not.EqualTo(not_equals2));
            Assert.That(equals1, Is.Not.EqualTo(not_equals3));
            Assert.That(equals1, Is.Not.EqualTo(not_equals4));
        }
        [Test]
        public void ApiDiagnosticBuffer_Entry_EqualsCheck()
        {
            ApiDiagnosticBuffer_Entry equals1 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text",
                Short_Text = "short text",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry equals2 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text",
                Short_Text = "short text",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry not_equals1 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2021, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text",
                Short_Text = "short text",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry not_equals2 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Outgoing,
                Long_Text = "long text",
                Short_Text = "short text",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry not_equals3 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text2",
                Short_Text = "short text",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry not_equals4 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text",
                Short_Text = "short text2",
                Help_Text = "help text"
            };
            ApiDiagnosticBuffer_Entry not_equals5 = new ApiDiagnosticBuffer_Entry()
            {
                Event = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 },
                Timestamp = new DateTime(2023, 6, 15, 10, 33, 24, 123),
                Status = ApiObjectDirectoryStatus.Incoming,
                Long_Text = "long text",
                Short_Text = "short text",
                Help_Text = "help text2"
            };
            Assert.That(equals1.Equals(equals2), $"{equals1} not equal to\n {equals2}");
            Assert.That(equals2.Equals(equals1), $"{equals2} not equal to\n {equals1}");

            Assert.That(equals1, Is.Not.EqualTo(not_equals1));
            Assert.That(equals1, Is.Not.EqualTo(not_equals2));
            Assert.That(equals1, Is.Not.EqualTo(not_equals3));
            Assert.That(equals1, Is.Not.EqualTo(not_equals4));
            Assert.That(equals1, Is.Not.EqualTo(not_equals5));
        }
        [Test]
        public void ApiDiagnosticBuffer_EntryEvent_EqualsCheck()
        {
            ApiDiagnosticBuffer_EntryEvent equals1 = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 };
            ApiDiagnosticBuffer_EntryEvent equals2 = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 5 };
            ApiDiagnosticBuffer_EntryEvent not_equals1 = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 1, Text_Id = 5 };
            ApiDiagnosticBuffer_EntryEvent not_equals2 = new ApiDiagnosticBuffer_EntryEvent() { Textlist_Id = 2, Text_Id = 6 };
            Assert.That(equals1.Equals(equals2), $"{equals1} \n not equal to\n  {equals2}");
            Assert.That(equals2.Equals(equals1), $"{equals2} \n not equal to\n  {equals1}");

            Assert.That(equals1, Is.Not.EqualTo(not_equals1));
            Assert.That(equals1, Is.Not.EqualTo(not_equals2));
        }
    }
}
