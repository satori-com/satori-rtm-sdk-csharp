//
// System.Net.Security.SslStream.cs
//
// Authors:
//  Tim Coleman (tim@timcoleman.com)
//  Atsushi Enomoto (atsushi@ximian.com)
//  Marek Safar (marek.safar@gmail.com)
//
// Copyright (C) Tim Coleman, 2004
// (c) 2004,2007 Novell, Inc. (http://www.novell.com)
// Copyright 2011 Xamarin Inc.
//

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

using X509CertificateCollection = System.Security.Cryptography.X509Certificates.X509CertificateCollection;

using System.Security.Authentication;

#if NET_4_5
using System.Threading.Tasks;
#endif

namespace System.Net.Security 
{
    public static class SslStreamExtensions 
    {
        #if NET_4_5

        public static Task AuthenticateAsClientAsync (this SslStream stream, string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            var t = Tuple.Create (targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, stream);

            return Task.Factory.FromAsync ((callback, state) => {
                var d = (Tuple<string, X509CertificateCollection, SslProtocols, bool, SslStream>) state;
                return d.Item5.BeginAuthenticateAsClient (d.Item1, d.Item2, d.Item3, d.Item4, callback, null);
            }, stream.EndAuthenticateAsClient, t);
        }

        #endif
    }
}
