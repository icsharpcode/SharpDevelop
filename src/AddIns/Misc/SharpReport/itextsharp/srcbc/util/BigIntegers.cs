using System;

using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Utilities
{
	/**
	 * BigInteger utilities.
	 */
	public sealed class BigIntegers
	{
		private BigIntegers()
		{
		}

		/**
		* Return the passed in value as an unsigned byte array.
		*
		* @param value value to be converted.
		* @return a byte array without a leading zero byte if present in the signed encoding.
		*/
		public static byte[] AsUnsignedByteArray(
			BigInteger n)
		{
			return n.ToByteArrayUnsigned();
		}
	}
}
