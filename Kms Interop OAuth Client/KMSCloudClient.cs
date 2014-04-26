using Kms.Interop.OAuth.Utils;
using Kms.Interop.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Kms.Interop.OAuth.SocialClients;

namespace Kms.Interop.CloudClient {
    public class KMSCloudClient : OAuthClient {
        public KMSCloudClient(
            KMSCloudUris clientUris,
            OAuthCryptoSet consumer,
            OAuthCryptoSet token = null
        ) : base(clientUris, consumer, token) {
        }

        /// <summary>
        ///     Registra una nueva cuenta en la Nube de KMS.
        /// </summary>
        /// <param name="accountData">
        ///     Información de la cuenta a registrar.
        /// </param>
        public OAuthCryptoSet RegisterAccount(Dictionary<string, string> accountData) {
            if ( this.Token != null ) 
                throw new KMSScrewYou();

            this.GetRequestToken();

            NameValueCollection parameters
                = new NameValueCollection();

            parameters.AddFromDictionary(accountData);

            OAuthResponse<NameValueCollection> response = this.RequestSimpleNameValue(
                HttpRequestMethod.POST,
                (this.ClientUris as KMSCloudUris).KmsRegisterAccountResource,
                parameters,
                null,
                new Dictionary<HttpRequestHeader, string>() {
                    {HttpRequestHeader.AcceptLanguage, CultureInfo.CurrentCulture.Name}
                }
            );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                OAuthCryptoSet tokenSet
                    = new OAuthCryptoSet(
                        response.Response.Get("oauth_token"),
                        response.Response.Get("oauth_token_secret")
                    );

                if (
                    string.IsNullOrEmpty(tokenSet.Key)
                    || string.IsNullOrEmpty(tokenSet.Secret) 
                ) {
                    throw new KMSScrewYou("BAM, BAM, BAM 3.4");
                } else {
                    this.Token
                        = tokenSet;
                    return tokenSet;
                }
            } else {
                if ( string.IsNullOrEmpty(response.Headers[HttpResponseHeader.Warning]) ) {
                    throw new KMSScrewYou(response.RawResponse);
                } else {
                    throw new KMSScrewYou(response.Headers[HttpResponseHeader.Warning]);
                }
            }
        }

        /// <summary>
        ///     Determina si la sesión actual es válida.
        /// </summary>
        /// <returns>
        ///     TRUE si la sesión es válida, FALSE de lo contrario.
        /// </returns>
        public bool SessionIsValid() {
            if ( this.Token == null )
                return false;
            if ( this.CurrentlyHasAccessToken )
                return true;

            try {
                OAuthResponse<string> response
                    = this.RequestString(
                        HttpRequestMethod.GET,
                        (this.ClientUris as KMSCloudUris).KmsSessionResource
                    );
                if ( response.StatusCode == HttpStatusCode.OK ) {
                    this.CurrentlyHasAccessToken
                        = true;

                    return true;
                } else {
                    return false;
                }
            } catch ( OAuthUnauthorized ) {
                return false;
            }
        }
        
        /// <summary>
        ///     Realizar un Inicio de Sesión básico (con Email y Contraseña del Usuario).
        /// </summary>
        /// <param name="email">Email del Usuario registrado en KMS</param>
        /// <param name="password">Contraseña del Usuario registrado en KMS (texto plano)</param>
        /// <returns>
        ///     Conjunto de Token y Token Secret para peticiones subsecuentes. Automáticamente se
        ///     establecen éstos valores en ésta instancia del Cliente OAuth.
        /// </returns>
        public OAuthCryptoSet LoginBasic(string email, string password) {
            Uri basicLoginUri
                = this.GetAuthorizationUri();
            HttpWebRequest request
                = (HttpWebRequest)WebRequest.Create(basicLoginUri);
            byte[] authorizationLoginBytes
                = Encoding.ASCII.GetBytes(
                    email + ":" +password
                );
            string authorizationLoginString
                = Convert.ToBase64String(authorizationLoginBytes);

            request.Method
                = "POST";
            request.ContentLength
                = 0;
            request.Headers.Add(
                HttpRequestHeader.Authorization,
                "Basic " + authorizationLoginString
            );
            request.Headers.Add(
                HttpRequestHeader.AcceptLanguage,
                CultureInfo.CurrentCulture.Name
            );

            HttpWebResponse response;
            try {
                response
                    = (HttpWebResponse)request.GetResponse();
            } catch ( WebException ex )  {
                response
                    = (HttpWebResponse)ex.Response;
            }

            if ( response.StatusCode == HttpStatusCode.Unauthorized ) {
                throw new KMSWrongUserCredentials();
            } else if ( response.StatusCode == HttpStatusCode.Forbidden ) {
                throw new KMSScrewYou(
                    response.Headers[HttpRequestHeader.Warning]
                );
            } else if (
                response.StatusCode != HttpStatusCode.OK
                && response.StatusCode != HttpStatusCode.Created
            ) {
                throw new KMSScrewYou();
            }

            StringBuilder responseStringBuilder
                = new StringBuilder();
            Stream responseStream
                = response.GetResponseStream();

            byte[] streamBuffer
                = new byte[8192];
            int count
                = 0;
            do {
                count
                    = responseStream.Read(
                        streamBuffer,
                        0,
                        streamBuffer.Length
                    );

                if ( count > 0 )
                    responseStringBuilder.Append(
                        Encoding.ASCII.GetString(
                            streamBuffer,
                            0,
                            count
                        )
                    );
            } while ( count > 0 );

            return this.ExchangeRequestToken(
                responseStringBuilder.ToString().Trim()
            );
        }

        /// <summary>
        ///     Realizar un Inicio de Sesión en la Nube de KMS con un Servicio OAuth de Terceros.
        /// </summary>
        /// <param name="oAuthClient">
        ///     Objeto de Cliente IOAuthSocial.
        /// </param>
        /// <remarks>
        ///     Facebook, y todos los futuros servicios que implementan OAuth 2.0 no utilizan el Token
        ///     Secret del protocolo OAuth 1.0a, por lo que DEBE omitirase en esos casos.
        /// </remarks>
        /// <returns>
        ///     Devuelve el conjunto de Token y Token Secret generados por la Nube de KMS. El conjunto
        ///     también se establece automáticamente de forma interna para peticiones subsecuentes.
        /// </returns>
        public OAuthCryptoSet Login3rdParty<T>(T oAuthClient) where T : IOAuthSocialClient {
            if ( this.Token != null || this.ConsumerCredentials == null )
                throw new OAuthUnexpectedRequest();

            NameValueCollection requestParameters;
            
            if ( oAuthClient is FacebookClient ) {
                requestParameters = 
                    new NameValueCollection {
                        {"ID", oAuthClient.UserID},
                        {"Code", (oAuthClient as FacebookClient).Code}
                    };
            } else {
                requestParameters =
                    new NameValueCollection {
                        {"Token", oAuthClient.Token.Key},
                        {"TokenSecret", oAuthClient.Token.Secret},
                        {"ID", oAuthClient.UserID}
                    };
            }

            string requestUrl = string.Format(
                (this.ClientUris as KMSCloudUris).KmsOAuth3rdLogin,
                oAuthClient.ProviderName.ToLower(CultureInfo.InvariantCulture)
            );

            OAuthResponse<NameValueCollection> response =
                this.RequestSimpleNameValue(
                    HttpRequestMethod.POST,
                    requestUrl,
                    requestParameters
                );

            if (
                response.StatusCode != HttpStatusCode.OK
                || response.StatusCode != HttpStatusCode.Created
                || response.StatusCode != HttpStatusCode.NotFound
            ) {
                throw new KMSScrewYou();
            }

            if ( response.StatusCode == HttpStatusCode.NotFound )
                throw new KMSWrongUserCredentials();

            string kmsOAuthToken
                = response.Response.Get("oauth_token");
            string kmsOAuthTokenSecret
                = response.Response.Get("oauth_token_secret");

            if ( string.IsNullOrEmpty(kmsOAuthToken) || string.IsNullOrEmpty(kmsOAuthTokenSecret) )
                throw new KMSScrewYou();

            this.Token
                = new OAuthCryptoSet(
                    kmsOAuthToken,
                    kmsOAuthTokenSecret
                );

            return this.Token;
        }

        /// <summary>
        ///     Realizar un Inicio de Sesión en la Nube de KMS con un Servicio OAuth de Terceros.
        /// </summary>
        /// <param name="oAuthClient">
        ///     Objeto de Cliente IOAuthSocial.
        /// </param>
        /// <remarks>
        ///     Facebook, y todos los futuros servicios que implementan OAuth 2.0 no utilizan el Token
        ///     Secret del protocolo OAuth 1.0a, por lo que DEBE omitirase en esos casos.
        /// </remarks>
        /// <returns>
        ///     Devuelve el conjunto de Token y Token Secret generados por la Nube de KMS. El conjunto
        ///     también se establece automáticamente de forma interna para peticiones subsecuentes.
        /// </returns>
        public OAuthCryptoSet Login3rdParty(object oAuthClient) {
            if ( oAuthClient is FacebookClient )
                return this.Login3rdParty<FacebookClient>(oAuthClient as FacebookClient);
            else if ( oAuthClient is TwitterClient )
                return this.Login3rdParty<TwitterClient>(oAuthClient as TwitterClient);
            else
                return this.Login3rdParty<IOAuthSocialClient>(oAuthClient as IOAuthSocialClient);
        }

        /// <summary>
        ///     Realizar un Cierre de Sesión de la Nube de KMS.
        /// </summary>
        /// <returns>
        ///     Devuelve si se logró cerrar la sesión del Usuario sin problemas.
        /// </returns>
        public bool Logout() {
            if ( this.Token != null || this.ConsumerCredentials == null )
                throw new OAuthUnexpectedRequest();

            OAuthResponse<string> response
                = this.RequestString(
                    HttpRequestMethod.DELETE,
                    (this.ClientUris as KMSCloudUris).KmsSessionResource
                );

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public void AddOAuth3rd<T>(T oAuthClient) where T : IOAuthSocialClient {
            if ( this.Token == null || this.ConsumerCredentials == null )
                throw new OAuthUnexpectedRequest();

            NameValueCollection requestParameters;

            if ( oAuthClient is FacebookClient ) {
                requestParameters =
                    new NameValueCollection {
                        {"ID", oAuthClient.UserID},
                        {"Code", (oAuthClient as FacebookClient).Code}
                    };
            } else {
                requestParameters =
                    new NameValueCollection {
                        {"Token", oAuthClient.Token.Key},
                        {"TokenSecret", oAuthClient.Token.Secret},
                        {"ID", oAuthClient.UserID}
                    };
            }

            string requestUrl = string.Format(
                (this.ClientUris as KMSCloudUris).KmsOAuth3rdAdd,
                oAuthClient.ProviderName.ToLower(CultureInfo.InvariantCulture)
            );

            OAuthResponse<string> response = this.RequestString(
                HttpRequestMethod.POST,
                string.Format(
                    CultureInfo.InvariantCulture,
                    (this.ClientUris as KMSCloudUris).KmsOAuth3rdAdd,
                    oAuthClient.ProviderName.ToLower(CultureInfo.InvariantCulture)
                ),
                requestParameters
            );

            if (
                response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
                || response.StatusCode == HttpStatusCode.NotFound
            ) {
                return;
            } else {
                throw new KMSScrewYou();
            }
        }
    }
}
