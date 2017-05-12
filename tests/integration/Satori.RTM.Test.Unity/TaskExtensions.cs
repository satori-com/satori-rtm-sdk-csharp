using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satori.Rtm.Test
{
    public static class TaskExtensions
    {
        public static IEnumerator Await(this Task task)
        {
            while(!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}
