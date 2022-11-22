#if NET6_0_OR_GREATER
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Siemens.Simatic.S7.Webserver.API.Services
{
    /// <summary>
    /// Provides access to custom server certificate callback for handling SSL errors.
    /// </summary>
    public class ServerCertificateCallback
    {        
        /// <summary>
        /// Gets or sets custom server certificate callback.
        /// </summary>
       public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>? CertificateCallback
        {
            get;
            set;
        }    
    }
}
#endif