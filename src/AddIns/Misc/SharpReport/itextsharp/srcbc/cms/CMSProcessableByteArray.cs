using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	/**
	* a holding class for a byte array of data to be processed.
	*/
	public class CmsProcessableByteArray
		:    CmsProcessable
	{
		private readonly byte[] bytes;

		public CmsProcessableByteArray(
			byte[] bytes)
		{
			this.bytes = bytes;
		}

		public virtual Stream Read()
		{
			return new MemoryStream(bytes, false);
		}

		public virtual void Write(Stream zOut)
		{
			zOut.Write(bytes, 0, bytes.Length);
		}

		/// <returns>A clone of the byte array</returns>
		public virtual object GetContent()
		{
			return bytes.Clone();
		}
	}
}
