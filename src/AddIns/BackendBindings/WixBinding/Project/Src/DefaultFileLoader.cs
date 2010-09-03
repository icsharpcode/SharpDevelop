// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;

namespace ICSharpCode.WixBinding
{
	public class DefaultFileLoader : IFileLoader
	{
		public DefaultFileLoader()
		{
		}
		
		public Bitmap LoadBitmap(string fileName)
		{
			if (File.Exists(fileName)) {
				return new Bitmap(fileName);
			}
			return null;
		}
	}
}
