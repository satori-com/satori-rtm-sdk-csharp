using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satori.RTM.Test.Unity
{
    public class UnityTestRunner
    {
        public static void Run()
        {
            //this is needed only once
            //CoreExtensions.Host.InstallBuiltins();

            //TestStatus suit = new TestSuite(new NullListener(), TestFilter.Empty);
        }
    }
}
