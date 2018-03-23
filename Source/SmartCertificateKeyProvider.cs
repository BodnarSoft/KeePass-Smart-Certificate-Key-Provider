namespace SmartCertificateKeyProviderPlugin
{
    using System;
    using KeePass.Plugins;
    using KeePassLib.Keys;

    public class SmartCertificateKeyProvider : KeyProvider
    {
        #region Constructors

        public SmartCertificateKeyProvider(IPluginHost host)
        {
            Host = host;
        }

        #endregion

        #region Public properties

        public override string Name { get; }

        #endregion

        #region Private properties

        private IPluginHost Host { get; }

        #endregion

        #region Public methods

        public override byte[] GetKey(KeyProviderQueryContext ctx) => throw new NotImplementedException();

        #endregion
    }
}