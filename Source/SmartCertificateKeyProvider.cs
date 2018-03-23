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

        #region Constructors

        public SmartCertificateKeyProvider(IPluginHost host)
        {
            Host = host;
            Host.MainWindow.FileOpened += OnDatabaseOpened;
        }

        #endregion

        #region Public properties

        public override bool DirectKey => true;

        public override bool GetKeyMightShowGui => true;

        public override string Name => "Smart Certificate Key Provider";

        public override bool SecureDesktopCompatible => true;

        #endregion

        #region Private properties

        private IPluginHost Host { get; }

        #endregion

        #region Public methods

        public void Dispose()
        {
            Host.MainWindow.FileOpened -= OnDatabaseOpened;

            GC.SuppressFinalize(this);
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            var title = "Available certificates";
            var message = "Select certificate to use it for encryption on your KeePass database.";

            var x509Certificates = X509Certificate2UI.SelectFromCollection(GetCertificates(), title, message, X509SelectionFlag.SingleSelection)
                                                     .Cast<X509Certificate2>();

            var certificate = x509Certificates.FirstOrDefault();

            if (certificate == null)
                MessageService.ShowInfo("No valid certificate selected!");
            else
            {
                try
                {
                    if (certificate.PrivateKey is RSA rsa)
                    {
                        var dataToSign = Encoding.UTF8.GetBytes(DefaultSignatureDataText);
                        return rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }
                }
                catch (Exception ex)
                {
                    MessageService.ShowWarning($"Selected certificate can't be used!\nReason: {ex.Message}.");
                }
            }

            return null;

            X509Certificate2Collection GetCertificates()
            {
                var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                myStore.Open(OpenFlags.ReadOnly);

                var certificates = myStore.Certificates.Cast<X509Certificate2>()
                                          .Where(c => c.HasPrivateKey)
                                          .ToArray();

                myStore.Close();

                return new X509Certificate2Collection(certificates);
            }
        }

        #endregion

        #region Private methods

        private void OnDatabaseOpened(object sender, FileOpenedEventArgs args)
        {
            var path = args.Database.IOConnectionInfo.Path;
        }

        #endregion
    }
}