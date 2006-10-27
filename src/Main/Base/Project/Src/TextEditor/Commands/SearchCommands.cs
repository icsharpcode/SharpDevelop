// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GotoLineNumber : AbstractMenuCommand
	{
		public override void Run()
		{
			GotoDialog.ShowSingleInstance();
		}
	}
	
	public class GotoMatchingBrace : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoMatchingBrace();
			}
		}
	}
	
	public class RunIncrementalSearch : AbstractMenuCommand
	{
		static IncrementalSearch incrementalSearch;
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				ITextEditorControlProvider textEditorControlProvider = window.ActiveViewContent as ITextEditorControlProvider;
				if (textEditorControlProvider != null) {
					if (incrementalSearch != null) {
						incrementalSearch.Dispose();
					}
					incrementalSearch = new IncrementalSearch(textEditorControlProvider.TextEditorControl, Forwards);
				}
			}
		}
		
		protected virtual bool Forwards {
			get { 
				return true;
			}
		}
	}
	
	public class RunReverseIncrementalSearch : RunIncrementalSearch
	{
		protected override bool Forwards {
			get { 
				return false;
			}
		}
	}
}
