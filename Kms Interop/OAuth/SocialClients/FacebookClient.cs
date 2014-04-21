using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kms.Interop.OAuth.Utils;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Kms.Interop.OAuth.SocialClients {
    public enum OAuth2ResponseType {
        Code,
        Token,
        CodeAndToken
    }

    public class FacebookClient /*: OAuth2.OAuth2Client */ : IOAuthSocialClient {
        public OAuthClientUris ClientUris {
            get;
            set;
        }

        public OAuthCryptoSet ConsumerCredentials {
            get;
            set;
        }

        public  OAuthCryptoSet Token {
            get {
                return this._token;
            }
            set {
                this._token
                    = value;
                this.CurrentlyHasAccessToken
                    = this._token != null;
            }
        }
        private OAuthCryptoSet _token;

        /// <summary>
        ///     Token obtenido por el cliente en App Móvil o Escritorio.
        /// </summary>
        public string AppToken {
            get {
                if ( this.Token == null)
                    return null;
                else
                    return this.Token.Key;
            }
        }

        /// <summary>
        ///     Token obtenido por el cliente en Servidor.
        /// </summary>
        public string ServerToken {
            get {
                if ( this.Token == null )
                    return null;
                else
                    return this.Token.Secret;
            }
        }

        public string Code {
            get;
            private set;
        }

        public bool CurrentlyHasAccessToken {
            get;
            private set;
        }

        public FacebookClient(
            OAuthCryptoSet consumer,
            OAuthCryptoSet token
                = null,
            Uri callbackRequestTokenUri
                = null
        ) {
            this.ClientUris
                = new OAuthClientUris() {
                    BaseUri
                        = new Uri("https://graph.facebook.com/"),
                    AuthorizationResource
                        = "https://www.facebook.com/dialog/oauth",
                    RequestTokenResource
                        = "https://www.facebook.com/dialog/oauth",
                    ExchangeTokenResource
                        = "oauth/access_token",
                    CallbackRequestTokenUri
                        = callbackRequestTokenUri
                        ?? new Uri("https://www.facebook.com/connect/login_success.html"),
                };

            this.ConsumerCredentials
                = consumer;
            this.Token
                = token;

            if ( this.Token != null )
                this.CurrentlyHasAccessToken
                    = true;
        }

        private OAuth2ResponseType AuthorizationResponseType
            = OAuth2ResponseType.Token;
        public Uri GetAuthorizationUri(
            OAuth2ResponseType responseType
                = OAuth2ResponseType.Token,
            FacebookPermission[] requestPermissions
                = null
        ) {
            this.AuthorizationResponseType
                = responseType;

            string responseTypeString
                = "fuck";

            switch ( responseType ) {
                case OAuth2ResponseType.Token :
                    responseTypeString
                        = "token";
                    break;
                case OAuth2ResponseType.Code :
                    responseTypeString
                        = "token";
                    break;
                case OAuth2ResponseType.CodeAndToken :
                    responseTypeString
                        = "code%20token";
                    break;
            }

            if ( requestPermissions == null ) {
                requestPermissions = new FacebookPermission[] {
                    FacebookPermission.BasicInfo
                };
            } else if ( !requestPermissions.Contains(FacebookPermission.BasicInfo) ) {
                FacebookPermission[] requestPermissionsTemp
                    = new FacebookPermission[requestPermissions.Length + 1];
                requestPermissionsTemp[0]
                    = FacebookPermission.BasicInfo;

                requestPermissions.CopyTo(requestPermissionsTemp, 1);
                requestPermissions
                    = requestPermissionsTemp;
            }

            StringBuilder scopeStringBuilder
                = new StringBuilder();

            foreach ( FacebookPermission permission in requestPermissions ) {
                string permissionString
                    = permission.CamelCaseToUnderlineString();

                if ( permissionString.StartsWith("user_actions") ) {
                    permissionString.Remove(12, 1);
                    permissionString.Insert(12, ".");
                } else if ( permissionString.StartsWith("friends_actions") ) {
                    permissionString.Remove(15, 1);
                    permissionString.Insert(15, ".");
                }

                scopeStringBuilder.Append(permissionString);
                scopeStringBuilder.Append(',');
            }

            return new Uri(
                string.Format(
                    "{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}&display=popup",
                    this.ClientUris.AuthorizationResource,
                    this.ConsumerCredentials.Key,
                    Uri.EscapeDataString(
                        this.ClientUris.CallbackRequestTokenUri == null
                            ? "https://www.facebook.com/connect/login_success.html"
                            : this.ClientUris.CallbackRequestTokenUri.AbsoluteUri
                    ),
                    responseTypeString,
                    scopeStringBuilder.ToString().Remove(
                        scopeStringBuilder.Length - 1
                    )
                )
            );
        }

        public OAuthCryptoSet ExchangeCodeForToken(string code) {
            if ( string.IsNullOrEmpty(this.ConsumerCredentials.Secret) )
                throw new InvalidOperationException(
                    "Current Facebook Client configuration does not include Client Secret."
                );
            if ( this.Token != null )
                throw new InvalidOperationException(
                    "Current Facebook Client configuration already contains a Token exchanged obtained via Code."
                );

            OAuthResponse<NameValueCollection> response
                = this.RequestSimpleNameValue(
                    HttpRequestMethod.GET,
                    string.Format(
                        "oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                        this.ConsumerCredentials.Key,
                        Uri.EscapeDataString(
                            this.ClientUris.CallbackRequestTokenUri.AbsoluteUri
                        ),
                        this.ConsumerCredentials.Secret,
                        code
                    )
                );

            string token
                = response.Response.Get("access_token");

            if ( string.IsNullOrEmpty(token) )
                throw new OAuthUnexpectedResponse();

            this.Token
                = new OAuthCryptoSet(
                    response.Response.Get("access_token")
                );
            this.CurrentlyHasAccessToken
                = true;
            this.Code
                = code;

            return this.Token;
        }

        public HttpWebResponse Request(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters
                = null,
            Dictionary<HttpRequestHeader, string> requestHeaders
                = null
        ) {
            // -- Validar que se tengan API-Key y Token --
            if ( this.ConsumerCredentials == null )
                throw new OAuthConsumerKeySetInvalid();
            if ( this.Token == null )
                throw new OAuthTokenNotSet();

            // -- Crear URI de Petición --
            Uri requestUri
                = new Uri(this.ClientUris.BaseUri, resource);

            if ( this.Token != null ) {
                requestUri
                    = new Uri(
                        string.Format(
                            "{0}{1}access_token={2}",
                            requestUri.AbsoluteUri,
                            requestUri.AbsoluteUri.Contains('?')
                                ? "&"
                                : "?",
                            this.Token.Key
                        )
                    );
            }

            // -- Crear cuerpo de petición --
            StringBuilder requestBodyStringBuilder
                = new StringBuilder();

            foreach ( KeyValuePair<string, string> param in bodyParameters ?? new Dictionary<string, string>() )
                requestBodyStringBuilder.Append(
                    string.Format(
                        "{0}={1}&",
                        Uri.EscapeDataString(param.Key),
                        Uri.EscapeDataString(param.Value)
                    )
                );

            string requestBodyString
                = "";

            if ( requestBodyStringBuilder.Length > 0 )
                requestBodyString
                    = requestBodyStringBuilder.ToString().Remove(
                        requestBodyStringBuilder.Length - 1
                    );

            // -- Preparar petición --
            HttpWebRequest request
                = (HttpWebRequest)WebRequest.Create(
                    requestUri
                );
            request.Method
                = requestMethod.ToString();

            // Añadir cabeceras de petición
            foreach ( KeyValuePair<HttpRequestHeader, string> item in requestHeaders ?? new Dictionary<HttpRequestHeader, string>() ) {
                switch ( item.Key ) {
                    case HttpRequestHeader.Accept:
                        request.Accept = item.Value;
                        break;

                    case HttpRequestHeader.IfModifiedSince:
                        request.IfModifiedSince = DateTime.Parse(item.Value);
                        break;

                    default:
                        request.Headers.Add(item.Key, item.Value);
                        break;
                }
            }

            if ( !string.IsNullOrEmpty(requestBodyString) ) {
                byte[] requestBodyBytes
                    = Encoding.ASCII.GetBytes(requestBodyString);

                request.ContentType
                    = "application/x-www-form-urlencoded";
                request.ContentLength
                    = requestBodyBytes.Length;

                Stream requestBodyStream
                    = request.GetRequestStream();

                requestBodyStream.Write(
                    requestBodyBytes,
                    0,
                    requestBodyBytes.Length
                );
            }

            // -- Solicitar y devolver respuesta de API --
            try {
                return request.GetResponse() as HttpWebResponse;
            } catch ( WebException ex ) {
                return ex.Response as HttpWebResponse;
            }
        }

        public OAuthResponse<string> RequestString(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters
                = null,
            Dictionary<HttpRequestHeader, string> requestHeaders
                = null
        ) {
            HttpWebResponse response
                = this.Request(
                    requestMethod,
                    resource,
                    bodyParameters,
                    requestHeaders
                );

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

            string responseString
                = responseStringBuilder.ToString();

            return new OAuthResponse<string>(
                response.StatusCode,
                response.Headers,
                responseString,
                responseString
            );
        }

        public OAuthResponse<NameValueCollection> RequestSimpleNameValue(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters
                = null,
            Dictionary<HttpRequestHeader, string> requestHeaders
                = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    bodyParameters,
                    new Dictionary<HttpRequestHeader, string>(
                        requestHeaders ?? new Dictionary<HttpRequestHeader, string>()
                    ) {
                        {HttpRequestHeader.Accept, "application/json"}
                    }
                );

            string[] responseItems
                = response.RawResponse.Split(new char[] { '&' });
            NameValueCollection nameValue
                = new NameValueCollection();
            foreach ( string item in responseItems ) {
                try {
                    string[] components
                        = item.Split(new char[] { '=' }, 2);
                    nameValue.Add(
                        Uri.UnescapeDataString(components[0]),
                        Uri.UnescapeDataString(components[1])
                    );
                } catch {
                }
            }

            return new OAuthResponse<NameValueCollection>(
                response.StatusCode,
                response.Headers,
                nameValue,
                response.RawResponse
            );
        }

        public OAuthResponse<JObject> RequestJson(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters
                = null,
            Dictionary<HttpRequestHeader, string> requestHeaders
                = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    bodyParameters,
                    new Dictionary<HttpRequestHeader, string>(
                        requestHeaders ?? new Dictionary<HttpRequestHeader, string>()
                    ) {
                        {HttpRequestHeader.Accept, "application/json"}
                    }
                );

            return new OAuthResponse<JObject>(
                response.StatusCode,
                response.Headers,
                JObject.Parse(response.Response),
                response.Response
            );
        }

        public OAuthResponse<T> RequestJson<T>(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters
                = null,
            Dictionary<HttpRequestHeader, string> requestHeaders
                = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    bodyParameters,
                    new Dictionary<HttpRequestHeader, string>(
                        requestHeaders ?? new Dictionary<HttpRequestHeader, string>()
                    ) {
                        {HttpRequestHeader.Accept, "application/json"}
                    }
                );

            T responseObject
                = JsonConvert.DeserializeObject<T>(
                    response.RawResponse
                );

            return new OAuthResponse<T>(
                response.StatusCode,
                response.Headers,
                responseObject,
                response.RawResponse
            );
        }

        public string UserID {
            get {
                if ( this._userID == null && ! ValidateSession() )
                    throw new OAuthUnexpectedResponse();

                return this._userID;
            }
        }
        private string _userID;
        
        public string UserName {
            get {
                if ( this._userName == null && !ValidateSession() )
                    throw new OAuthUnexpectedResponse();

                return this._userName;
            }
        }
        private string _userName;

        public bool ValidateSession() {
            var jsonResponse = this.RequestJson(
                HttpRequestMethod.GET,
                "/me"
            ).Response;

            this._userName = jsonResponse.SelectToken("$.name").ToString();
            this._userID   = jsonResponse.SelectToken("$.id").ToString();

            return !(
                string.IsNullOrEmpty(this._userName)
                || string.IsNullOrEmpty(this._userID)
            );
        }
    }
}
