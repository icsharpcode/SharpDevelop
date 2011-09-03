// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

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
				ITextEditor editor = editorProvider.TextEditor;
				var resolveResult = ParserService.Resolve(editor.FileName, editor.Caret.Location, editor.Document);
				RunImpl(editor, editor.Caret.Offset, resolveResult);
			}
		}
		
		protected abstract void RunImpl(ITextEditor editor, int caretOffset, ResolveResult symbol);
		
		protected IEntity GetEntity(ResolveResult symbol)
		{
			TypeResolveResult trr = symbol as TypeResolveResult;
			if (trr != null)
				return trr.Type.GetDefinition();
			MemberResolveResult mrr = symbol as MemberResolveResult;
			if (mrr != null)
				return mrr.Member.MemberDefinition;
			return null;
		}
	}
}
