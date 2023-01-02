// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.ApiPlcProgramDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// ApiPlcProgramDataTypes - static Helper Class for Determination wether PlcProgramDataType is supported by simple mode or not (is a structure or not)
    /// </summary>
    public static class ApiPlcProgramDataTypes
    {
        /// <summary>
        /// function to determine wethere the PlcProgramDataType is supported by simple mode for reading/writing - status: Firmware >=2.8 and less than at least 2.9 (planned V18)
        /// </summary>
        /// <param name="plcProgramDataType"></param>
        /// <returns></returns>
        public static bool IsSupportedByPlcProgramReadOrWrite(this ApiPlcProgramDataType plcProgramDataType)
        {
            switch (plcProgramDataType)
            {
                case ApiPlcProgramDataType.None:
                case ApiPlcProgramDataType.DataBlock:
                case ApiPlcProgramDataType.Struct:
                case ApiPlcProgramDataType.Iec_counter:
                case ApiPlcProgramDataType.Iec_timer:
                case ApiPlcProgramDataType.Dtl:
                case ApiPlcProgramDataType.Iec_ltimer:
                case ApiPlcProgramDataType.Iec_scounter:
                case ApiPlcProgramDataType.Iec_dcounter:
                case ApiPlcProgramDataType.Iec_lcounter:
                case ApiPlcProgramDataType.Iec_ucounter:
                case ApiPlcProgramDataType.Iec_uscounter:
                case ApiPlcProgramDataType.Iec_udcounter:
                case ApiPlcProgramDataType.Iec_ulcounter:
                case ApiPlcProgramDataType.Error_struct:
                case ApiPlcProgramDataType.Nref:
                case ApiPlcProgramDataType.Cref:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// function to determine the amount of bytes the plcprogramdatatype uses/needs
        /// </summary>
        /// <param name="plcProgramDataType"></param>
        /// <returns>the amount of bytes used defaults to -1 for unsupported DataTypes!</returns>
        public static int GetBytesOfDataType(this ApiPlcProgramDataType plcProgramDataType)
        {
            if (plcProgramDataType == ApiPlcProgramDataType.String)
                return 256;
            if (plcProgramDataType == ApiPlcProgramDataType.Wstring)
                return 512;
            int intVal = (int)plcProgramDataType;
            if (intVal >= 2 && intVal <= 6)
                return 1;
            if (intVal >= 7 && intVal <= 45)
                return 2;
            if (intVal >= 46 && intVal <= 57)
                return 4;
            if (intVal >= 58 && intVal <= 64)
                return 8;
            //default: return -1 for unsupported datatypes / 
            return -1;
        }

        /// <summary>
        /// returns the Type that represents the C# data structure (values) to the according tia datatype (plcprogramdatatype)
        /// </summary>
        /// <param name="plcProgramDataType"></param>
        /// <returns>returns the Type that represents the C# data structure (values) to the according tia datatype (plcprogramdatatype)</returns>
        public static Type GetAccordingDataType(this ApiPlcProgramDataType plcProgramDataType)
        {
            int intVal = (int)plcProgramDataType;
            if(intVal >= 3 && intVal <= 4)
                return typeof(byte);
            if(intVal >= 6 && intVal <= 7)
                return typeof(char);
            if (intVal >= 8 && intVal <= 18)
                return typeof(short);
            if (intVal >= 19 && intVal <= 44)
                return typeof(ushort);
            if (intVal >= 47 && intVal <= 48)
                return typeof(int);
            if (intVal >= 49 && intVal <= 56)
                return typeof(uint);
            if (intVal >= 59 && intVal <= 61)
                return typeof(long);
            if (intVal >= 62 && intVal <= 64)
                return typeof(ulong);
            if(intVal >= 65 && intVal <= 66)
                return typeof(string);
            switch (plcProgramDataType)
            {
                case ApiPlcProgramDataType.Bool:
                    return typeof(bool);
                case ApiPlcProgramDataType.Sint:
                    return typeof(sbyte);                
                case ApiPlcProgramDataType.S5time:
                    return typeof(ApiS5Time);
                case ApiPlcProgramDataType.Date_and_time:
                    return typeof(ApiDateAndTime);
                case ApiPlcProgramDataType.Real:
                    return typeof(float);
                case ApiPlcProgramDataType.Lreal:
                    return typeof(double);
                default:
                    return typeof(object);
            }
        }
    }

    /// <summary>
    /// ApiPlcProgramDataType => TIA DataTypes!
    /// </summary>
    public enum ApiPlcProgramDataType
    {
        /// <summary>
        /// should never be the case
        /// </summary>
        None = 0,
        /// <summary>
        /// DataBlock
        /// </summary>
        DataBlock = 1,
        /// <summary>
        /// Bool
        /// </summary>
        Bool = 2,
        /// <summary>
        /// Byte
        /// </summary>
        Byte = 3,
        /// <summary>
        /// Usint
        /// </summary>
        Usint = 4,
        /// <summary>
        /// Sint
        /// </summary>
        Sint = 5,
        /// <summary>
        /// Char
        /// </summary>
        Char = 6,
        /// <summary>
        /// Wchar
        /// </summary>
        Wchar = 7,
        /// <summary>
        /// Int
        /// </summary>
        Int = 8,
        /// <summary>
        /// Ob_any
        /// </summary>
        Ob_any = 9,
        /// <summary>
        /// OB_att
        /// </summary>
        Ob_att = 10,
        /// <summary>
        /// Ob_cyclic
        /// </summary>
        Ob_cyclic = 11,
        /// <summary>
        /// OB_delay
        /// </summary>
        Ob_delay = 12,
        /// <summary>
        /// Ob_diag
        /// </summary>
        Ob_diag = 13,
        /// <summary>
        /// Ob_hwint
        /// </summary>
        Ob_hwint = 14,
        /// <summary>
        /// Ob_pcycle
        /// </summary>
        Ob_pcycle = 15,
        /// <summary>
        /// OB_startup
        /// </summary>
        Ob_startup = 16,
        /// <summary>
        /// Ob_timeerror
        /// </summary>
        Ob_timeerror = 17,
        /// <summary>
        /// Ob_tod
        /// </summary>
        Ob_tod = 18,
        /// <summary>
        /// Conn_any
        /// </summary>
        Conn_any = 19,
        /// <summary>
        /// Conn_ouc
        /// </summary>
        Conn_ouc = 20,
        /// <summary>
        /// Conn_prg
        /// </summary>
        Conn_prg = 21,
        /// <summary>
        /// Date
        /// </summary>
        Date = 22,
        /// <summary>
        /// Db_any
        /// </summary>
        Db_any = 23,
        /// <summary>
        /// Db_dyn
        /// </summary>
        Db_dyn = 24,
        /// <summary>
        /// Db_www
        /// </summary>
        Db_www = 25,
        /// <summary>
        /// Hw_any
        /// </summary>
        Hw_any = 26,
        /// <summary>
        /// Hw_device
        /// </summary>
        Hw_device = 27,
        /// <summary>
        /// Hw_dpmaster
        /// </summary>
        Hw_dpmaster = 28,
        /// <summary>
        /// Hw_dpslaves
        /// </summary>
        Hw_dpslave = 29,
        /// <summary>
        /// Hw_hsc
        /// </summary>
        Hw_hsc = 30,
        /// <summary>
        /// Hw_ieport
        /// </summary>
        Hw_ieport = 31,
        /// <summary>
        /// Hw_interface
        /// </summary>
        Hw_interface = 32,
        /// <summary>
        /// Hw_io
        /// </summary>
        Hw_io = 33,
        /// <summary>
        /// Hw_iosystem
        /// </summary>
        Hw_iosystem = 34,
        /// <summary>
        /// Hw_module
        /// </summary>
        Hw_module = 35,
        /// <summary>
        /// Hw_pto
        /// </summary>
        Hw_pto = 36,
        /// <summary>
        /// Hw_pwm
        /// </summary>
        Hw_pwm = 37,
        /// <summary>
        /// Hw_submodule
        /// </summary>
        Hw_submodule = 38,
        /// <summary>
        /// Pip
        /// </summary>
        Pip = 39,
        /// <summary>
        /// Port
        /// </summary>
        Port = 40,
        /// <summary>
        /// Rtm
        /// </summary>
        Rtm = 41,
        /// <summary>
        /// Rtm_id
        /// </summary>
        Rtm_id = 42,
        /// <summary>
        /// Uint
        /// </summary>
        Uint = 43,
        /// <summary>
        /// Word
        /// </summary>
        Word = 44,
        /// <summary>
        /// S5time
        /// </summary>
        S5time = 45,
        /// <summary>
        /// Date_and_time
        /// </summary>
        Date_and_time = 46,
        /// <summary>
        /// Dint
        /// </summary>
        Dint = 47,
        /// <summary>
        /// Time
        /// </summary>
        Time = 48,
        /// <summary>
        /// Aom_ident
        /// </summary>
        Aom_ident = 49,
        /// <summary>
        /// Conn_r_id
        /// </summary>
        Conn_r_id = 50,
        /// <summary>
        /// Event_any
        /// </summary>
        Event_any = 51,
        /// <summary>
        /// Event_att
        /// </summary>
        Event_att = 52,
        /// <summary>
        /// Event_hwint
        /// </summary>
        Event_hwint = 53,
        /// <summary>
        /// Dword
        /// </summary>
        Dword = 54,
        /// <summary>
        /// Time_of_day
        /// </summary>
        Time_of_day = 55,
        /// <summary>
        /// Udint
        /// </summary>
        Udint = 56,
        /// <summary>
        /// Real
        /// </summary>
        Real = 57,
        /// <summary>
        /// Lreal
        /// </summary>
        Lreal = 58,
        /// <summary>
        /// Ldt
        /// </summary>
        Ldt = 59,
        /// <summary>
        /// Lint
        /// </summary>
        Lint = 60,
        /// <summary>
        /// Ltime
        /// </summary>
        Ltime = 61,
        /// <summary>
        /// Ulint
        /// </summary>
        Ulint = 62,
        /// <summary>
        /// Lword
        /// </summary>
        Lword = 63,
        /// <summary>
        /// Ltime_of_day
        /// </summary>
        Ltime_of_day = 64,
        /// <summary>
        /// String
        /// </summary>
        String = 65,
        /// <summary>
        /// Wstring
        /// </summary>
        Wstring = 66,
        /// <summary>
        /// Cref
        /// </summary>
        Cref = 67,
        /// <summary>
        /// Dtl
        /// </summary>
        Dtl = 68,
        /// <summary>
        /// Error_struct
        /// </summary>
        Error_struct = 69,
        /// <summary>
        /// Iec_counter
        /// </summary>
        Iec_counter = 70,
        /// <summary>
        /// Iec_dcounter
        /// </summary>
        Iec_dcounter = 71,
        /// <summary>
        /// Iec_lcounter
        /// </summary>
        Iec_lcounter = 72,
        /// <summary>
        /// Iec_ltimer
        /// </summary>
        Iec_ltimer = 73,
        /// <summary>
        /// Iec_scounter
        /// </summary>
        Iec_scounter = 74,
        /// <summary>
        /// Iec_timer
        /// </summary>
        Iec_timer = 75,
        /// <summary>
        /// Iec_ucounter
        /// </summary>
        Iec_ucounter = 76,
        /// <summary>
        /// Iec_udcounter
        /// </summary>
        Iec_udcounter = 77,
        /// <summary>
        /// Iec_ulcounter
        /// </summary>
        Iec_ulcounter = 78,
        /// <summary>
        /// Iec_uscounter
        /// </summary>
        Iec_uscounter = 79,
        /// <summary>
        /// Nref
        /// </summary>
        Nref = 80,
        /// <summary>
        /// Struct
        /// </summary>
        Struct = 81
    }
}
