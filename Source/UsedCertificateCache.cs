namespace SmartCertificateKeyProviderPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using SmartCertificateKeyProviderPlugin.Extensions;

    internal sealed class UsedCertificateCache : IDisposable
    {
        #region Constructors

        public UsedCertificateCache()
        {
            Cache = new Dictionary<string, UsedCertificateCacheItem>(StringComparer.Ordinal);
        }

        #endregion

        #region Private properties

        private IDictionary<string, UsedCertificateCacheItem> Cache { get; }

        #endregion

        #region Public methods

        public void Dispose()
        {
            Cache.Clear();

            GC.SuppressFinalize(this);
        }

        public string GetCachedValue(string databasePath)
        {
            var path = EncryptPath(databasePath);

            if (Cache.TryGetValue(path, out var value))
            {
                if (value.IsValid)
                {
                    var bytes = value.Data.AsEnumerable()
                                     .ToArray();

                    ProtectedMemory.Unprotect(bytes, MemoryProtectionScope.SameProcess);

                    return Encoding.UTF8.GetString(bytes)
                                   .Trim();
                }
            }

            return null;
        }

        public void SetCachedItemAsValid(string databasePath)
        {
            var path = EncryptPath(databasePath);

            if (Cache.TryGetValue(path, out var value))
            {
                value.IsValid = true;
                value.ClearAttempts();
            }
        }

        public void StoreCachedValue(string databasePath, string thumbprint)
        {
            var path = EncryptPath(databasePath);

            if (!Cache.ContainsKey(path))
                Cache.Add(path, new UsedCertificateCacheItem());

            var usedCertificateCacheItem = Cache[path];

            var bytes = Encoding.UTF8.GetBytes(thumbprint.PadRightToMachLengthOfMultiply(16));
            ProtectedMemory.Protect(bytes, MemoryProtectionScope.SameProcess);

            if (usedCertificateCacheItem.Data.SequenceEqual(bytes))
            {
                if (usedCertificateCacheItem.Attempt >= 3)
                    usedCertificateCacheItem.IsValid = false;

                usedCertificateCacheItem.Attempt++;
            }
            else
                usedCertificateCacheItem.SetNewValue(bytes);
        }

        #endregion

        #region Private methods

        private string EncryptPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return path.Trim()
                       .ToLower()
                       .GenerateSha256Hash();
        }

        #endregion

        #region Classes

        private sealed class UsedCertificateCacheItem
        {
            #region Constructors

            public UsedCertificateCacheItem()
            {
                IsValid = false;
                SetNewValue(null);
                ClearAttempts();
            }

            #endregion

            #region Public properties

            public byte Attempt { get; set; }

            public byte[] Data { get; private set; }

            public bool IsValid { get; set; }

            #endregion

            #region Public methods

            public void ClearAttempts()
            {
                Attempt = 0;
            }

            public void SetNewValue(byte[] data)
            {
                Data = data ?? new byte[0];
                ClearAttempts();
                IsValid = false;
            }

            #endregion
        }

        #endregion
    }
}