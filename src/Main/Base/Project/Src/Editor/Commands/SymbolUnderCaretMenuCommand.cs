// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// A menu command that uses the symbol under the editor's caret.
	/// </summary>
	public abstract class SymbolUnderCaretMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider editorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (editorProvider != null) {
				Run(editorProvider.TextEditor, editorProvider.TextEditor.Caret.Offset);
			}
		}
		
		public void Run(ITextEditor editor, int caretOffset)
		{
			var resolveResult = ParserService.Resolve(caretOffset, editor.Document, editor.FileName);
			RunImpl(editor, caretOffset, resolveResult);
		}
		
		protected abstract void RunImpl(ITextEditor editor, int caretOffset, ResolveResult symbol);
		
		public IClass GetClass(ResolveResult symbol)
		{
			if (symbol == null || !(symbol is TypeResolveResult)) {
				return null;
			}
			return ((TypeResolveResult)symbol).ResolvedClass;
		}
		
		public IMember GetMember(ResolveResult symbol)
		{
			if (symbol == null || !(symbol is MemberResolveResult)) {
				return null;
			}
			return ((MemberResolveResult)symbol).ResolvedMember;
		}
	}
}
