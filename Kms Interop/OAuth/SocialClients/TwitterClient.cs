using Kms.Interop.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.OAuth.SocialClients {
    /// <summary>
    ///     Cliente del Twitter API
    /// </summary>
    public class TwitterClient : OAuth.OAuthClient, IOAuthSocialClient {
        public override String ProviderName {
            get {
                return "Twitter";
            }
        }
        
        /// <summary>
        ///     Crea una nueva instancia del Cliente del Twitter API.
        /// </summary>
        /// <param name="consumer">
        ///     Conjunto de API-Key y API-Secret.
        /// </param>
        /// <param name="token">
        ///     Conjunto de Token y Token Secret.
        /// </param>
        public TwitterClient(
            OAuthCryptoSet consumer,
            OAuthCryptoSet token = null,
            Uri callbackUri = null
        ) : base(
            new OAuthClientUris() {
                BaseUri
                    = new Uri("https://api.twitter.com/"),
                AuthorizationResource
                    = "oauth/authorize",
                ExchangeTokenResource
                    = "oauth/access_token",
                RequestTokenResource
                    = "oauth/request_token",
                CallbackRequestTokenUri
                    = callbackUri
            },
            consumer,
            token
        ) {
        }

        public override OAuthCryptoSet ExchangeRequestToken(string verifier) {
            base.ExchangeRequestToken(verifier);

            this.UserName.Count(); // forzar get de UserName

            return this.Token;
        }

        /// <summary>
        ///     Devuelve el Nombre de Usuario (Handle) en Twitter.
        /// </summary>
        public string UserName {
            get {
                if ( this._userName == null ) {
                    ValidateSession();

                    if ( this._userName == null )
                        throw new OAuthUnexpectedResponse();
                }

                return this._userName;
            }
        }
        private string _userName;

        public string UserID {
            get {
                if ( this._userID == null && ! this.ValidateSession() )
                        throw new OAuthUnexpectedResponse();

                return this._userID;
            }
        }
        private string _userID;

        public bool ValidateSession() {
            if ( !this.CurrentlyHasAccessToken )
                throw new Exception("Must login first");

            var jsonResponse = this.RequestJson(
                HttpRequestMethod.GET,
                "1.1/account/verify_credentials.json"
            ).Response;

            this._userName = jsonResponse.SelectToken("$.screen_name").ToString();
            this._userID   = jsonResponse.SelectToken("$.id").ToString();

            return !(
                string.IsNullOrEmpty(this._userName)
                || string.IsNullOrEmpty(this._userID)
            );
        }
    }
}
