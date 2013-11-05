// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			string fileNameStr = fileName;
			return fileNameStr.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
				|| fileNameStr.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
				|| fileNameStr.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase)
				|| fileNameStr.StartsWith("browser:", StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			string fileName = file.FileName;
			
			BrowserPane browserPane = new BrowserPane();
			if (fileName.StartsWith("browser://", StringComparison.OrdinalIgnoreCase)) {
				browserPane.Navigate(fileName.Substring("browser://".Length));
			} else {
				browserPane.Navigate(fileName);
			}
			return browserPane;
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return CanCreateContentForFile(fileName);
		}
		
		public double AutoDetectFileContent(FileName fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
}
