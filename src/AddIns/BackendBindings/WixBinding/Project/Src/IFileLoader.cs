// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		Bitmap GetBitmap(string fileName);
	}
}
