using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.OAuth {
    /// <summary>
    ///     Representa el conjunto de Token y Secret, o Consumer Key (API-Key de KMS)
    ///     y Secret (API-Secret de KMS) de OAuth
    /// </summary>
    public sealed class OAuthCryptoSet {
        /// <summary>
        ///     Token o Consumer Key de OAuth (API-Key de KMS)
        /// </summary>
        public readonly string Key;
        /// <summary>
        ///     Token Secret o Consumer Secret de OAuth (API-Secret de KMS)
        /// </summary>
        public readonly string Secret;

        /// <summary>
        ///     Crea un nuevo conjunto de Token y Secret, o Consumer Key (API-Key de KMS)
        ///     y Secret (API-Secret de KMS) de OAuth
        /// </summary>
        /// <param name="key">
        ///     Token o Consumer Key (API Key de KMS)
        /// </param>
        /// <param name="secret">
        ///     Token Secret o Consumer Secret (API Secret de KMS)
        /// </param>
        public OAuthCryptoSet(string key, string secret = null) {
            this.Key
                = key;
            this.Secret
                = secret;
        }
    }
}
