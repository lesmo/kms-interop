using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kms.Interop.OAuth;
using Xunit;
namespace Kms.Interop.OAuth.Tests {
    public class OAuthClient_Tests {
        /// <summary>
        ///     Esta prueba valida los procesos responsables de generar las firmas
        ///     de petición de OAuth. Los parámetros base y el resultado al que se
        ///     debe llegar se tomaron de la documentación y ejemplo de Twitter:
        ///     https://dev.twitter.com/oauth/overview/creating-signatures
        /// </summary>
        [Fact(DisplayName = "OAuthClient.GetSignature")]
        public void GetSignature_Test() {
            var oAuthClient = new OAuthClient(
                new OAuthClientUris {
                    BaseUri = new Uri("https://api.twitter.com/1/")
                },
                new OAuthCryptoSet(
                    "xvz1evFS4wEEPTGEFPHBog",
                    "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw"
                ),
                new OAuthCryptoSet(
                    "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb",
                    "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE"
                )
            );

            var oAuthRequestUri = new Uri(
                oAuthClient.ClientUris.BaseUri,
                new Uri(
                    "statuses/update.json",
                    UriKind.Relative
                )
            );
            var oAuthSignature = oAuthClient.GetSignature(
                HttpRequestMethod.POST,
                oAuthRequestUri,
                new NameValueCollection {
                    {"status", "Hello Ladies + Gentlemen, a signed OAuth request!"},
                    {"include_entities", "true"},
                    {"oauth_consumer_key", oAuthClient.ConsumerCredentials.Key},
                    {"oauth_nonce", "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg"},
                    {"oauth_signature_method", oAuthClient.SignatureMethod},
                    {"oauth_timestamp", "1318622958"},
                    {"oauth_token", oAuthClient.Token.Key},
                    {"oauth_version", oAuthClient.Version}
                }
            );

            Assert.Equal(
                "tnnArxj06cWHq44gCs1OSKk/jLY=".ToCharArray(),
                oAuthSignature.ToCharArray()
            );
        }
    }
}
