// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
	public class IsBreakpointCondition : IConditionEvaluator
	{
		public IsBreakpointCondition()
		{
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null)
				return false;
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorProvider;
			if (provider == null)
				return false;
			if (string.IsNullOrEmpty(provider.TextEditor.FileName))
				return false;
			
			foreach (BreakpointBookmark mark in DebuggerService.Breakpoints) {
				if ((mark.FileName == provider.TextEditor.FileName) &&
				    (mark.LineNumber == provider.TextEditor.Caret.Line))
					return true;
			}
			
			return false;
		}
	}
}
