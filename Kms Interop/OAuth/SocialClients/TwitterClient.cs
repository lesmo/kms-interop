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
                    if ( !this.CurrentlyHasAccessToken )
                        throw new Exception("Must login first");

                    this._userName
                        = this.RequestJson(
                            HttpRequestMethod.GET,
                            "1.1/account/settings.json"
                        ).Response.SelectToken("$.screen_name").ToString();
                }

                return this._userName;
            }
        }
        private string _userName;
    }
}
