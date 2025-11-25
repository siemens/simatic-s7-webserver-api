// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Represents the comment text of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeCommentText
    {
        /// <summary>
        /// The text containing the comment
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeCommentText text &&
                   Text == text.Text;
        }

        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Text).GetHashCode();
        }

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Represents the comment of a node accessible using its HWID
    /// </summary>
    public class ModulesNodeComment
    {
        /// <summary>
        /// Comment of the node
        /// </summary>
        public ModulesNodeCommentText Comment { get; set; }

        /// <summary>
        /// The language of the comment
        /// </summary>
        public string Language { get; set; }


        /// <summary>
        /// Check if incoming object is the same as this
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they match</returns>
        public override bool Equals(object obj)
        {
            return obj is ModulesNodeComment comment &&
                   EqualityComparer<ModulesNodeCommentText>.Default.Equals(Comment, comment.Comment) &&
                   Language == comment.Language;
        }


        /// <summary>
        /// Get hashcode of object
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return (Comment, Language).GetHashCode();
        }

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
