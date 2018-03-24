namespace SmartCertificateKeyProviderPlugin.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class StringExtension
    {
        #region Static Public methods

        public static string GenerateSha256Hash(this string text)
        {
            using (var sha = new SHA256Cng())
            {
                var computeHash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                return Convert.ToBase64String(computeHash);
            }
        }

        public static string PadRightToMachLengthOfMultiply(this string text, byte multiplyBy)
        {
            var length = text.Length;

            if (length % multiplyBy == 0)
                return text;

            var size = Convert.ToDouble(length) / multiplyBy;
            var newSize = Convert.ToInt32(Math.Ceiling(size) * multiplyBy);

            return text.PadRight(newSize);
        }

        #endregion
    }
}