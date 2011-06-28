// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of FindDerivedClassesOrOverrides.
	/// </summary>
	public class FindDerivedClassesOrOverrides : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			var classUnderCaret = GetClass(symbol);
			if (classUnderCaret != null) {
				ContextActionsHelper.MakePopupWithDerivedClasses(classUnderCaret).OpenAtCaretAndFocus();
				return;
			}
			var memberUnderCaret = GetMember(symbol);
			if (memberUnderCaret != null && memberUnderCaret.IsOverridable)
			{
				ContextActionsHelper.MakePopupWithOverrides(memberUnderCaret).OpenAtCaretAndFocus();
				return;
			}
			MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassOrOverridableSymbolUnderCursorError}");
		}
		
		// TODO
//		public override bool IsEnabled {
//			get { 
//				WorkbenchSingleton.Workbench.ActiveViewContent.
//				var symbol = ParserService.Resolve(
//				var classUnderCaret = GetClass(symbol);
//				if (classUnderCaret != 				
//			}
//			set { base.IsEnabled = value; }
//		}
	}
}
