using System;

namespace Org.BouncyCastle.Crypto
{
    public abstract class CryptoException
		: Exception
    {
        protected CryptoException()
        {
        }

		protected CryptoException(
            string message)
			: base(message)
        {
        }

		protected CryptoException(
            string		message,
            Exception	exception)
			: base(message, exception)
        {
        }
    }
}
