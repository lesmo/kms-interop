namespace Kms.Interop.OAuth {

    public interface IOAuthClient {
        string ProviderName { get; }

        OAuthCryptoSet ConsumerCredentials { get; set; }
        OAuthCryptoSet Token { get; set; }
    }

}