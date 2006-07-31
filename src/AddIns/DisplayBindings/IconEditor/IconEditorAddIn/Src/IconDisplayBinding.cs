// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.IconEditorAddIn
{
	public class IconDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true; // definition in .addin does extension-based filtering
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			IconViewContent vc = new IconViewContent();
			vc.Load(fileName);
			return vc;
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return false;
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			throw new NotSupportedException();
		}
	}
}
