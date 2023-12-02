// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer
{
    /// <summary>
    /// Containes the Textlist_Id and Text_Id of a diagnosticbuffer entry event.
    /// </summary>
    public class ApiDiagnosticBuffer_EntryEvent
    {
        /// <summary>
        /// The text list ID of the event.
        /// </summary>
        public int Textlist_Id { get; set; }
        /// <summary>
        /// The text ID of the event.
        /// </summary>
        public int Text_Id { get; set; }
        /// <summary>
        /// Check wether properties match
        /// </summary>
        /// <param name="obj">ApiDiagnosticBuffer_EntryEvent</param>
        /// <returns>Returns true if the DiagnosticBufferEvents are the same</returns>
        public override bool Equals(object obj)
        {
            var structure = obj as ApiDiagnosticBuffer_EntryEvent;
            if (structure == null) { return false; }
            return structure.Textlist_Id == this.Textlist_Id &&
                   structure.Text_Id == this.Text_Id;
        }
        /// <summary>
        /// GetHashCode for DiagnosticBufferEvent
        /// </summary>
        /// <returns>hashcode for the DiagnosticBufferEvent</returns>
        public override int GetHashCode()
        {
            return (Textlist_Id, Text_Id).GetHashCode();
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer_EntryEvent
        /// </summary>
        /// <returns>ApiDiagnosticBuffer_EntryEvent as a multiline string</returns>
        public override string ToString()
        {
            return ToString(NullValueHandling.Ignore);
        }
        /// <summary>
        /// ToString for ApiDiagnosticBuffer_EntryEvent
        /// </summary>
        /// <param name="nullValueHandling">Defines if null values should be ignored</param>
        /// <returns>ApiDiagnosticBuffer_EntryEvent as a multiline string</returns>
        public string ToString(NullValueHandling nullValueHandling)
        {
            JsonSerializerSettings options = new JsonSerializerSettings()
            {
                NullValueHandling = nullValueHandling,
            };
            string result = JsonConvert.SerializeObject(this, Formatting.Indented, options);
            return result;
        }
    }
}
