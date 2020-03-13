using System;

using Xamarin.Auth;

namespace Memobook.Models
{
   
        public class AuthenticationState
        {
            /// <summary>
            /// The authenticator.
            /// </summary>
            // TODO:
            // Oauth1Authenticator inherits from WebAuthenticator
            // Oauth2Authenticator inherits from WebRedirectAuthenticator
            public static WebAuthenticator Authenticator;
        }
    }
