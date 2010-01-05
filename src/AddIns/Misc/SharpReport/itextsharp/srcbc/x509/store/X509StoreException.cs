using System;

namespace Org.BouncyCastle.X509.Store
{
	public class X509StoreException
		: Exception
	{
		public X509StoreException()
		{
		}

		public X509StoreException(
			string message)
			: base(message)
		{
		}

		public X509StoreException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
