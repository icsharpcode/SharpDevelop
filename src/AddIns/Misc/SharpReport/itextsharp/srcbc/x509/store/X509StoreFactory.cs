using System;
using System.Globalization;

namespace Org.BouncyCastle.X509.Store
{
	public sealed class X509StoreFactory
	{
		private X509StoreFactory()
		{
		}

		public static IX509Store Create(
			string					type,
			IX509StoreParameters	parameters)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			string[] parts = type.ToUpper(CultureInfo.InvariantCulture).Split('/');

			if (parts.Length < 2)
				throw new ArgumentException("type");


			switch (parts[0])
			{
				case "ATTRIBUTECERTIFICATE":
				case "CERTIFICATE":
				case "CERTIFICATEPAIR":
				case "CRL":
				{
					if (parts[1] == "COLLECTION")
					{
						X509CollectionStoreParameters p = (X509CollectionStoreParameters) parameters;
						return new X509CollectionStore(p.GetCollection());
					}
					break;
				}
			}

			throw new NoSuchStoreException("X.509 store type '" + type + "' not available.");
		}
	}
}
