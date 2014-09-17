using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Kms.Interop.OAuth.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kms.Interop.OAuth.SocialClients {

    public enum OAuth2ResponseType {
        Code,
        Token,
        CodeAndToken
    }

    public class FacebookClient /*: OAuth2.OAuth2Client */ : IOAuthSocialClient {

        private OAuth2ResponseType AuthorizationResponseType = OAuth2ResponseType.Token;

        private OAuthCryptoSet _token;
        private string _userID;
        private string _userName;

        public FacebookClient(OAuthCryptoSet consumer, OAuthCryptoSet token = null, Uri callbackRequestTokenUri = null) {
            ClientUris = new OAuthClientUris {
                BaseUri = new Uri("https://graph.facebook.com/"),
                AuthorizationResource = "https://www.facebook.com/dialog/oauth",
                RequestTokenResource = "https://www.facebook.com/dialog/oauth",
                ExchangeTokenResource = "oauth/access_token",
                CallbackRequestTokenUri =
                    callbackRequestTokenUri ?? new Uri("https://www.facebook.com/connect/login_success.html"),
            };

            ConsumerCredentials = consumer;
            Token = token;

            if ( Token != null )
                CurrentlyHasAccessToken = true;
        }

        public OAuthClientUris ClientUris { get; set; }

        /// <summary>
        ///     Token obtenido por el cliente en App Móvil o Escritorio.
        /// </summary>
        public string AppToken {
            get {
                return Token == null ? null : Token.Key;
            }
        }

        /// <summary>
        ///     Token obtenido por el cliente en Servidor.
        /// </summary>
        public string ServerToken {
            get {
                return Token == null ? null : Token.Secret;
            }
        }

        public string Code { get; set; }

        public bool CurrentlyHasAccessToken { get; private set; }

        public String ProviderName {
            get { return "Facebook"; }
        }

        public OAuthCryptoSet ConsumerCredentials { get; set; }

        public OAuthCryptoSet Token {
            get { return _token; }
            set {
                _token = value;
                CurrentlyHasAccessToken = _token != null;
            }
        }

        public string UserID {
            get {
                if ( _userID == null && ! ValidateSession() )
                    throw new OAuthUnexpectedResponse();

                return _userID;
            }
        }

        public string UserName {
            get {
                if ( _userName == null && !ValidateSession() )
                    throw new OAuthUnexpectedResponse();

                return _userName;
            }
        }

        public bool ValidateSession() {
            var jsonResponse = RequestJson(HttpRequestMethod.GET, "/me").Response;

            _userName = jsonResponse.SelectToken("$.name").ToString();
            _userID   = jsonResponse.SelectToken("$.id").ToString();

            return !(string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_userID));
        }

        public Uri GetAuthorizationUri(
            OAuth2ResponseType responseType = OAuth2ResponseType.Token,
            FacebookPermission[] requestPermissions = null) {
            AuthorizationResponseType = responseType;

            var responseTypeString = "fuck";

            switch ( responseType ) {
                case OAuth2ResponseType.Token:
                    responseTypeString = "token";
                    break;
                case OAuth2ResponseType.Code:
                    responseTypeString = "token";
                    break;
                case OAuth2ResponseType.CodeAndToken:
                    responseTypeString = "code%20token";
                    break;
            }

            if ( requestPermissions == null ) {
                requestPermissions = new[] {
                    FacebookPermission.BasicInfo
                };
            } else if ( !requestPermissions.Contains(FacebookPermission.BasicInfo) ) {
                var requestPermissionsTemp = new FacebookPermission[requestPermissions.Length + 1];
                requestPermissionsTemp[0] = FacebookPermission.BasicInfo;

                requestPermissions.CopyTo(requestPermissionsTemp, 1);
                requestPermissions = requestPermissionsTemp;
            }

            var scopeStringBuilder = new StringBuilder();
            foreach ( var permission in requestPermissions ) {
                var permissionString = permission.CamelCaseToUnderlineString();

                if ( permissionString.StartsWith("user_actions") ) {
                    permissionString = permissionString.Remove(12, 1);
                    permissionString = permissionString.Insert(12, ".");
                } else if ( permissionString.StartsWith("friends_actions") ) {
                    permissionString = permissionString.Remove(15, 1);
                    permissionString = permissionString.Insert(15, ".");
                }

                scopeStringBuilder.Append(permissionString);
                scopeStringBuilder.Append(',');
            }

            return
                new Uri(
                    string.Format(
                        "{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}&display=popup",
                        ClientUris.AuthorizationResource,
                        ConsumerCredentials.Key,
                        Uri.EscapeDataString(
                            ClientUris.CallbackRequestTokenUri == null
                                ? "https://www.facebook.com/connect/login_success.html"
                                : ClientUris.CallbackRequestTokenUri.AbsoluteUri),
                        responseTypeString,
                        scopeStringBuilder.ToString().Remove(scopeStringBuilder.Length - 1)));
        }

        public OAuthCryptoSet ExchangeCodeForToken(string code) {
            if ( string.IsNullOrEmpty(ConsumerCredentials.Secret) )
                throw new InvalidOperationException(
                    "Current Facebook Client configuration does not include Client Secret.");

            if ( Token != null )
                throw new InvalidOperationException(
                    "Current Facebook Client configuration already contains a Token exchanged obtained via Code.");

            var response = RequestSimpleNameValue(
                HttpRequestMethod.GET,
                string.Format(
                    "oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                    ConsumerCredentials.Key,
                    Uri.EscapeDataString(ClientUris.CallbackRequestTokenUri.AbsoluteUri),
                    ConsumerCredentials.Secret,
                    code));

            var token = response.Response.Get("access_token");

            if ( string.IsNullOrEmpty(token) )
                throw new OAuthUnexpectedResponse();

            Token = new OAuthCryptoSet(response.Response.Get("access_token"));
            Code  = code;
            CurrentlyHasAccessToken = true;

            return Token;
        }

        public HttpWebResponse Request(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null) {

            // -- Validar que se tengan API-Key y Token --
            if ( ConsumerCredentials == null )
                throw new OAuthConsumerKeySetInvalid();

            // -- Crear URI de Petición --
            var requestUri = new Uri(ClientUris.BaseUri, resource);

            if ( Token != null ) {
                requestUri =
                    new Uri(
                        string.Format(
                            "{0}{1}access_token={2}",
                            requestUri.AbsoluteUri,
                            requestUri.AbsoluteUri.IndexOf('?') > -1 ? "&" : "?",
                            Token.Key));
            }

            // -- Crear cuerpo de petición --
            var requestBodyStringBuilder = new StringBuilder();

            foreach ( var param in bodyParameters ?? new Dictionary<string, string>() ) {
                requestBodyStringBuilder.Append(
                    string.Format("{0}={1}&", Uri.EscapeDataString(param.Key), Uri.EscapeDataString(param.Value)));
            }

            var requestBodyString = "";
            if ( requestBodyStringBuilder.Length > 0 )
                requestBodyString = requestBodyStringBuilder.ToString().Remove(requestBodyStringBuilder.Length - 1);

            // -- Preparar petición --
            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = requestMethod.ToString();

            // Añadir cabeceras de petición
            foreach ( var item in requestHeaders ?? new Dictionary<HttpRequestHeader, string>() ) {
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
                var requestBodyBytes = Encoding.ASCII.GetBytes(requestBodyString);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = requestBodyBytes.Length;

                var requestBodyStream = request.GetRequestStream();
                requestBodyStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
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
            Dictionary<string, string> bodyParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null) {

            var response = Request(requestMethod, resource, bodyParameters, requestHeaders);
            var responseStringBuilder = new StringBuilder();
            var responseStream        = response.GetResponseStream();

            var streamBuffer = new byte[8192];
            var count = 0;
            do {
                if ( responseStream != null )
                    count = responseStream.Read(streamBuffer, 0, streamBuffer.Length);

                if ( count > 0 )
                    responseStringBuilder.Append(Encoding.ASCII.GetString(streamBuffer, 0, count));
            } while ( count > 0 );

            var responseString = responseStringBuilder.ToString();

            return new OAuthResponse<string>(response.StatusCode, response.Headers, responseString, responseString);
        }

        public OAuthResponse<NameValueCollection> RequestSimpleNameValue(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null) {

            var response = RequestString(
                requestMethod,
                resource,
                bodyParameters,
                new Dictionary<HttpRequestHeader, string>(requestHeaders ?? new Dictionary<HttpRequestHeader, string>()) {
                    {
                        HttpRequestHeader.Accept, "application/json"
                    }
                });

            var nameValue     = new NameValueCollection();
            var responseItems = response.RawResponse.Split('&');
            foreach ( var item in responseItems ) {
                try {
                    var components = item.Split("=".ToCharArray(), 2);
                    nameValue.Add(Uri.UnescapeDataString(components[0]), Uri.UnescapeDataString(components[1]));
                } catch {}
            }

            return new OAuthResponse<NameValueCollection>(
                response.StatusCode,
                response.Headers,
                nameValue,
                response.RawResponse);
        }

        public OAuthResponse<JObject> RequestJson(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null) {

            if ( requestHeaders == null )
                requestHeaders = new Dictionary<HttpRequestHeader, string>();

            var response = RequestString(
                requestMethod,
                resource,
                bodyParameters,
                new Dictionary<HttpRequestHeader, string>(requestHeaders) {
                    {
                        HttpRequestHeader.Accept, "application/json"
                    }
                });

            return new OAuthResponse<JObject>(
                response.StatusCode,
                response.Headers,
                JObject.Parse(response.Response),
                response.Response);
        }

        public OAuthResponse<T> RequestJson<T>(
            HttpRequestMethod requestMethod,
            string resource,
            Dictionary<string, string> bodyParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null) {

            var response = RequestString(
                requestMethod,
                resource,
                bodyParameters,
                new Dictionary<HttpRequestHeader, string>(requestHeaders ?? new Dictionary<HttpRequestHeader, string>()) {
                    {
                        HttpRequestHeader.Accept, "application/json"
                    }
                });

            var responseObject = JsonConvert.DeserializeObject<T>(response.RawResponse);

            return new OAuthResponse<T>(
                response.StatusCode,
                response.Headers,
                responseObject,
                response.RawResponse);
        }

    }

}