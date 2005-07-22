// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GoToDefinition : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				// TODO: use click position instead of cursor position
				return new ICSharpCode.SharpDevelop.DefaultEditor.Actions.GoToDefinition();
			}
		}
	}
	/*
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
	*/
}
