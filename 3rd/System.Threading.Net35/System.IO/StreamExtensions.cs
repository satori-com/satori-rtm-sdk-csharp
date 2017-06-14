//
// System.IO.Stream.cs
//
// Authors:
//   Dietmar Maurer (dietmar@ximian.com)
//   Miguel de Icaza (miguel@ximian.com)
//   Gonzalo Paniagua Javier (gonzalo@ximian.com)
//   Marek Safar (marek.safar@gmail.com)
//
// (C) 2001, 2002 Ximian, Inc.  http://www.ximian.com
// (c) 2004 Novell, Inc. (http://www.novell.com)
// Copyright 2011 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
#if NET_4_5
using System.Threading.Tasks;
#endif

namespace System.IO
{
    public static class StreamExtensions 
    {
        #if NET_4_5
        public static Task<int> ReadAsync (this Stream stream, byte[] buffer, int offset, int count)
        {
            return stream.ReadAsync (buffer, offset, count, CancellationToken.None);
        }

        public static Task<int> ReadAsync (this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return TaskConstants<int>.Canceled;

            return Task<int>.Factory.FromAsync (stream.BeginRead, stream.EndRead, buffer, offset, count, null);
        }

        public static Task WriteAsync (this Stream stream, byte[] buffer, int offset, int count)
        {
            return stream.WriteAsync (buffer, offset, count, CancellationToken.None);
        }

        public static Task WriteAsync (this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync (stream.BeginWrite, stream.EndWrite, buffer, offset, count, null);
        }
        #endif
    }
}