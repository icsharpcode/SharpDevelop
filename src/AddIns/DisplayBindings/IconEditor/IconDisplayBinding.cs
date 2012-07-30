// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.IconEditor
{
	public class IconDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true; // definition in .addin does extension-based filtering
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new IconViewContent(file);
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(string fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
}
