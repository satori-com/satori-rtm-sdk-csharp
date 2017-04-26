#pragma warning disable 1591

using System;

namespace Satori.Common
{
    public partial class SuccessfulAwaiter<T>
    {
        private bool _isCompleted;

        public SuccessfulAwaiter()
        {
            _isCompleted = false;
        }

        public SuccessfulAwaiter(T result)
        {
            _isCompleted = true;
            Result = result;
        }

        public event Action Completed
        {
            add
            {
                if (_isCompleted)
                {
                    value();
                }
                else
                {
                    PrivateCompleted += value;
                }
            }

            remove
            {
                if (!_isCompleted)
                {
                    PrivateCompleted -= value;
                }
            }
        }

        private event Action PrivateCompleted;

        public T Result { get; private set; }

        private void NotifyOnCompleted()
        {
            if (PrivateCompleted != null)
            {
                foreach (Action cb in PrivateCompleted.GetInvocationList())
                {
                    try
                    {
                        cb();
                    }
                    catch (Exception exn)
                    {
                        UnhandledExceptionWatcher.Swallow(exn);
                    }
                }

                PrivateCompleted = null;
            }
        }
    }

    public partial class SuccessfulAwaiter<T> : ISuccessfulAwaiter<T>
    {
        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        public T GetResult()
        {
            return Result;
        }

        public void OnCompleted(Action cont)
        {
            Completed += cont;
        }
    }

    public partial class SuccessfulAwaiter<T> : ISuccessfulCompletionSink<T>
    {
        public bool Succeed(T result)
        {
            if (_isCompleted)
            {
                return false;
            }

            Result = result;
            _isCompleted = true;
            NotifyOnCompleted();
            return true;
        }
    }
}
