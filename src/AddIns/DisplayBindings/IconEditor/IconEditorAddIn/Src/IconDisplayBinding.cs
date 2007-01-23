// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.IconEditorAddIn
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
	}
}
