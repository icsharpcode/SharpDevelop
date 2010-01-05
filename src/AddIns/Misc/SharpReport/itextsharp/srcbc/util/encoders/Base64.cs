using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public sealed class Base64
	{
//		private static readonly IEncoder encoder = new Base64Encoder();

		private Base64()
		{
		}

		/**
		 * encode the input data producing a base 64 encoded byte array.
		 *
		 * @return a byte array containing the base 64 encoded data.
		 */
		public static byte[] Encode(
			byte[] data)
		{
			string s = Convert.ToBase64String(data, 0, data.Length);
			return Encoding.ASCII.GetBytes(s);

//			MemoryStream bOut = new MemoryStream();
//			encoder.Encode(data, 0, data.Length, bOut);
//			return bOut.ToArray();
		}

		/**
		 * Encode the byte data to base 64 writing it to the given output stream.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Encode(
			byte[]	data,
			Stream	outStream)
		{
			string s = Convert.ToBase64String(data, 0, data.Length);
			byte[] encoded = Encoding.ASCII.GetBytes(s);
			outStream.Write(encoded, 0, encoded.Length);
			return encoded.Length;

//			return encoder.Encode(data, 0, data.Length, outStream);
		}

		/**
		 * Encode the byte data to base 64 writing it to the given output stream.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Encode(
			byte[]	data,
			int		off,
			int		length,
			Stream	outStream)
		{
			string s = Convert.ToBase64String(data, off, length);
			byte[] encoded = Encoding.ASCII.GetBytes(s);
			outStream.Write(encoded, 0, encoded.Length);
			return encoded.Length;

//			return encoder.Encode(data, off, length, outStream);
		}

		/**
		 * decode the base 64 encoded input data. It is assumed the input data is valid.
		 *
		 * @return a byte array representing the decoded data.
		 */
		public static byte[] Decode(
			byte[] data)
		{
			string s = Encoding.ASCII.GetString(data, 0, data.Length);
			return Convert.FromBase64String(s);

//			MemoryStream bOut = new MemoryStream();
//			encoder.Decode(data, 0, data.Length, bOut);
//			return bOut.ToArray();
		}

		/**
		 * decode the base 64 encoded string data - whitespace will be ignored.
		 *
		 * @return a byte array representing the decoded data.
		 */
		public static byte[] Decode(
			string data)
		{
			return Convert.FromBase64String(data);

//			MemoryStream bOut = new MemoryStream();
//			encoder.DecodeString(data, bOut);
//			return bOut.ToArray();
		}

		/**
		 * decode the base 64 encoded string data writing it to the given output stream,
		 * whitespace characters will be ignored.
		 *
		 * @return the number of bytes produced.
		 */
		public static int Decode(
			string	data,
			Stream	outStream)
		{
			byte[] decoded = Decode(data);
			outStream.Write(decoded, 0, decoded.Length);
			return decoded.Length;

//			return encoder.DecodeString(data, outStream);
		}
	}
}
