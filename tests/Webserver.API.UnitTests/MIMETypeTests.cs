﻿// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using System.Linq;

namespace Webserver.API.UnitTests
{
    public class MIMETypeTests
    {
        [Test]
        public void GetMIMEType_Defaults_ApplicationOctetStream()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping("");
            if (res != "application/octet-stream")
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetMIMEType_HTML_textHtml()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            if (res != "text/html")
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetFileExtension_DefaultsTo_Txt()
        {
            var res = MimeMapping.MimeUtility.GetExtensions("text/plain");
            if (!res.Any(ext => ext == "txt"))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetFileExtension_TextHtml_htm()
        {
            var res = MimeMapping.MimeUtility.GetExtensions("text/html");
            // CARE - getfileext will take the first match it will find - cannot determine what the original file was!
            if (!res.Any(ext => ext == "htm"))
            {
                Assert.Fail();
            }
        }


        [Test]
        public void MicrosoftMimeMapping()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            if (res != "text/html")
            {
                Assert.Fail();
            }
            //MimeMapping.GetMimeMapping("");
            res = MimeMapping.MimeUtility.GetMimeMapping("asdf.html");
            res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            res = MimeMapping.MimeUtility.GetMimeMapping("");
        }

    }
}
