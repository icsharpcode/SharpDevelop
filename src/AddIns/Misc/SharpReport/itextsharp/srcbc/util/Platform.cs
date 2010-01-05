using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Utilities
{
	internal sealed class Platform
	{
		private Platform()
		{
		}

#if NETCF_1_0
		internal static Exception CreateNotImplementedException(
			string message)
		{
			return new Exception("Not implemented: " + message);
		}

		internal static bool Equals(
			object	a,
			object	b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		internal static string GetEnvironmentVariable(
			string variable)
		{
			return null;
		}

		private static string GetNewLine()
		{
			MemoryStream buf = new MemoryStream();
			StreamWriter w = new StreamWriter(buf, Encoding.ASCII);
			w.WriteLine();
			w.Close();
			byte[] bs = buf.ToArray();
			return Encoding.ASCII.GetString(bs, 0, bs.Length);
		}
#else
		internal static Exception CreateNotImplementedException(
			string message)
		{
			return new NotImplementedException(message);
		}

		internal static string GetEnvironmentVariable(
			string variable)
		{
			try
			{
				return Environment.GetEnvironmentVariable(variable);
			}
			catch (System.Security.SecurityException)
			{
				// We don't have the required permission to read this environment variable,
				// which is fine, just act as if it's not set
				return null;
			}
		}

		private static string GetNewLine()
		{
			return Environment.NewLine;
		}
#endif

		internal static readonly string NewLine = GetNewLine();
	}
}
