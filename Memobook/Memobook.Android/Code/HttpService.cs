using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Javax.Net.Ssl;
using Memobook.Droid.Code;
using Memobook.Interfaces;
using Xamarin.Android.Net;
using Xamarin.Forms;
[assembly: Dependency(typeof(HTTPClientHandlerCreationService_Android))]
namespace Memobook.Droid.Code
{
    
        public class HTTPClientHandlerCreationService_Android : IHTTPClientHandlerCreationService
    {
            public HttpClientHandler GetInsecureHandler()
            {
                return new IgnoreSSLClientHandler();
            }
        }

        internal class IgnoreSSLClientHandler : AndroidClientHandler
        {
            protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
            {
                return SSLCertificateSocketFactory.GetInsecure(1000, null);
            }

            protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
            {
                return new IgnoreSSLHostnameVerifier();
            }
        }

        internal class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
        {
            public bool Verify(string hostname, ISSLSession session)
            {
                return true;
            }
        }
    }
