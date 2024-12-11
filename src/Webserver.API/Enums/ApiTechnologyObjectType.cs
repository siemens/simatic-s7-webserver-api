// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Enum of all possible technology objects
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiTechnologyObjectType
    {
        To_SpeedAxis,
        To_PositioningAxis,
        To_SynchronousAxis,
        To_ExternalEncoder,
        To_MeasuringInput,
        To_OutputCam,
        To_CamTrack,
        To_Cam,
        To_Kinematics,
        To_LeadingAxisProxy,
        To_Cam_10k,
        To_Interpreter,
        To_InterpreterMapping,
        To_InterpreterProgram,
        To_Cam_600,
        To_Cam_6k,
        Unknown
    }
}
