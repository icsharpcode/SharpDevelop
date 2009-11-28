// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
