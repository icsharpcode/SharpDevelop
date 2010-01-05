using System;

using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Utilities.Net
{
	public class IPAddress
	{
		/**
		* Validate the given IPv4 or IPv6 address.
		* 
		* @param address the IP address as a string.
		* 
		* @return true if a valid address, false otherwise
		*/
		public static bool IsValid(
			string address)
		{
			return IsValidIPv4(address) || IsValidIPv6(address);
		}

		/**
		* Validate the given IPv4 address.
		* 
		* @param address the IP address as a string.
		*
		* @return true if a valid IPv4 address, false otherwise
		*/
		private static bool IsValidIPv4(
			string address)
		{
			if (address.Length == 0)
				return false;

			BigInteger octet;
			int octets = 0;

			string temp = address + ".";

			int pos;
			int start = 0;
			while (start < temp.Length
				&& (pos = temp.IndexOf('.', start)) > start)
			{
				if (octets == 4)
					return false;

				try
				{
					octet = new BigInteger(temp.Substring(start, pos - start));
				}
				catch (FormatException)
				{
					return false;
				}

				if (octet.SignValue < 0 || octet.BitLength > 8)
					return false;

				start = pos + 1;
				++octets;
			}

			return octets == 4;
		}

		/**
		* Validate the given IPv6 address.
		*
		* @param address the IP address as a string.
		*
		* @return true if a valid IPv4 address, false otherwise
		*/
		private static bool IsValidIPv6(
			string address)
		{
			if (address.Length == 0)
				return false;

			BigInteger octet;
			int octets = 0;

			string temp = address + ":";

			int pos;
			int start = 0;
			while (start < temp.Length
				&& (pos = temp.IndexOf(':', start)) > start)
			{
				if (octets == 8)
					return false;

				try
				{
					octet = new BigInteger(temp.Substring(start, pos - start), 16);
				}
				catch (FormatException)
				{
					return false;
				}

				if (octet.SignValue < 0 || octet.BitLength > 16)
					return false;

				start = pos + 1;
				octets++;
			}

			return octets == 8;
		}
	}
}
