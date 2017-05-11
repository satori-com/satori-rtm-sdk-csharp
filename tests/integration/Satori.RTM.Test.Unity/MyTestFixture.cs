using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satori.Rtm.Test
{
    public class MyTestFixture
    {
        public async Task MyTest()
        {
            await Task.Delay(1000);
            Assert.Fail("My test failed after delay");
        }
    }
}
