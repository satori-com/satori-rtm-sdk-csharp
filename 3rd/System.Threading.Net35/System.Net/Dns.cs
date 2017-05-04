// System.Net.Dns.cs
//
// Authors:
//  Mads Pultz (mpultz@diku.dk)
//  Lawrence Pit (loz@cable.a2000.nl)

// Author: Mads Pultz (mpultz@diku.dk)
//     Lawrence Pit (loz@cable.a2000.nl)
//     Marek Safar (marek.safar@gmail.com)
//     Gonzalo Paniagua Javier (gonzalo.mono@gmail.com)
//
// (C) Mads Pultz, 2001
// Copyright (c) 2011 Novell, Inc.
// Copyright (c) 2011 Xamarin, Inc.

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

using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
#if NET_4_5
using System.Threading.Tasks;
#endif

#if !MOBILE
using Mono.Net.Dns;
#endif

namespace System.Net 
{
    public static class DnsExtensions 
    {        
        #if NET_4_5
        public static Task<IPAddress[]> GetHostAddressesAsync (string hostNameOrAddress)
        {
            return Task<IPAddress[]>.Factory.FromAsync (Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses, hostNameOrAddress, null);
        }

        public static Task<IPHostEntry> GetHostEntryAsync (string hostNameOrAddress)
        {
            return Task<IPHostEntry>.Factory.FromAsync (Dns.BeginGetHostEntry, Dns.EndGetHostEntry, hostNameOrAddress, null);
        }
        #endif
    }
}

