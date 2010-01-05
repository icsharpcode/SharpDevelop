using System;

namespace Org.BouncyCastle.Tsp
{
	public class TspException
		: Exception
	{
		public TspException()
		{
		}

		public TspException(
			string message)
			: base(message)
		{
		}

		public TspException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
