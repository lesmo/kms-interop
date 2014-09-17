using System;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Kms.Interop.OAuth.SocialClients {

    /// <summary>
    ///     Cliente del Twitter API
    /// </summary>
    public class TwitterClient : OAuthClient, IOAuthSocialClient {

        private string _userID;
        private string _userName;

        /// <summary>
        ///     Crea una nueva instancia del Cliente del Twitter API.
        /// </summary>
        /// <param name="consumer">
        ///     Conjunto de API-Key y API-Secret.
        /// </param>
        /// <param name="token">
        ///     Conjunto de Token y Token Secret.
        /// </param>
        public TwitterClient(OAuthCryptoSet consumer, OAuthCryptoSet token = null, Uri callbackUri = null)
            : base(new OAuthClientUris {
                BaseUri = new Uri("https://api.twitter.com/"),
                AuthorizationResource = "oauth/authorize",
                ExchangeTokenResource = "oauth/access_token",
                RequestTokenResource = "oauth/request_token",
                CallbackRequestTokenUri = callbackUri
            },
                consumer,
                token) {}

        public override String ProviderName {
            get { return "Twitter"; }
        }

        /// <summary>
        ///     Devuelve el Nombre de Usuario (Handle) en Twitter.
        /// </summary>
        public string UserName {
            get {
                if ( _userName != null )
                    return _userName;
                
                ValidateSession();

                if ( _userName == null )
                    throw new OAuthUnexpectedResponse();
                
                return _userName;
            }
        }

        public string UserID {
            get {
                if ( _userID == null && ! ValidateSession() )
                    throw new OAuthUnexpectedResponse();

                return _userID;
            }
        }

        public bool ValidateSession() {
            if ( !CurrentlyHasAccessToken )
                throw new Exception("Must login first");

            var jsonResponse = RequestJson(HttpRequestMethod.GET, "1.1/account/verify_credentials.json").Response;

            _userName = jsonResponse.SelectToken("$.screen_name").ToString();
            _userID = jsonResponse.SelectToken("$.id").ToString();

            return !(string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_userID));
        }

        public override OAuthCryptoSet ExchangeRequestToken(string verifier) {
            base.ExchangeRequestToken(verifier);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            UserName.Count(); // forzar get de UserName

            return Token;
        }
    }

}