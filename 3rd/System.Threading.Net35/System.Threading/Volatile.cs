using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading
{
    public static class Volatile
    {
        public static T Read<T>(ref T location)
        where T : class {
            Object temp = location;
            return (T) Thread.VolatileRead(ref temp);
        }
    }
}
