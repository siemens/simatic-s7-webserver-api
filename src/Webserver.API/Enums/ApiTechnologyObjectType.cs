// Copyright (c) 2025, Siemens AG
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
        /// <summary>
        /// Technology Object SpeedAxis
        /// </summary>
        To_SpeedAxis,
        /// <summary>
        /// Technology Object PositioningAxis
        /// </summary>
        To_PositioningAxis,
        /// <summary>
        /// Technology Object SynchronousAxis
        /// </summary>
        To_SynchronousAxis,
        /// <summary>
        /// Technology Object ExternalEncoder
        /// </summary>
        To_ExternalEncoder,
        /// <summary>
        /// Technology Object MeasuringInput
        /// </summary>
        To_MeasuringInput,
        /// <summary>
        /// Technology Object OutputCam
        /// </summary>
        To_OutputCam,
        /// <summary>
        /// Technology Object CamTrack
        /// </summary>
        To_CamTrack,
        /// <summary>
        /// Technology Object Cam
        /// </summary>
        To_Cam,
        /// <summary>
        /// Technology Object Kinematics
        /// </summary>
        To_Kinematics,
        /// <summary>
        /// Technology Object LeadingAxisProxy
        /// </summary>
        To_LeadingAxisProxy,
        /// <summary>
        /// Technology Object Cam_10K
        /// </summary>
        To_Cam_10k,
        /// <summary>
        /// Technology Object Interpreter
        /// </summary>
        To_Interpreter,
        /// <summary>
        /// Technology Object InterpreterMapping
        /// </summary>
        To_InterpreterMapping,
        /// <summary>
        /// Technology Object InterpreterProgram
        /// </summary>
        To_InterpreterProgram,
        /// <summary>
        /// Technology Object Cam_600
        /// </summary>
        To_Cam_600,
        /// <summary>
        /// Technology Object Cam_6k
        /// </summary>
        To_Cam_6k,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}