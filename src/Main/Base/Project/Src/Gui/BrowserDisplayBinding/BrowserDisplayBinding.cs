// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
