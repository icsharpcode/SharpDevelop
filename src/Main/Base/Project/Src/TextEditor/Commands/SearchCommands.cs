// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

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
