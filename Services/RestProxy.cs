using System;
using System.Net;

namespace PodNoms.Api.Services
{
   public class RestProxy : IWebProxy
    {
        public RestProxy(string proxyUri)
            : this(new Uri(proxyUri))
        {
        }

        public RestProxy(Uri proxyUri)
        {
            this.ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public System.Net.ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return this.ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false; /* Proxy all requests */
        }
    }
}