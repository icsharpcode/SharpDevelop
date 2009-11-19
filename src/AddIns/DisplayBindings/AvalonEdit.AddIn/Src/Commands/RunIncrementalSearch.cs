// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public class RunIncrementalSearch : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				TextArea textArea = provider.TextEditor.GetService(typeof(TextArea)) as TextArea;
				if (textArea != null) {
					textArea.ActiveInputHandler = new IncrementalSearch(textArea, LogicalDirection.Forward);
				}
			}
		}
	}
	
	public class RunReverseIncrementalSearch : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				TextArea textArea = provider.TextEditor.GetService(typeof(TextArea)) as TextArea;
				if (textArea != null) {
					textArea.ActiveInputHandler = new IncrementalSearch(textArea, LogicalDirection.Backward);
				}
			}
		}
	}
}
