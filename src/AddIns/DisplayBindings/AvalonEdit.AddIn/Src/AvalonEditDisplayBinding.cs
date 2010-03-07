// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditDisplayBinding : IDisplayBinding
	{
		static bool addInHighlightingDefinitionsRegistered;
		
		internal static void RegisterAddInHighlightingDefinitions()
		{
			WorkbenchSingleton.AssertMainThread();
			if (!addInHighlightingDefinitionsRegistered) {
				foreach (AddInTreeSyntaxMode syntaxMode in AddInTree.BuildItems<AddInTreeSyntaxMode>(SyntaxModeDoozer.Path, null, false)) {
					syntaxMode.Register(HighlightingManager.Instance);
				}
				addInHighlightingDefinitionsRegistered = true;
			}
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			RegisterAddInHighlightingDefinitions();
			return new AvalonEditViewContent(file);
		}
	}
}
