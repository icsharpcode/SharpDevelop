using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Cms
{
	/**
	* a holding class for a file of data to be processed.
	*/
	public class CmsProcessableFile
		: CmsProcessable
	{
		private const int DefaultBufSize = 32 * 1024;

		private readonly FileInfo	_file;
		private readonly int		_bufSize;

		public CmsProcessableFile(
			FileInfo file)
			: this(file, DefaultBufSize)
		{
		}

		public CmsProcessableFile(
			FileInfo	file,
			int			bufSize)
		{
			_file = file;
			_bufSize = bufSize;
		}

		public virtual Stream Read()
		{
			return new FileStream(
				_file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, _bufSize);
		}

		public virtual void Write(
			Stream zOut)
		{
			Stream inStr = Read();
			Streams.PipeAll(inStr, zOut);
			inStr.Close();
		}

		/// <returns>The file handle</returns>
		public virtual object GetContent()
		{
			return _file;
		}
	}
}
