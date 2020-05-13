namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ICryptoService
    {
        string GenerateEncryptedSecretKey();

        void EncryptAsServer<T>(T obj);
        void DecryptAsServer<T>(T obj);

        void EncryptAsUser<T>(T obj, string encryptedSecretKey);
        void DecryptAsUser<T>(T obj, string encryptedSecretKey);
    }
}
