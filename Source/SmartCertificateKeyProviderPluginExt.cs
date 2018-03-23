namespace SmartCertificateKeyProviderPlugin
{
    using KeePass.Plugins;

    public sealed class SmartCertificateKeyProviderPluginExt : Plugin
    {
        #region Constructors

        public SmartCertificateKeyProviderPluginExt()
        {
            Host = null;
        }

        #endregion

        #region Private properties

        private IPluginHost Host { get; set; }

        private SmartCertificateKeyProvider SmartCertificateKeyProvider { get; set; }

        #endregion

        #region Public methods

        public override bool Initialize(IPluginHost host)
        {
            Host = host;
            SmartCertificateKeyProvider = new SmartCertificateKeyProvider(Host);

            Host.KeyProviderPool.Add(SmartCertificateKeyProvider);

            return base.Initialize(host);
        }

        public override void Terminate()
        {
            Host.KeyProviderPool.Remove(SmartCertificateKeyProvider);

            SmartCertificateKeyProvider.Dispose();
            SmartCertificateKeyProvider = null;

            base.Terminate();
        }

        #endregion
    }
}