using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tengu.Utility
{
    public class CryptoHandler
    {
        private const int SaltByteSize = 32;
        private const int HashByteSize = 32;
        private const int HasingIterationsCount = 10101;

        public byte[] GenerateSalt(int saltByteSize = SaltByteSize)
        {
            using (RNGCryptoServiceProvider saltGenerator = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[saltByteSize];
                saltGenerator.GetBytes(salt);
                return salt;
            }
        }
        public byte[] ComputeHash(string password, byte[] salt, int iterations = HasingIterationsCount, int hashByteSize = HashByteSize)
        {
            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                hashGenerator.IterationCount = iterations;
                return hashGenerator.GetBytes(hashByteSize);
            }
        }
        public bool VerifyPassword(string password, string passwordSalt, string passwordHash)
        {
            byte[] saltByte = StringToByte(passwordSalt);
            byte[] hashByte = StringToByte(passwordHash);


            byte[] computedHash = ComputeHash(password, saltByte);
            return AreHashesEqual(computedHash, hashByte);
        }
        public string ByteToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public byte[] StringToByte(string bytes)
        {
            return Convert.FromBase64String(bytes);
        }
        private bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int minHashLenght = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < minHashLenght; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
