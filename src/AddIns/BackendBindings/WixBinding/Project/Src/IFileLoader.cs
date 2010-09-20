// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Hides the file system from the WixDialog and WixDocument class.
	/// </summary>
	public interface IFileLoader
	{
		/// <summary>
		/// Loads the bitmap from the specified file if the file exists.
		/// </summary>
		/// <returns>
		/// <see langword="null"/> if the file does not exist.
		/// </returns>
		Bitmap LoadBitmap(string fileName);
	}
}
