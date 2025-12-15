// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiWebAppResource => Data container for WebApp Resources, configurations can be made using Api functions
    /// </summary>
    public class ApiWebAppResource
    {

        /// <summary>
        /// Function to MemberwiseClone an ApiWebAppResource to another Object.
        /// </summary>
        /// <returns></returns>
        public ApiWebAppResource ShallowCopy()
        {
            return (ApiWebAppResource)this.MemberwiseClone();
        }

        /// <summary>
        /// Resource Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Size of the resource in Bytes - windows long but on the plc will be stored as uint 32 - therefor max. Size is not Windows max. Size (long)
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Resources Media Type - (plc uses to) determine how to display/interpret the file 
        /// For mappings see static MIMEType.Mappings
        /// </summary>
        public string Media_type { get; set; }

        private ApiWebAppResourceVisibility visibility;
        /// <summary>
        /// Resource Visibility - (plc uses to) determine wether a user is allowed to access the Resource (has cookie with according rights)
        /// </summary>
        public ApiWebAppResourceVisibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                if (value == ApiWebAppResourceVisibility.None)
                {
                    throw new ApiInvalidResponseException($"Returned from api was:{value} - which is not valid! contact Siemens");
                }
                visibility = value;
            }
        }

        /// <summary>
        /// Last Modified Date of the Resource (UTC)(as string according to RFC3339)
        /// </summary>
        public DateTime Last_modified { get; set; }

        /// <summary>
        /// Etag (optional) to identify resources identity e.g.
        /// </summary>
        public string Etag { get; set; }

        /// <summary>
        /// Bool used to determine the Compare for the equals function:
        /// The origin for the issue is the "Byte order Mark" (BOM) that JavaScript e.g. will not transfer to the plc (and therefor on comparing the resource will be "different"
        /// when set to true: another resource with the same size, size+3byte, size-3byte will also be accepted as "Equal"
        /// when not set to true: only a resource that has the same size aswell will be accepted
        /// </summary>
        public bool? IgnoreBOMDifference { get; set; }

        /// <summary>
        /// Method used to determine wether the file is an HTML file or not
        /// </summary>
        /// <returns>resource.Media_type == "text/html"</returns>
        public bool IsHtmlFile() => this.Media_type.Equals("text/html");

        /// <summary>
        /// Depending on the configured IgnoreBOMDifference =>
        /// Compare resources Name,Size,Media_Type,Visibility, LastModified and Etag wether the resources are Equal or not
        /// IF IgnoreBOMDifference is set
        /// 3 bytes more or less will be accepted aswell (Byte Order Mark has 3 bytes length and will not be sent using JavaScript e.g.)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ApiWebAppResource other)
        {
            if (other is null)
                return false;

            string lastModified = this.Last_modified.ToString();
            string otherLastModified = other.Last_modified.ToString();
            if (IgnoreBOMDifference.HasValue && IgnoreBOMDifference == true)
            {
                bool toReturn = (this.Name == other.Name && ((this.Size == (other.Size - 3)) || (this.Size == other.Size) || (this.Size == (other.Size + 3))) && this.Media_type == other.Media_type
                && this.Visibility == other.Visibility && (lastModified == otherLastModified) && this.Etag == other.Etag);
                return toReturn;
            }
            else
            {
                return this.Name == other.Name && (this.Size == other.Size) && this.Media_type == other.Media_type
                && this.Visibility == other.Visibility && (lastModified == otherLastModified) && this.Etag == other.Etag;
            }

        }

        /// <summary>
        /// Calls the Equals for the object being an ApiWebAppResource
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiWebAppResource);

        /// <summary>
        /// Depending on IgnoreBOMDifference:
        /// true: (Name, Media_type, Visibility, Etag, Last_modified.ToString()).GetHashCode()
        /// false: (Name, Media_type, Visibility, Etag, Last_modified.ToString()).GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (IgnoreBOMDifference.HasValue && (bool)IgnoreBOMDifference) ? (Name, Media_type, Visibility, Etag, Last_modified.ToString()).GetHashCode() : (Name, Media_type, Visibility, Etag, Last_modified.ToString(), Size).GetHashCode();

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
