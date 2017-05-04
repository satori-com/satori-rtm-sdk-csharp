using System;
using System.Threading.Tasks;

namespace Satori.Rtm
{
    public class MyClass
    {
        public MyClass()
        { 
        }

        public async Task Func() 
        {
            await Task.Delay(1000);
        }
    }
}
