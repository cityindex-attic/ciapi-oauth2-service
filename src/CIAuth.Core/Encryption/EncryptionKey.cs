namespace CIAuth.Core.Encryption
{
    public class EncryptionKey
    {
        public EncryptionKey(string publicKey , string privateKey )
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}