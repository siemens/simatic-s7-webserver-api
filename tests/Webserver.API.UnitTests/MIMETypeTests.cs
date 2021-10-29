// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webserver.API.UnitTests
{
    public class MIMETypeTests
    {
        [Test]
        public void GetMIMETypeDefaultsTo_ApplicationOctetStream()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping("");
            if(res != "application/octet-stream")
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetMIMETypeHTML_textHtml()
        {
            var res = MimeMapping.MimeUtility.GetMimeMapping(".html");
            if (res != "text/html")
            { 
                Assert.Fail();
            }
        }

        [Test]
        public void GetFileExtensionDefaultsTo_Txt()
        {
            var res = MimeMapping.MimeUtility.GetExtensions("text/plain");
            if (!res.Any(ext => ext == "txt"))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetFileExtensionTextHtml_htm()
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
