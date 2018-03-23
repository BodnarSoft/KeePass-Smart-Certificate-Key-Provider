namespace SmartCertificateKeyProviderPlugin
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using KeePass.Forms;
    using KeePass.Plugins;
    using KeePassLib.Keys;
    using KeePassLib.Utility;

    public class SmartCertificateKeyProvider : KeyProvider, IDisposable
    {
        #region Constants

        private const string DefaultSignatureDataText = "Data text for KeePass Password Safe Plugin - {F3EF424C-7517-4D58-A3FB-C1FB458FDDB6}!"; // DO NOT CHANGE THIS!!!!

        #endregion

        #region Private static properties

        private static X509Certificate2[] UserCertificates
        {
            get
            {
                var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                myStore.Open(OpenFlags.ReadOnly);

                var certificates = myStore.Certificates.Cast<X509Certificate2>()
                                          .Where(c => c.HasPrivateKey)
                                          .ToArray();

                myStore.Close();

                return certificates;
            }
        }

        #endregion

        #region Constructors

        public SmartCertificateKeyProvider(IPluginHost host)
        {
            Host = host;
            CertificateCache = new UsedCertificateCache();

            Host.MainWindow.FileOpened += OnDatabaseOpened;
            DataToSign = Encoding.UTF8.GetBytes(DefaultSignatureDataText);
        }

        #endregion

        #region Public properties

        public override bool DirectKey => true; // DO NOT CHANGE THIS!!!!

        public override bool GetKeyMightShowGui => true;

        public override string Name => "Smart Certificate Key Provider";

        public override bool SecureDesktopCompatible => true;

        #endregion

        #region Private properties

        private UsedCertificateCache CertificateCache { get; }

        private byte[] DataToSign { get; }

        private IPluginHost Host { get; }

        #endregion

        #region Public methods

        public void Dispose()
        {
            Host.MainWindow.FileOpened -= OnDatabaseOpened;

            CertificateCache.Dispose();

            GC.SuppressFinalize(this);
        }

        public override byte[] GetKey(KeyProviderQueryContext keyProviderQueryContext)
        {
            X509Certificate2 certificate = null;

            if (!keyProviderQueryContext.CreatingNewKey)
                certificate = GetCertificateFromCache(keyProviderQueryContext.DatabasePath);

            if (certificate == null)
            {
                var title = "Available certificates";
                var message = "Select certificate to use it for encryption on your KeePass database.";

                var x509Certificates = X509Certificate2UI.SelectFromCollection(new X509Certificate2Collection(UserCertificates), title, message, X509SelectionFlag.SingleSelection)
                                                         .Cast<X509Certificate2>();

                certificate = x509Certificates.FirstOrDefault();
            }

            if (certificate == null)
                MessageService.ShowInfo("No valid certificate selected!");
            else
            {
                try
                {
                    if (certificate.PrivateKey is RSA rsa)
                    {
                        CertificateCache.StoreCachedValue(keyProviderQueryContext.DatabasePath, certificate.Thumbprint);

                        return rsa.SignData(DataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1); // DO NOT CHANGE THIS!!!!;
                    }
                }
                catch (Exception ex)
                {
                    MessageService.ShowWarning($"Selected certificate can't be used!\nReason: {ex.Message}.");
                }
            }

            return null;
        }

        #endregion

        #region Private methods

        private X509Certificate2 GetCertificateFromCache(string databasePath)
        {
            try
            {
                var thumbprint = CertificateCache.GetCachedValue(databasePath);

                if (thumbprint != null)
                    return UserCertificates.SingleOrDefault(c => c.Thumbprint.Equals(thumbprint));
            }
            catch (Exception ex)
            {
                MessageService.ShowWarning($"Selected certificate can't be used!\nReason: {ex.Message}.");
            }

            return null;
        }

        private void OnDatabaseOpened(object sender, FileOpenedEventArgs args)
        {
            var path = args.Database.IOConnectionInfo.Path;

            CertificateCache.SetCachedItemAsValid(path);
        }

        #endregion
    }
}