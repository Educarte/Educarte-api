using System.Security.Cryptography;
using System.Text;

namespace Api.Infrastructure.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class HashService
    {
        private readonly Random random;
        private readonly SHA512 shaAlgo;

        /// <summary>
        /// 
        /// </summary>
        public HashService()
        {
            random = new Random();
            shaAlgo = SHA512.Create();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GenerateSalt(int size)
        {
            var result = new byte[size];
            random.NextBytes(result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] salt, string data)
        {
            var chunk = salt.Concat(Encoding.UTF8.GetBytes(data)).ToArray();
            return shaAlgo.ComputeHash(chunk);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public (byte[] hash, byte[] salt) Encrypt(string data)
        {
            var salt = GenerateSalt(128);
            var hash = Encrypt(salt, data);
            return (hash, salt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Compare(byte[] hash, byte[] salt, string data)
        {
            var dataHashed = Encrypt(salt, data);
            return hash.SequenceEqual(dataHashed);
        }
    }
}
