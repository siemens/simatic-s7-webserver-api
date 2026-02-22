using NUnit.Framework;
using System;

namespace Webserver.API.UnitTests
{
    public class TimeSpanAssumptions
    {
        [Test]
        public void TimeSpan_TotalMilliseconds_Returns_AllMilliseconds([Values(10, 100, 1000, 5000, 10_000, 20_000, 100_000)] int milliseconds)
        {
            var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            Assert.That(timeSpan.TotalMilliseconds, Is.EqualTo(milliseconds));
        }
    }
}
