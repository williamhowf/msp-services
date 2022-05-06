using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Com.GGIT.Common.Util
{
    public class RandomUtil
    {
        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            Random random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Generate random integer number within specified range
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Generate random string from GUID.
        /// Default 10 characters, maximum 32 characters.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomString(int charLength)
        {
            if (charLength < 10) charLength = 10;
            if (charLength > 32) charLength = 32;
            return Guid.NewGuid().ToString().ToLower().Replace("-", "").Substring(0, charLength);
        }

        /// <summary>
        /// Generate random characters
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomChars(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generate random digits
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomDigits(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
