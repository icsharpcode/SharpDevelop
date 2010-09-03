// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.WixBinding
{
	public class WixFileName
	{
		public const string WixSourceFileExtension = ".wxs";
		public const string WixIncludeFileExtension = ".wxi";

		WixFileName()
		{
		}
		
		/// <summary>
		/// Checks the file extension to see if the file is a Wix file. The file
		/// can either be a Wix source file (.wxs) or a Wix include file (.wxi).
		/// </summary>
		public static bool IsWixFileName(string fileName)
		{
			if (String.IsNullOrEmpty(fileName)) {
				return false;
			}
			string extension = Path.GetExtension(fileName.ToLowerInvariant());
			switch (extension) {
				case WixSourceFileExtension:
					return true;
				case WixIncludeFileExtension:
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Checks whether the file extension is for a Wix source file (.wxs).
		/// </summary>
		public static bool IsWixSourceFileName(string fileName)
		{
			return String.Compare(Path.GetExtension(fileName), WixSourceFileExtension, true) == 0;
		}
	}
}
