using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CoviIDApiCore.V1.Interfaces.Services;
using CoviIDApiCore.V1.Attributes;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    enum TransformDirection { ToCipher, FromCipher };

    public class CryptoService : ICryptoService
    {
        private readonly string serverKey;
        private readonly RNGCryptoServiceProvider rng;

        public CryptoService(IConfiguration configuration)
        {
            serverKey = configuration.GetValue<string>("ServerKey");
            rng = new RNGCryptoServiceProvider();
        }

        public string GenerateEncryptedSecretKey()
        {
            byte[] key = new byte[32]; // 256 bits
            rng.GetBytes(key);
            return Encrypt(Convert.ToBase64String(key), serverKey);
        }

        public void EncryptAsServer<T>(T obj, bool isStatic = false)
        {
            TransformObj(TransformDirection.ToCipher, obj, serverKey, true, isStatic);
        }

        public void DecryptAsServer<T>(T obj, bool isStatic = false)
        {
            TransformObj(TransformDirection.FromCipher, obj, serverKey, true, isStatic);
        }

        public void EncryptAsUser<T>(T obj, string encryptedSecretKey)
        {
            var secretKey = Decrypt(encryptedSecretKey, serverKey);
            TransformObj(TransformDirection.ToCipher, obj, secretKey, false);
        }

        public void DecryptAsUser<T>(T obj, string encryptedSecretKey)
        {
            var secretKey = Decrypt(encryptedSecretKey, serverKey);
            TransformObj(TransformDirection.FromCipher, obj, secretKey, false);
        }

        private void TransformObj<T>(TransformDirection direction, T obj, string key, bool serverManaged = false, bool isStatic = false)
        {
            foreach (PropertyInfo prop in GetEncryptedProperties(obj, serverManaged))
            {
                // Weird things would happen here if prop.PropertyType is not a
                // string. We should probably enforce that.
                var before = prop.GetValue(obj) as string;
                string after;
                if (direction == TransformDirection.ToCipher) {
                    after = Encrypt(before, key, isStatic);
                } else {
                    after = Decrypt(before, key, isStatic);
                }
                prop.SetValue(obj, after);
            }
        }

        private List<PropertyInfo> GetEncryptedProperties<T>(T obj, bool serverManaged = false)
        {
            List<PropertyInfo> encryptedProps = new List<PropertyInfo>();

            PropertyInfo[] allProps = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in allProps)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    Encrypted encrypted = attr as Encrypted;
                    if (encrypted != null && encrypted.serverManaged == serverManaged)
                    {
                        //Don't encrypt null values
                        var value = prop.GetValue(obj);
                        if (value == null)
                            continue;

                        encryptedProps.Add(prop);
                    }
                }
            }

            return encryptedProps;
        }

        private string Encrypt (string plainText, string key, bool isStatic = false) {
            using (var aes = Aes.Create())
            {
                var iv = new byte[16];

                if (isStatic)
                    Array.Copy(Encoding.ASCII.GetBytes(key), 0, iv, 0, iv.Length);
                else
                    iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(Convert.FromBase64String(key), iv))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cs))
                    {
                        // prepend IV to data
                        ms.Write(iv);
                        binaryWriter.Write(Encoding.UTF8.GetBytes(plainText));
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string Decrypt (string cipherText, string key, bool isStatic = false)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            var iv = new byte[16];

            Array.Copy(isStatic ? Encoding.ASCII.GetBytes(key) : cipherBytes, 0, iv, 0, iv.Length);

            using (var aes = Aes.Create())
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(Convert.FromBase64String(key), iv), CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cs))
                    {
                        binaryWriter.Write(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
