// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// A menu command that uses the symbol under the editor's caret.
	/// </summary>
	public abstract class SymbolUnderCaretCommand : AbstractMenuCommand
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
		
		protected abstract void RunImpl(ITextEditor editor, int caretOffset, ResolveResult symbol);
	}
}
