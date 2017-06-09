#pragma warning disable 1591

using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Satori.Rtm
{
    /// <summary>
    /// This class validates a trust chain against the root certificate, stored in a static field. 
    /// This helper was introduced to allow a chain validation on Unity. 
    /// Unity trust store is empty so it doesn't contain required root certificates. 
    /// </summary>
    internal class ChainValidationHelper
    {
        // Subject: DN: C=US, O=GeoTrust Inc., CN=GeoTrust Global CA
        // Self-signed
        private const string GeoTrustGlobalCACert = @"-----BEGIN CERTIFICATE-----
MIIDVDCCAjygAwIBAgIDAjRWMA0GCSqGSIb3DQEBBQUAMEIxCzAJBgNVBAYTAlVT
MRYwFAYDVQQKEw1HZW9UcnVzdCBJbmMuMRswGQYDVQQDExJHZW9UcnVzdCBHbG9i
YWwgQ0EwHhcNMDIwNTIxMDQwMDAwWhcNMjIwNTIxMDQwMDAwWjBCMQswCQYDVQQG
EwJVUzEWMBQGA1UEChMNR2VvVHJ1c3QgSW5jLjEbMBkGA1UEAxMSR2VvVHJ1c3Qg
R2xvYmFsIENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2swYYzD9
9BcjGlZ+W988bDjkcbd4kdS8odhM+KhDtgPpTSEHCIjaWC9mOSm9BXiLnTjoBbdq
fnGk5sRgprDvgOSJKA+eJdbtg/OtppHHmMlCGDUUna2YRpIuT8rxh0PBFpVXLVDv
iS2Aelet8u5fa9IAjbkU+BQVNdnARqN7csiRv8lVK83Qlz6cJmTM386DGXHKTubU
1XupGc1V3sjs0l44U+VcT4wt/lAjNvxm5suOpDkZALeVAjmRCw7+OC7RHQWa9k0+
bw8HHa8sHo9gOeL6NlMTOdReJivbPagUvTLrGAMoUgRx5aszPeE4uwc2hGKceeoW
MPRfwCvocWvk+QIDAQABo1MwUTAPBgNVHRMBAf8EBTADAQH/MB0GA1UdDgQWBBTA
ephojYn7qwVkDBF9qn1luMrMTjAfBgNVHSMEGDAWgBTAephojYn7qwVkDBF9qn1l
uMrMTjANBgkqhkiG9w0BAQUFAAOCAQEANeMpauUvXVSOKVCUn5kaFOSPeCpilKIn
Z57QzxpeR+nBsqTP3UEaBU6bS+5Kb1VSsyShNwrrZHYqLizz/Tt1kL/6cdjHPTfS
tQWVYrmm3ok9Nns4d0iXrKYgjy6myQzCsplFAMfOEVEiIuCl6rYVSAlk6l5PdPcF
PseKUgzbFbS9bZvlxrFUaKnjaZC2mqUPuLk/IH2uSrW4nOQdtqvmlKXBx4Ot2/Un
hw4EbNX/3aBd7YdStysVAq45pmp06drE57xNNB6pXE0zX5IJL4hmXXeXxx12E6nV
5fEWCRE11azbJHFwLJhWC9kXtNHjUStedejV0NxPNO3CBWaAocvmMw==
-----END CERTIFICATE-----";

        // Subject: C=US/O=GeoTrust Inc./CN=GeoTrust SSL CA - G3
        // Issuer: C=US/O=GeoTrust Inc./CN= GeoTrust Global CA
        private const string GeoTrustCAG3Cert = @"-----BEGIN CERTIFICATE-----
MIIETzCCAzegAwIBAgIDAjpvMA0GCSqGSIb3DQEBCwUAMEIxCzAJBgNVBAYTAlVT
MRYwFAYDVQQKEw1HZW9UcnVzdCBJbmMuMRswGQYDVQQDExJHZW9UcnVzdCBHbG9i
YWwgQ0EwHhcNMTMxMTA1MjEzNjUwWhcNMjIwNTIwMjEzNjUwWjBEMQswCQYDVQQG
EwJVUzEWMBQGA1UEChMNR2VvVHJ1c3QgSW5jLjEdMBsGA1UEAxMUR2VvVHJ1c3Qg
U1NMIENBIC0gRzMwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDjvn4K
hqPPa209K6GXrUkkTdd3uTR5CKWeop7eRxKSPX7qGYax6E89X/fQp3eaWx8KA7UZ
U9ulIZRpY51qTJEMEEe+EfpshiW3qwRoQjgJZfAU2hme+msLq2LvjafvY3AjqK+B
89FuiGdT7BKkKXWKp/JXPaKDmJfyCn3U50NuMHhiIllZuHEnRaoPZsZVP/oyFysx
j0ag+mkUfJ2fWuLrM04QprPtd2PYw5703d95mnrU7t7dmszDt6ldzBE6B7tvl6QB
I0eVH6N3+liSxsfQvc+TGEK3fveeZerVO8rtrMVwof7UEJrwEgRErBpbeFBFV0xv
vYDLgVwts7x2oR5lAgMBAAGjggFKMIIBRjAfBgNVHSMEGDAWgBTAephojYn7qwVk
DBF9qn1luMrMTjAdBgNVHQ4EFgQU0m/3lvSFP3I8MH0j2oV4m6N8WnwwEgYDVR0T
AQH/BAgwBgEB/wIBADAOBgNVHQ8BAf8EBAMCAQYwNgYDVR0fBC8wLTAroCmgJ4Yl
aHR0cDovL2cxLnN5bWNiLmNvbS9jcmxzL2d0Z2xvYmFsLmNybDAvBggrBgEFBQcB
AQQjMCEwHwYIKwYBBQUHMAGGE2h0dHA6Ly9nMi5zeW1jYi5jb20wTAYDVR0gBEUw
QzBBBgpghkgBhvhFAQc2MDMwMQYIKwYBBQUHAgEWJWh0dHA6Ly93d3cuZ2VvdHJ1
c3QuY29tL3Jlc291cmNlcy9jcHMwKQYDVR0RBCIwIKQeMBwxGjAYBgNVBAMTEVN5
bWFudGVjUEtJLTEtNTM5MA0GCSqGSIb3DQEBCwUAA4IBAQCg1Pcs+3QLf2TxzUNq
n2JTHAJ8mJCi7k9o1CAacxI+d7NQ63K87oi+fxfqd4+DYZVPhKHLMk9sIb7SaZZ9
Y73cK6gf0BOEcP72NZWJ+aZ3sEbIu7cT9clgadZM/tKO79NgwYCA4ef7i28heUrg
3Kkbwbf7w0lZXLV3B0TUl/xJAIlvBk4BcBmsLxHA4uYPL4ZLjXvDuacu9PGsFj45
SVGeF0tPEDpbpaiSb/361gsDTUdWVxnzy2v189bPsPX1oxHSIFMTNDcFLENaY9+N
QNaFHlHpURceA1bJ8TCt55sRornQMYGbaLHZ6PPmlH7HrhMvh+3QJbBo+d4IWvMp
zNSS
-----END CERTIFICATE-----";

        public static Logger Log { get; } = Client.DefaultLoggers.Serialization;

        /// <summary>
        /// This methods overrides chain validation. 
        /// </summary>
        /// <returns>true if valid</returns>
        public static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain oldChain, SslPolicyErrors sslPolicyErrors)
        {
            if (certificate == null || oldChain == null)
            {
                Log.W("Certificate validation callback is called with null args: certificate={0}, oldChain={1}", certificate, oldChain);
                return false;
            }

            if (sslPolicyErrors != SslPolicyErrors.None
                && sslPolicyErrors != SslPolicyErrors.RemoteCertificateChainErrors
                && sslPolicyErrors != SslPolicyErrors.RemoteCertificateNotAvailable
                && sslPolicyErrors != (SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateChainErrors))
            {
                Log.E("Certificate validation failed with ssl policy errors: {0}", sslPolicyErrors);
                return false;
            }

            // Customize only chain validation
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                byte[] rawRootCert = Encoding.ASCII.GetBytes(GeoTrustGlobalCACert);
                X509Certificate2 rootCert = new X509Certificate2(rawRootCert);

                byte[] rawIntermCert = Encoding.ASCII.GetBytes(GeoTrustCAG3Cert);
                X509Certificate2 intermCert = new X509Certificate2(rawIntermCert);

                X509Chain newChain = new X509Chain();
                newChain.ChainPolicy.RevocationMode = oldChain.ChainPolicy.RevocationMode;
                newChain.ChainPolicy.RevocationFlag = oldChain.ChainPolicy.RevocationFlag;
                newChain.ChainPolicy.VerificationFlags = oldChain.ChainPolicy.VerificationFlags;
                newChain.ChainPolicy.UrlRetrievalTimeout = oldChain.ChainPolicy.UrlRetrievalTimeout;
                newChain.ChainPolicy.ExtraStore.Add(rootCert);
                newChain.ChainPolicy.ExtraStore.Add(intermCert);
                newChain.ChainPolicy.ExtraStore.AddRange(oldChain.ChainPolicy.ExtraStore);

                foreach (var cert in newChain.ChainPolicy.ExtraStore)
                {
                    Log.V("Certificate in the extra store: subject={0}, issuer={1}", cert.Subject, cert.Issuer);
                }
                
                if (!newChain.Build(new X509Certificate2(certificate)))
                {
                    X509Certificate chainRootCert = null;
                    if (newChain.ChainElements.Count > 0)
                    {
                        chainRootCert = newChain.ChainElements[newChain.ChainElements.Count - 1].Certificate;
                    }

                    // Ignore only UntrustedRoot status for our root certificate. 
                    // Root certificates, that are not in roots trusted store, marked as UntrustedRoot. 
                    // In order to add a certificate to the trusted store, one needs additional priviliges. 
                    if (newChain.ChainStatus.Length == 1
                        && newChain.ChainStatus.First().Status == X509ChainStatusFlags.UntrustedRoot
                        && rootCert == chainRootCert)
                    {
                        Log.V("UntrustedRoot status is ignored because we trust this root certificate");
                    }
                    else
                    {
                        foreach (var status in newChain.ChainStatus)
                        {
                            if (status.Status != X509ChainStatusFlags.NoError)
                            {
                                Log.E("Chain building failed with status {0}: {1}", status.Status, status.StatusInformation);
                            }
                        }

                        return false;
                    }
                }
            }

            return true;
        }
    }
}
