using System;
using System.Text;

namespace Org.BouncyCastle.Utilities
{

    /// <summary> General array utilities.</summary>
    public sealed class Arrays
    {
        private Arrays()
        {
        }

        /// <summary>
        /// Are two arrays equal.
        /// </summary>
        /// <param name="a">Left side.</param>
        /// <param name="b">Right side.</param>
        /// <returns>True if equal.</returns>
        public static bool AreEqual(
			byte[]	a,
			byte[]	b)
        {
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return HaveSameContents(a, b);
		}

		[Obsolete("Use 'AreEqual' method instead")]
		public static bool AreSame(
			byte[]	a,
			byte[]	b)
		{
			return AreEqual(a, b);
		}

		public static bool AreEqual(
			int[]	a,
			int[]	b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return HaveSameContents(a, b);
		}

		private static bool HaveSameContents(
			byte[]	a,
			byte[]	b)
		{
			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
					return false;
			}

			return true;
		}

		private static bool HaveSameContents(
			int[]	a,
			int[]	b)
		{
			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
					return false;
			}

			return true;
		}

		public static string ToString(
			object[] a)
		{
			StringBuilder sb = new StringBuilder('[');
			if (a.Length > 0)
			{
				sb.Append(a[0]);
				for (int index = 1; index < a.Length; ++index)
				{
					sb.Append(", ").Append(a[index]);
				}
			}
			sb.Append(']');
			return sb.ToString();
		}

		public static int GetHashCode(
			byte[] data)
		{
			if (data == null)
			{
				return 0;
			}

			int i = data.Length;
			int hc = i + 1;

			while (--i >= 0)
			{
				hc *= 257;
				hc ^= data[i];
			}

			return hc;
		}

		public static byte[] Clone(
			byte[] data)
		{
			return data == null ? null : (byte[]) data.Clone();
		}

		public static int[] Clone(
			int[] data)
		{
			return data == null ? null : (int[]) data.Clone();
		}
	}
}
