﻿// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses.ResponseResults
{
    /// <summary>
    /// Result of a Files.Browse request (List
    /// </summary>
    public class ApiBrowseFilesResult
    {
        /// <summary>
        /// Resources contained or the File requested
        /// </summary>
        public List<ApiFileResource> Resources { get; set; }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
