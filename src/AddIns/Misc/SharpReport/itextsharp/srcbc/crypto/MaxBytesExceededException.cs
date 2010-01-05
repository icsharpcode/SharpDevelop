using System;

namespace Org.BouncyCastle.Crypto
{
	/// <summary>
	/// This exception is thrown whenever a cipher requires a change of key, iv
	/// or similar after x amount of bytes enciphered
	/// </summary>
	public class MaxBytesExceededException
		: CryptoException
	{
		public MaxBytesExceededException()
		{
		}

		public MaxBytesExceededException(
			string message)
			: base(message)
		{
		}

		public MaxBytesExceededException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
