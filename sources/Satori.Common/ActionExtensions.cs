#pragma warning disable 1591

using System;

namespace Satori.Common
{
    public static class ActionExtensions
    {
        public static void InvokeSafe(this Action action)
        {
            if (action == null)
            {
                return;
            }

            foreach (Action a in action.GetInvocationList())
            {
                try
                {
                    a();
                }
                catch (Exception exn)
                {
                    UnhandledExceptionWatcher.Swallow(exn);
                }
            }
        }

        public static void InvokeSafe<T>(this Action<T> action, T p1)
        {
            if (action == null)
            {
                return;
            }

            foreach (Action<T> a in action.GetInvocationList())
            {
                try
                {
                    a(p1);
                }
                catch (Exception exn)
                {
                    UnhandledExceptionWatcher.Swallow(exn);
                }
            }
        }

        public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 p1, T2 p2)
        {
            if (action == null)
            {
                return;
            }

            foreach (Action<T1, T2> a in action.GetInvocationList())
            {
                try
                {
                    a(p1, p2);
                }
                catch (Exception exn)
                {
                    UnhandledExceptionWatcher.Swallow(exn);
                }
            }
        }
    }
}