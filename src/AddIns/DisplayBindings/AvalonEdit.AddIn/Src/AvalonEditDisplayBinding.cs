// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditDisplayBinding : IDisplayBinding
	{
		const string path = "/SharpDevelop/ViewContent/DefaultTextEditor/SyntaxModes";
		bool builtAddInHighlighting;
		
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			BuildAddInHighlighting();
			return new AvalonEditViewContent(file);
		}
		
		void BuildAddInHighlighting()
		{
			if (!builtAddInHighlighting) {
				builtAddInHighlighting = true;
				AddInTree.BuildItems<object>(path, this, false);
			}
		}
	}
}
