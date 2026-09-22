// Copyright (c) 2026, Siemens AG
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
            Assert.That(res, Is.EqualTo("application/octet-stream"));
        }

        [Test]
        public void GetMIMEType_HTML_textHtml()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            Assert.That(res, Is.EqualTo("text/html"));
        }

        [Test]
        public void GetFileExtension_DefaultsTo_Txt()
        {
            var res = MimeMapping.MimeUtility.GetExtensions("text/plain");
            Assert.That(res, Contains.Item("txt"));
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
            Assert.That(res, Contains.Item("htm"));
            if (!res.Any(ext => ext == "htm"))
            {
                Assert.Fail();
            }
        }


        [Test]
        public void MicrosoftMimeMapping()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            Assert.That(res, Is.EqualTo("text/html"));
            res = MimeMapping.MimeUtility.GetMimeMapping("asdf.html");
            Assert.That(res, Is.EqualTo("text/html"));
            res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            Assert.That(res, Is.EqualTo("text/html"));
            res = MimeMapping.MimeUtility.GetMimeMapping("");
            Assert.That(res, Is.EqualTo("application/octet-stream"));
        }

    }
}
