using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
    public interface CmsProcessable
    {
		/// <summary>
		/// Return a stream from which the data can be read.
		/// </summary>
		/// <remarks>
		/// This routine may be called more than once, but previous returned
		/// streams should be closed first.
		/// </remarks>
		Stream Read();

		/// <summary>
		/// Generic routine to copy out the data we want processed.
		/// </summary>
		/// <remarks>
		/// This routine may be called multiple times.
		/// </remarks>
        void Write(Stream outStream);

        object GetContent();
    }
}
