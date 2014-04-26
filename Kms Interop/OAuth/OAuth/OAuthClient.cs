using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Specialized;
using Kms.Interop.OAuth;
using Kms.Interop.OAuth.Utils;

namespace Kms.Interop.OAuth {
    /// <summary>
    ///     Permite acceder a los recursos de un API que utiliza el protocolo OAuth, específicamente la Nube de KMS.
    ///     Los procesos normales de Autorización OAuth son ligeramente diferentes, pues las Consumer Keys (API-Keys
    ///     de KMS) permiten a las aplicaciones oficiales de KMS inciar sesión a través de un proceso de Inicio de
    ///     Sesión HTTP Básico (Basic HTTP Authorization), de forma que las aplicaciones oficiales pueden saltarse
    ///     el mostrar un formulario Web de Inicio de Sesión y Autorización.
    /// </summary>
    public class OAuthClient : IOAuthClient {
        #region Properties
        public virtual String ProviderName {
            get;
            set;
        }

        /// <summary>
        ///     Información de la ubicación de diversos recursos HTTP necesarios para el flujo
        ///     básico e inicial del protocolo OAuth.
        /// </summary>
        public OAuthClientUris ClientUris {
            get;
            set;
        }

        /// <summary>
        ///     Consumer Key y Secret.
        /// </summary>
        public OAuthCryptoSet ConsumerCredentials {
            get;
            set;
        }
        /// <summary>
        ///     Token y Token Secret. Esta propiedad almacena el Request Token y Access Token.
        /// </summary>
        public OAuthCryptoSet Token {
            get {
                return this._token;
            }
            set {
                if ( value == null )
                    this.CurrentlyHasAccessToken = false;
                
                this._token = value;
            }
        }
        private OAuthCryptoSet _token;
        /// <summary>
        ///     Devuelve si el Token y Token Secret actualmente en ésta instancia corresponden a un
        ///     conjunto de Access Token, que pueden acceder a todos los recursos del API.
        /// </summary>
        public bool CurrentlyHasAccessToken {
            get;
            protected set;
        }
        /// <summary>
        ///     Método de Firmado de la Petición. Por ahora, la librería sólo soporta HMAC-SHA1.
        /// </summary>
        public string SignatureMethod {
            get {
                return "HMAC-SHA1";
            }
        }
        /// <summary>
        ///     Versión de OAuth que utiliza el API. Por ahora, la librería sólo soporta OAuth 1.0a, y según
        ///     el protocolo debe reportarse que se espera que la petición se procese por OAuth 1.0.
        /// </summary>
        public string Version {
            get {
                return "1.0";
            }
        }
        #endregion
        
        /// <summary>
        ///     Crear un nuevo cliente de OAuth vacío.
        /// </summary>
        public OAuthClient() {
            this.CurrentlyHasAccessToken
                = false;
        }

        /// <summary>
        ///     Crear un nuevo cliente de OAuth.
        /// </summary>
        /// <param name="oAuthClientUris">
        ///     Contenedor de las URIs utilizadas para obtener Request Tokens, Access Token y Autorización del Usuario.
        /// </param>
        /// <param name="oAuthConsumerCredentials">
        ///     Conjunto de Consumer Key (API-Key de KMS) y Consumer Secret (API-Secret de KMS).
        /// </param>
        /// <param name="oAuthToken">
        ///     Conjunto de Token y Token Secret.
        /// </param>
        public OAuthClient(
            OAuthClientUris oAuthClientUris,
            OAuthCryptoSet oAuthConsumerCredentials = null,
            OAuthCryptoSet oAuthToken = null
        ) {
            this.ClientUris
                = oAuthClientUris;
            this.ConsumerCredentials
                = oAuthConsumerCredentials;
            this.Token
                = oAuthToken;
            this.CurrentlyHasAccessToken
                = oAuthToken != null;
        }

        /// <summary>
        ///     Devuelve la Signature Base, o Base de Firma, necesaria para generar la Firma de Petición OAuth.
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de la Petición.
        /// </param>
        /// <param name="requestUri">
        ///     URI a la que se realiza la petición OAuth. Debe ser posible obtener el AbsoluteUri de éste.
        /// </param>
        /// <param name="parameters">
        ///     Diccionario que contiene los parámetros ordenados, incluyendo todos los necesarios por OAuth,
        ///     para generar la Firma de Petición.
        /// </param>
        /// <returns>
        ///     Devuelve los bytes que representan la Firma de Petición (listo para obtener la represetnación en
        ///     Base 64).
        /// </returns>
        public byte[] GetSignatureBase(
            HttpRequestMethod requestMethod,
            Uri requestUri,
            NameValueCollection parameters
        ) {
            StringBuilder parameterStringBuilder
                = new StringBuilder();

            foreach ( string key in parameters.SortByName() ) {
                foreach ( string value in parameters.GetValues(key) )
                    parameterStringBuilder.Append(
                        string.Format(
                            "{0}={1}&",
                            Uri.EscapeDataString(key),
                            Uri.EscapeDataString(value).Replace("!","%21")
                        )
                    );
            }

            string parameterString
                = parameterStringBuilder
                    .ToString()
                    .Remove(parameterStringBuilder.Length - 1);

            string signatureBaseString
                =  string.Format(
                    "{0}&{1}&{2}",
                    requestMethod.ToString().ToUpper(),
                    Uri.EscapeDataString(requestUri.AbsoluteUri),
                    Uri.EscapeDataString(parameterString)
                );
            
            return System.Text.Encoding.UTF8.GetBytes(
                signatureBaseString
            );
        }

        /// <summary>
        ///     Devuelve la Firma de Petición OAuth de la solicitud configurada.
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de la Petición.
        /// </param>
        /// <param name="requestUri">
        ///     URI del recurso al que se hará la Petición.
        /// </param>
        /// <param name="oAuthSortedParameters">
        ///     Parámetros a enviarle al recurso.
        /// </param>
        /// <returns>
        ///     Devuelve la Firma de Petición calculada, URL-Encoded.
        /// </returns>
        public string GetSignature(
            HttpRequestMethod requestMethod,
            Uri requestUri,
            NameValueCollection parameters
        ) {
            if ( this.ConsumerCredentials == null )
                throw new OAuthConsumerKeySetInvalid();

            byte[] signatureBase
                = this.GetSignatureBase(
                    requestMethod,
                    requestUri,
                    parameters
                );
            string signatureKey
                = string.Format(
                    "{0}&{1}",
                    this.ConsumerCredentials.Secret,
                    this.Token == null
                        ? ""
                        : this.Token.Secret
                );
            
            HMACSHA1 hmacsha1
                = new HMACSHA1(
                    Encoding.UTF8.GetBytes(signatureKey)
                );

            string signatureString
                = Convert.ToBase64String(
                    hmacsha1.ComputeHash(signatureBase)    
                );

            return signatureString;
        }

        /// <summary>
        ///     Genera una petición de generar un nuevo Request Token de OAuth, y devuelve la URL utilizada para
        ///     continuar con el proceso de autorización de acceso OAuth por parte del Usuario.
        /// </summary>
        /// <returns>
        ///     Devuelve un conjunto de Request Token y Token Secret
        /// </returns>
        public OAuthCryptoSet GetRequestToken(Dictionary<string, string> extraParameters = null) {
            NameValueCollection oAuthExtraParameters
                = new NameValueCollection();
            this.Token
                = null;
            
            if ( this.ClientUris == null )
                throw new OAuthUnexpectedRequest();

            if ( this.ClientUris.CallbackRequestTokenUri == null)
                oAuthExtraParameters.Add(
                    "oauth_callback",
                    "oob"
                );
            else
                oAuthExtraParameters.Add(
                    "oauth_callback",
                    this.ClientUris.CallbackRequestTokenUri.AbsoluteUri
                );

            OAuthResponse<NameValueCollection> response
                = this.RequestSimpleNameValue(
                    HttpRequestMethod.POST,
                    this.ClientUris.RequestTokenResource,
                    oAuthExtraParameters,
                    oAuthExtraParameters.ToDictionary(false)
                );

            try {
                this.Token
                    = new OAuthCryptoSet(
                        response.Response.Get("oauth_token"),
                        response.Response.Get("oauth_token_secret")
                    );
            } catch ( IndexOutOfRangeException ) {
                throw new OAuthUnexpectedResponse();
            }

            return this.Token;
        }

        /// <summary>
        ///     Obtiene la URI a través de la cual el Usuario autoriza a la Aplicación el acceso a su información
        /// </summary>
        /// <returns>
        ///     Devuelve la URI a través de la cual el Usuario autoriza a la Aplicación el acceso a su información.
        /// </returns>
        public Uri GetAuthorizationUri() {
            if ( this.CurrentlyHasAccessToken )
                throw new OAuthUnexpectedRequest();

            OAuthCryptoSet requestToken
                = this.GetRequestToken();

            Uri baseAuthorizationUri
                = new Uri(
                    this.ClientUris.BaseUri,
                    this.ClientUris.AuthorizationResource
                );

            return new Uri(
                string.Format(
                    "{0}{1}oauth_token={2}",
                    baseAuthorizationUri.AbsoluteUri,
                    baseAuthorizationUri.AbsoluteUri.Contains('?')
                        ? "&"
                        : "?",
                    requestToken.Key
                )
            );
        }

        /// <summary>
        ///     Realiza el cambio o "canje" de un Request Token de OAuth por un Access Token, con permiso
        ///     totales de acceso a la información del Usuario (o al menos aquellas autorizadas por el Usuario).
        /// </summary>
        /// <param name="requestToken">
        ///     Request Token de OAuth
        /// </param>
        /// <param name="verifier">
        ///     Verifier Code de OAuth
        /// </param>
        /// <returns>
        ///     Conjunto de Access Token y Token Secret para peticiones subsecuentes. El 
        /// </returns>
        public virtual OAuthCryptoSet ExchangeRequestToken(string verifier) {
            if ( this.Token == null || this.CurrentlyHasAccessToken )
                throw new OAuthUnexpectedRequest();

            if ( string.IsNullOrEmpty(verifier) )
                throw new ArgumentNullException("verifier");

            OAuthResponse<NameValueCollection>
                response = this.RequestSimpleNameValue(
                    HttpRequestMethod.POST,
                    this.ClientUris.ExchangeTokenResource,
                    new NameValueCollection() {
                        {"oauth_verifier", verifier}
                    }
                );

            string token
                = response.Response.Get("oauth_token");
            string tokenSecret
                = response.Response.Get("oauth_token_secret");

            if ( string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret) )
                throw new OAuthUnexpectedResponse();

            this.Token
                = new OAuthCryptoSet(
                    response.Response.Get("oauth_token"),
                    response.Response.Get("oauth_token_secret")
                );
            this.CurrentlyHasAccessToken
                = true;

            return this.Token;
        }

        /// <summary>
        ///     Realiza una nueva petición OAuth al recurso especificado, y devuleve el objeto HttpWebResponse
        ///     generado por la petición.
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de Petición HTTP.
        /// </param>
        /// <param name="resource">
        ///     Recurso HTTP al que llamar (parte del URI después del dominio).
        /// </param>
        /// <param name="parameters">
        ///     Parámetros a enviar en la petición
        /// </param>
        /// <param name="oAuthExtraParameters">
        ///     Parámetros extra a añadir en la cabecera Authorization de OAuth.
        /// </param>
        /// <param name="requestHeaders">
        ///     Cabeceras HTTP a añadir a la petición.
        /// </param>
        /// <returns>
        ///     Devuelve la respuesta recibida del API.
        /// </returns>
        public HttpWebResponse Request(
            HttpRequestMethod requestMethod,
            string resource,
            NameValueCollection parameters = null,
            Dictionary<string, string> oAuthExtraParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null
        ) {
            // -- Validar que se tengan API-Key y Token --
            if ( this.ConsumerCredentials == null )
                throw new OAuthConsumerKeySetInvalid();

            // -- Crear URI de Petición --
            Uri requestUri
                = new Uri(this.ClientUris.BaseUri, resource);

            // -- Generar valores de cabecera OAuth --
            // Calcular Timestamp UNIX (segundos desde 1970-01-01 00:00:00)
            TimeSpan timestamp
                = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            // Calcular Nonce
            byte[] nonceBytes
                = new byte[16];

            (new Random()).NextBytes(nonceBytes);

            string nonce
                = Convert.ToBase64String(nonceBytes);

            // Generar valores de cabecera Authorization: OAuth
            NameValueCollection oAuthParameters
                = new NameValueCollection();

            if ( oAuthExtraParameters != null )
                oAuthParameters.AddFromDictionary(oAuthExtraParameters);

            oAuthParameters.Add(
                new NameValueCollection {
                    {"oauth_consumer_key", this.ConsumerCredentials.Key},
                    {"oauth_nonce", nonce},
                    {"oauth_signature_method", this.SignatureMethod},
                    {"oauth_timestamp", ((int)timestamp.TotalSeconds).ToString()},
                    {"oauth_version", this.Version}
                }
            );

            if ( this.Token != null && this.Token.Key != null )
                oAuthParameters.Add("oauth_token", this.Token.Key);

            // -- Generar base para Firma de Petición --
            var oAuthSignatureBaseParameters = new NameValueCollection();

            oAuthSignatureBaseParameters.Add(oAuthParameters);

            if ( parameters != null )
                oAuthSignatureBaseParameters.Add(parameters);

            // -- Añadir Firma de Petición a cabecera Authorization: OAuth
            oAuthParameters.Add(
                "oauth_signature",
                this.GetSignature(
                    requestMethod,
                    requestUri,
                    oAuthSignatureBaseParameters
                )
            );

            // -- Generar HttpWebRequest --
            // Generar cuerpo de petición
            StringBuilder requestStringBuilder
                = new StringBuilder();
            
            if ( parameters == null )
                parameters = new NameValueCollection();

            foreach ( string key in parameters.AllKeys ) {
                foreach ( string value in parameters.GetValues(key) )
                    requestStringBuilder.Append(
                        string.Format(
                            "{0}={1}&",
                            Uri.EscapeDataString(key),
                            Uri.EscapeDataString(value)
                        )
                    );
            }

            string requestString
                = "";
            if ( requestStringBuilder.Length > 0 )
                requestString
                    = requestStringBuilder.ToString().Remove(
                        requestStringBuilder.Length - 1
                    );

            // Preparar objeto de Petición Web
            if ( requestMethod == HttpRequestMethod.GET && requestString.Length > 0 ) {
                requestUri = new Uri(
                    string.Format(
                        "{0}{1}{2}",
                        requestUri.AbsoluteUri,
                        requestUri.AbsoluteUri.Contains('?')
                            ? "&"
                            : "?",
                        requestString
                    )
                );
            }

            HttpWebRequest request
                = (HttpWebRequest)WebRequest.Create(
                    requestUri
                );
            request.Method
                = requestMethod.ToString();

            foreach ( KeyValuePair<HttpRequestHeader, string> item in requestHeaders ?? new Dictionary<HttpRequestHeader, string>() ) {
                switch ( item.Key ) {
                    case HttpRequestHeader.Accept :
                        request.Accept = item.Value;
                        break;

                    case HttpRequestHeader.IfModifiedSince :
                        request.IfModifiedSince = DateTime.Parse(item.Value);
                        break;

                    default :
                        request.Headers.Add(item.Key, item.Value);
                        break;
                }
            }

            // Añadir cabecera Authorization: OAuth
            StringBuilder oAuthHeaderString
                = new StringBuilder("OAuth ");

            foreach ( string key in oAuthParameters.AllKeys ) {
                foreach ( string value in oAuthParameters.GetValues(key) )
                    oAuthHeaderString.AppendFormat(
                        "{0}=\"{1}\", ",
                        key,
                        Uri.EscapeDataString(value) 
                    );
            }

            request.Headers.Add(
                HttpRequestHeader.Authorization,
                oAuthHeaderString.ToString().Remove(oAuthHeaderString.Length - 2)
            );

            if ( requestMethod != HttpRequestMethod.GET ) {
                byte[] requestBodyBytes
                    = Encoding.UTF8.GetBytes(requestString);

                request.ContentType
                    = "application/x-www-form-urlencoded; charset=utf-8";
                request.ContentLength
                    = requestBodyBytes.Length;
                
                Stream requestBodyStream
                    = request.GetRequestStream();

                requestBodyStream.Write(
                    requestBodyBytes,
                    0,
                    requestBodyBytes.Length
                );
                requestBodyStream.Flush();
            }

            // -- Solicitar y devolver respuesta de API --
            try {
                return request.GetResponse() as HttpWebResponse;
            } catch ( WebException ex ) {
                HttpWebResponse response
                    = ex.Response as HttpWebResponse;

                if ( response == null ) {
                    throw new OAuthUnexpectedResponse();
                } else if ( response.StatusCode == HttpStatusCode.Unauthorized ) {
                    this.Token = null;
                    this.CurrentlyHasAccessToken = false;
                    throw new OAuthUnauthorized();
                } else {
                    return response;
                }
            }
        }
        
        /// <summary>
        ///     Realiza una nueva petición OAuth al recurso especificado, devolviendo la respuesta tal como viene
        ///     en una cadena.
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de Petición HTTP.
        /// </param>
        /// <param name="resource">
        ///     Recurso HTTP al que llamar (parte del URI después del dominio).
        /// </param>
        /// <param name="parameters">
        ///     Parámetros a enviar en la petición
        /// </param>
        /// <param name="oAuthExtraParameters">
        ///     Parámetros extra a añadir en la cabecera Authorization de OAuth.
        /// </param>
        /// <param name="requestHeaders">
        ///     Cabeceras HTTP a añadir a la petición.
        /// </param>
        /// <returns>
        ///     Devuelve la respuesta recibida del API.
        /// </returns>
        public OAuthResponse<string> RequestString(
            HttpRequestMethod requestMethod,
            string resource,
            NameValueCollection parameters = null,
            Dictionary<string, string> oAuthExtraParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null
        ) {
            HttpWebResponse response
                = this.Request(
                    requestMethod,
                    resource,
                    parameters,
                    oAuthExtraParameters,
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

        /// <summary>
        ///     Realiza una nueva petición OAuth al recurso especificado, deserializando la respuesta de tipo
        ///     HTTP URL Encoded (como formulario) a una NameValueCollection
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de Petición HTTP.
        /// </param>
        /// <param name="resource">
        ///     Recurso HTTP al que llamar (parte del URI después del dominio).
        /// </param>
        /// <param name="parameters">
        ///     Parámetros a enviar en la petición
        /// </param>
        /// <param name="oAuthExtraParameters">
        ///     Parámetros extra a añadir en la cabecera Authorization de OAuth.
        /// </param>
        /// <param name="requestHeaders">
        ///     Cabeceras HTTP a añadir a la petición.
        /// </param>
        /// <returns>
        ///     Devuelve la respuesta recibida del API.
        /// </returns>
        public OAuthResponse<NameValueCollection> RequestSimpleNameValue(
            HttpRequestMethod requestMethod,
            string resource,
            NameValueCollection parameters = null,
            Dictionary<string, string> oAuthExtraParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    parameters,
                    oAuthExtraParameters,
                    requestHeaders
                );
            
            string[] responseItems
                = response.RawResponse.Split(new char[]{'&'});
            NameValueCollection nameValue
                = new NameValueCollection();
            foreach ( string item in responseItems ) {
                try {
                    string[] components
                        = item.Split(new char[]{'='}, 2);
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

        /// <summary>
        ///     Realizar una nueva petición al recurso OAuth especificado, deserializando la respuesta en JSON
        ///     hacia el tipo de objeto especificado.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo al que se realizará la deserialización de la respuesta JSON.
        /// </typeparam>
        /// <param name="requestMethod">
        ///     Método de Petición HTTP.
        /// </param>
        /// <param name="resource">
        ///     Recurso HTTP al que llamar (parte del URI después del dominio).
        /// </param>
        /// <param name="parameters">
        ///     Parámetros a enviar en la petición
        /// </param>
        /// <param name="oAuthExtraParameters">
        ///     Parámetros extra a añadir en la cabecera Authorization de OAuth.
        /// </param>
        /// <param name="requestHeaders">
        ///     Cabeceras HTTP a añadir a la petición.
        /// </param>
        /// <returns>
        ///     Devuelve la respuesta recibida del API.
        /// </returns>
        public OAuthResponse<T> RequestJson<T>(
            HttpRequestMethod requestMethod,
            string resource,
            NameValueCollection parameters = null,
            Dictionary<string, string> oAuthExtraParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    parameters,
                    oAuthExtraParameters,
                    new Dictionary<HttpRequestHeader, string>(requestHeaders) {
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

        /// <summary>
        ///     Realizar una nueva petición al recurso OAuth especificado, deserializando la respuesta en JSON.
        /// </summary>
        /// <param name="requestMethod">
        ///     Método de Petición HTTP.
        /// </param>
        /// <param name="resource">
        ///     Recurso HTTP al que llamar (parte del URI después del dominio).
        /// </param>
        /// <param name="parameters">
        ///     Parámetros a enviar en la petición
        /// </param>
        /// <param name="oAuthExtraParameters">
        ///     Parámetros extra a añadir en la cabecera Authorization de OAuth.
        /// </param>
        /// <param name="requestHeaders">
        ///     Cabeceras HTTP a añadir a la petición.
        /// </param>
        /// <returns>
        ///     Devuelve la respuesta recibida del API.
        /// </returns>
        public OAuthResponse<JObject> RequestJson(
            HttpRequestMethod requestMethod,
            string resource,
            NameValueCollection parameters = null,
            Dictionary<string, string> oAuthExtraParameters = null,
            Dictionary<HttpRequestHeader, string> requestHeaders = null
        ) {
            OAuthResponse<string> response
                = this.RequestString(
                    requestMethod,
                    resource,
                    parameters,
                    oAuthExtraParameters,
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
    }
}