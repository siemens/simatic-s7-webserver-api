// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class EnumTests : Base
    {

        [Test]
        public void GetAccordingDataType_Valid_AsExpected()
        {
            CheckRange_AccordingDataType(Enumerable.Range(3, 2), typeof(byte));
            CheckRange_AccordingDataType(Enumerable.Range(6, 2), typeof(char));
            CheckRange_AccordingDataType(Enumerable.Range(8, 10), typeof(short));
            CheckRange_AccordingDataType(Enumerable.Range(19, 24), typeof(ushort));
            CheckRange_AccordingDataType(Enumerable.Range(47, 2), typeof(int));
            CheckRange_AccordingDataType(Enumerable.Range(49, 8), typeof(uint));
            CheckRange_AccordingDataType(Enumerable.Range(59, 3), typeof(long));
            CheckRange_AccordingDataType(Enumerable.Range(62, 3), typeof(ulong));
            CheckRange_AccordingDataType(Enumerable.Range(65, 2), typeof(string));

            CheckRange_AccordingDataType(Enumerable.Range(2, 1), typeof(bool));
            CheckRange_AccordingDataType(Enumerable.Range(5, 1), typeof(sbyte));
            CheckRange_AccordingDataType(Enumerable.Range(45, 1), typeof(ApiS5Time));
            CheckRange_AccordingDataType(Enumerable.Range(46, 1), typeof(ApiDateAndTime));
            CheckRange_AccordingDataType(Enumerable.Range(57, 1), typeof(float));
            CheckRange_AccordingDataType(Enumerable.Range(58, 1), typeof(double));

            CheckRange_AccordingDataType(Enumerable.Range(0, 1), typeof(object));
            // check db independent from none - I know this could be 0,2 and 1 line instead of two.
            CheckRange_AccordingDataType(Enumerable.Range(1, 1), typeof(object));
            CheckRange_AccordingDataType(Enumerable.Range(67, (81 - 67 + 1)), typeof(object));
        }

        [Test]
        public void GetBytesOfDataType_valid_AsExpected()
        {
            CheckRange_BytesOfDataType(Enumerable.Range(65, 1), 256);
            CheckRange_BytesOfDataType(Enumerable.Range(66, 1), 512);
            CheckRange_BytesOfDataType(Enumerable.Range(2, 6 - 2 + 1), 1);
            CheckRange_BytesOfDataType(Enumerable.Range(7, 45 - 7 + 1), 2);
            CheckRange_BytesOfDataType(Enumerable.Range(46, 57 - 46 + 1), 4);
            CheckRange_BytesOfDataType(Enumerable.Range(58, 64 - 58 + 1), 8);

            CheckRange_BytesOfDataType(Enumerable.Range(0, 1), -1);
            // check db independent from none - I know this could be 0,2 and 1 line instead of two.
            CheckRange_BytesOfDataType(Enumerable.Range(1, 1), -1);
            CheckRange_BytesOfDataType(Enumerable.Range(67, 81 - 67 + 1), -1);
        }

        [Test]
        public void IsSupportedByPlcProgramReadOrWrite_valid_AsExpected()
        {
            CheckRange_IsSupported(Enumerable.Range(65, 1), true);
            CheckRange_IsSupported(Enumerable.Range(66, 1), true);
            CheckRange_IsSupported(Enumerable.Range(2, 6 - 2 + 1), true);
            CheckRange_IsSupported(Enumerable.Range(7, 45 - 7 + 1), true);
            CheckRange_IsSupported(Enumerable.Range(46, 57 - 46 + 1), true);
            CheckRange_IsSupported(Enumerable.Range(58, 64 - 58 + 1), true);

            CheckRange_IsSupported(Enumerable.Range(0, 1), false);
            // check db independent from none - I know this could be 0,2 and 1 line instead of two.
            CheckRange_IsSupported(Enumerable.Range(1, 1), false);
            CheckRange_IsSupported(Enumerable.Range(67, 81 - 67 + 1), false);
        }

        [Test]
        public void Consistency_Structs_NotSupportedByReadOrWrite()
        {
            var enumVals = (ApiPlcProgramDataType[])Enum.GetValues(typeof(ApiPlcProgramDataType));
            foreach (var datatype in enumVals)
            {
                var isSupported = datatype.IsSupportedByPlcProgramReadOrWrite();
                var bytesofDt = datatype.GetBytesOfDataType();
                var accType = datatype.GetAccordingDataType();
                Assert.That(
                    // not supported:
                    !isSupported && accType.Equals(typeof(object)) && bytesofDt == -1
                    ||
                    isSupported && !accType.Equals(typeof(object)) && bytesofDt != -1,
                    $"for datatype: {datatype}");
            }
        }

        private void CheckRange_AccordingDataType(IEnumerable<int> range, Type expectedType)
        {
            var apiPlcProgramDatas = new List<ApiPlcProgramDataType>() { };
            foreach (var rangeEl in range)
            {
                apiPlcProgramDatas.Add((ApiPlcProgramDataType)rangeEl);
            }
            foreach (var apiPlcProgData in apiPlcProgramDatas)
            {
                var accType = apiPlcProgData.GetAccordingDataType();
                Assert.That(accType.Equals(expectedType), $"For ApiPlcProgramData:" +
                    $"{apiPlcProgData.ToString()}:{(int)apiPlcProgData} the according Data Type was {accType} instead of {expectedType}!");
            }
        }

        private void CheckRange_BytesOfDataType(IEnumerable<int> range, int expectedBytes)
        {
            var apiPlcProgramDatas = new List<ApiPlcProgramDataType>() { };
            foreach (var rangeEl in range)
            {
                apiPlcProgramDatas.Add((ApiPlcProgramDataType)rangeEl);
            }
            foreach (var apiPlcProgData in apiPlcProgramDatas)
            {
                var accBytes = apiPlcProgData.GetBytesOfDataType();
                Assert.That(accBytes.Equals(expectedBytes), $"For ApiPlcProgramData:" +
                    $"{apiPlcProgData.ToString()}:{(int)apiPlcProgData} the according Data Type was {accBytes} instead of {expectedBytes}!");
            }
        }

        private void CheckRange_IsSupported(IEnumerable<int> range, bool expectedSupport)
        {
            var apiPlcProgramDatas = new List<ApiPlcProgramDataType>() { };
            foreach (var rangeEl in range)
            {
                apiPlcProgramDatas.Add((ApiPlcProgramDataType)rangeEl);
            }
            foreach (var apiPlcProgData in apiPlcProgramDatas)
            {
                var isSupportedByDt = apiPlcProgData.IsSupportedByPlcProgramReadOrWrite();
                Assert.That(isSupportedByDt.Equals(expectedSupport), $"For ApiPlcProgramData:" +
                    $"{apiPlcProgData.ToString()}:{(int)apiPlcProgData} the according Data Type was {isSupportedByDt} instead of {expectedSupport}!");
            }
        }

        [Test]
        public void ModulesBrowseModeNull_ToString_Null()
        {
            ModulesBrowseMode? mode = null;
            Assert.That(mode?.ToString(), Is.EqualTo(null));
        }

        [Test]
        public void ApiLedColor_Serialized_LowerCase()
        {
            ApiLedColor color = ApiLedColor.Green;
            Assert.That(color.ToString(), Is.EqualTo("Green"));
            var someObject = new { Color = color };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"Color\":\"green\"}"));
        }


        [Test]
        public void ApiLedStatus_Serialized_LowerCase()
        {
            var ledStatus = ApiLedStatus.Flashing;
            Assert.That(ledStatus.ToString(), Is.EqualTo("Flashing"));
            var someObject = new { LedStatus = ledStatus };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"LedStatus\":\"flashing\"}"));
        }

        [Test]
        public void ApiLedType_Serialized_LowerCase()
        {
            var ledType = ApiLedType.RunStop;
            Assert.That(ledType.ToString(), Is.EqualTo("RunStop"));
            var someObject = new { LedType = ledType };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"LedType\":\"run_stop\"}"));
        }

        [Test]
        public void ApiModulesFilterAttribute_Serialized_LowerCase()
        {
            var filterAttribute = ApiModulesFilterAttribute.GeoAddress;
            Assert.That(filterAttribute.ToString(), Is.EqualTo("GeoAddress"));
            var someObject = new { FilterAttribute = filterAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"FilterAttribute\":\"geo_address\"}"));
        }

        [Test]
        public void ApiModulesNodeAttribute_Serialized_LowerCase()
        {
            var nodeAttribute = ApiModulesNodeAttribute.ConfiguredTopology;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("ConfiguredTopology"));
            var someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"configured_topology\"}"));
            nodeAttribute = ApiModulesNodeAttribute.ConfigurationControl;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("ConfigurationControl"));
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"configuration_control\"}"));
            nodeAttribute = ApiModulesNodeAttribute.FirmwareUpdate;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("FirmwareUpdate"));
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"firmware_update\"}"));
            nodeAttribute = ApiModulesNodeAttribute.ServiceData;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("ServiceData"));
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"service_data\"}"));
            nodeAttribute = ApiModulesNodeAttribute.Failsafe;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("Failsafe"));
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"failsafe\"}"));
            nodeAttribute = ApiModulesNodeAttribute.ServiceData;
            Assert.That(nodeAttribute.ToString(), Is.EqualTo("ServiceData"));
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"service_data\"}"));
        }

        [Test]
        public void ApiModulesNodeState_Serialized_LowerCase()
        {
            var nodeAttribute = ApiModulesNodeState.IoNotAvailable;
            var someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"io_not_available\"}"));
            nodeAttribute = ApiModulesNodeState.NotReachable;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"not_reachable\"}"));
            nodeAttribute = ApiModulesNodeState.MaintenanceDemanded;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"maintenance_demanded\"}"));
            nodeAttribute = ApiModulesNodeState.MaintenanceRequired;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"maintenance_required\"}"));
            nodeAttribute = ApiModulesNodeState.Unknown;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"unknown\"}"));
        }

        [Test]
        public void ApiModulesNodeSubType_Serialized_LowerCase()
        {
            var nodeAttribute = ApiModulesNodeSubType.ProfibusIoSystem;
            var someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profibus_iosystem\"}"));
            nodeAttribute = ApiModulesNodeSubType.CentralIoSystem;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"central_iosystem\"}"));
            nodeAttribute = ApiModulesNodeSubType.CentralDevice;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"central_device\"}"));
            nodeAttribute = ApiModulesNodeSubType.ProfinetIoSystem;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profinet_iosystem\"}"));
            nodeAttribute = ApiModulesNodeSubType.ProfibusInterface;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profibus_interface\"}"));
            nodeAttribute = ApiModulesNodeSubType.ProfinetPort;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profinet_port\"}"));
            nodeAttribute = ApiModulesNodeSubType.ProfinetInterfaceVirtual;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profinet_interface_virtual\"}"));
            nodeAttribute = ApiModulesNodeSubType.ProfinetInterface;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"profinet_interface\"}"));
        }

        [Test]
        public void ApiModulesNodeType_Serialized_LowerCase()
        {
            var nodeAttribute = ApiModulesNodeType.Device;
            var someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"device\"}"));
            nodeAttribute = ApiModulesNodeType.Submodule;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"submodule\"}"));
            nodeAttribute = ApiModulesNodeType.IoSystem;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"iosystem\"}"));
            nodeAttribute = ApiModulesNodeType.Module;
            someObject = new { NodeAttribute = nodeAttribute };
            Assert.That(JsonConvert.SerializeObject(someObject), Is.EqualTo("{\"NodeAttribute\":\"module\"}"));
        }


    }
}
