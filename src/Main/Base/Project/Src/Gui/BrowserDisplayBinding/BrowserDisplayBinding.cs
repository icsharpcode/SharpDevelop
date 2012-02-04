// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return fileName.StartsWith("http:")
				|| fileName.StartsWith("https:")
				|| fileName.StartsWith("ftp:")
				|| fileName.StartsWith("browser:");
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			string fileName = file.FileName;
			
			BrowserPane browserPane = new BrowserPane();
			if (fileName.StartsWith("browser://")) {
				browserPane.Navigate(fileName.Substring("browser://".Length));
			} else {
				browserPane.Navigate(fileName);
			}
			return browserPane;
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return CanCreateContentForFile(fileName);
		}
		
		public double AutoDetectFileContent(string fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
}
