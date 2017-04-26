// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Helpers for assemblies that don't yet depend on the System.Threading.Tasks contract
    /// that includes Task.CompletedTask, Task.FromCanceled, and Task.FromException.
    /// </summary>
    internal static class TaskHelpers
    {
        private struct VoidTaskResult { }

        internal static Task FromException (Exception e)
        {
            return FromException<VoidTaskResult>(e);
        }

        internal static Task<T> FromException<T> (Exception e)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(e);
            return tcs.Task;
        }

        internal static Task CompletedTask ()
        {
            return s_completedTask ?? (s_completedTask = Task.FromResult(default(VoidTaskResult)));
        }

        private static Task s_completedTask;
    }
}
