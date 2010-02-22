// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui.Pads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Debugger.AddIn.Service;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn
{
	public class EnableBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorProvider;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditor.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditor.Caret.Line)) {
					point = breakpoint;
					break;
				}
			}
			
			if (point != null) {
				point.IsEnabled = true;
			}
		}
	}
}
