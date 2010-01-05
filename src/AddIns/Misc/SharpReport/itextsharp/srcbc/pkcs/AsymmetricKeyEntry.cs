using System.Collections;

using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Pkcs
{
    public class AsymmetricKeyEntry
        : Pkcs12Entry
    {
        private readonly AsymmetricKeyParameter key;

		public AsymmetricKeyEntry(
            AsymmetricKeyParameter key)
			: base(new Hashtable())
        {
            this.key = key;
        }

		public AsymmetricKeyEntry(
            AsymmetricKeyParameter	key,
            Hashtable				attributes)
			: base(attributes)
        {
            this.key = key;
        }

		public AsymmetricKeyParameter Key
        {
            get { return this.key; }
        }
    }
}
