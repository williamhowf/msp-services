using Com.GGIT.Common.Util;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Com.GGIT.Common.Security
{
    public class Cryptography
    {
        #region Variables
        private readonly byte[] DefaultSecretKey = Encoding.UTF8.GetBytes(AppSettingsJson.GetAppSettingsValue("SecretSettings", "SecretKey"));

        private const string DECRYPT = "decrypt";
        private const string ENCRYPT = "encrypt";
        private const string SHA1 = "SHA1";
        private const string SHA256 = "SHA256";
        private const string SHA512 = "SHA512";
        private const string MD5 = "MD5";
        #endregion

        #region Util
        public virtual string ConvertByteToBase64String(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        public virtual byte[] ConvertBase64StringToByte(string input)
        {
            return Convert.FromBase64String(input);
        }

        public virtual string ConvertByteToHex(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", "").ToLower();
        }
        #endregion

        #region Hashing Methods
        public virtual string SHA1MessageHash(string message)
        {
            return Hashing(SHA1, message);
        }

        public virtual string SHA256MessageHash(string message)
        {
            return Hashing(SHA256, message);
        }

        public virtual string SHA512MessageHash(string message)
        {
            return Hashing(SHA512, message);
        }

        public virtual string MD5MessageHash(string message)
        {
            return Hashing(MD5, message);
        }

        public virtual string Hashing(string algorithm, string message)
        {
            dynamic HashAlgorithm = algorithm switch
            {
                SHA1 => new SHA1Managed(),
                SHA256 => new SHA256Managed(),
                SHA512 => new SHA512Managed(),
                MD5 => new MD5CryptoServiceProvider(),
                _ => throw new Exception("Invalid hashing algorithm"),
            };
            byte[] bufferHashed = HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(message));
            string hashed = ConvertByteToBase64String(bufferHashed);
            HashAlgorithm.Clear();
            return hashed;
        }

        public virtual string CreateHash(byte[] data, string hashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = HashAlgorithm.Create(hashAlgorithm);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        public virtual string CreateSaltKey(int size = 5)
        {
            //generate a cryptographic random number
            using var provider = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            provider.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return ConvertByteToBase64String(buff);
        }
        #endregion

        #region Encryption Methods
        public virtual string TripleDES_Encryptor(string plainMessage, byte[] secretKey = null)
        {
            return TripleDESCrypto(plainMessage, ENCRYPT, secretKey);
        }

        public virtual string TripleDES_Decryptor(string ciphterText, byte[] secretKey = null)
        {
            return TripleDESCrypto(ciphterText, DECRYPT, secretKey);
        }

        protected virtual string TripleDESCrypto(string str, string type, byte[] secretKey)
        {
            byte[] defaultSecret = secretKey?.Length > 0 ? secretKey : DefaultSecretKey;
            byte[] secretkey = new byte[24]; // its because c# using 24 bytes to do encryption for 3DES
            Array.Copy(defaultSecret, secretkey, 24);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                //set the secret key for the tripleDES algorithm
                Key = secretkey,
                //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
                Mode = CipherMode.ECB,
                //padding mode(if any extra byte added)
                Padding = PaddingMode.PKCS7
            };

            try
            {
                ICryptoTransform cryptoEngine;
                string data;
                byte[] inputBuffer;
                byte[] results;
                switch (type)
                {
                    case DECRYPT:
                        cryptoEngine = tdes.CreateDecryptor();
                        inputBuffer = ConvertBase64StringToByte(str);
                        results = cryptoEngine.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                        data = Encoding.UTF8.GetString(results);
                        break;
                    case ENCRYPT:
                    default:
                        cryptoEngine = tdes.CreateEncryptor();
                        inputBuffer = Encoding.UTF8.GetBytes(str);
                        results = cryptoEngine.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                        data = ConvertByteToBase64String(results);
                        break;
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new Exception("Unable to encrypt/decrypt due to data input mismatch. Input => " + str);
            }
            finally
            {
                tdes.Clear();
            }
        }

        public bool ValidateRsaMessageWithHash(string encryptedInput, string hashedInput)
        {
            try
            {
                return CreateHash(Encoding.UTF8.GetBytes(encryptedInput), SHA256).Equals(hashedInput);
            }
            catch
            {
                return false;
            }
        }

        public void Rsa_Encryption(string raw, string rsaPublic, out string encryptedData, out string hashedData)
        {
            encryptedData = EncryptRsaMessage(raw, rsaPublic);
            hashedData = CreateHash(Encoding.UTF8.GetBytes(encryptedData), SHA256);
        }

        public string Rsa_Decryption(string encryptedData, string rsaPrivate)
        {
            return DecryptRsaMessage(encryptedData, rsaPrivate);
        }

        public virtual string EncryptRsaMessage(string rawInput, string rsaKey = "")
        {
            if (string.IsNullOrEmpty(rawInput)) return string.Empty;
            if (string.IsNullOrWhiteSpace(rsaKey)) throw new ArgumentException("Invalid Public Key");

            try
            {
                using var rsaProvider = new RSACryptoServiceProvider();
                var inputBytes = Encoding.UTF8.GetBytes(rawInput);
                rsaProvider.ImportCspBlob(Convert.FromBase64String(rsaKey));
                int bufferSize = (rsaProvider.KeySize / 8) - 11;
                var buffer = new byte[bufferSize];
                using MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream();
                while (true)
                {
                    int readSize = inputStream.Read(buffer, 0, bufferSize);
                    if (readSize <= 0) break;
                    var temp = new byte[readSize];
                    Array.Copy(buffer, 0, temp, 0, readSize);
                    var encryptedBytes = rsaProvider.Encrypt(temp, false);
                    outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                }
                return Convert.ToBase64String(outputStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        public virtual string DecryptRsaMessage(string encryptedInput, string rsaKey = "")
        {
            if (string.IsNullOrEmpty(encryptedInput)) return string.Empty;
            if (string.IsNullOrWhiteSpace(rsaKey)) throw new ArgumentException("Invalid Private Key");

            try
            {
                using var rsaProvider = new RSACryptoServiceProvider();
                var inputBytes = Convert.FromBase64String(encryptedInput);
                rsaProvider.ImportCspBlob(Convert.FromBase64String(rsaKey));
                int bufferSize = rsaProvider.KeySize / 8;
                var buffer = new byte[bufferSize];
                using MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream();
                while (true)
                {
                    int readSize = inputStream.Read(buffer, 0, bufferSize);
                    if (readSize <= 0) break;
                    var temp = new byte[readSize];
                    Array.Copy(buffer, 0, temp, 0, readSize);
                    var rawBytes = rsaProvider.Decrypt(temp, false);
                    outputStream.Write(rawBytes, 0, rawBytes.Length);
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
