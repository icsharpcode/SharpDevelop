// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Helper class for <see cref="IContextActionsProvider.GetAvailableActions"></see>.
	/// Never keep long-lived references to this class
	/// - the AST serves as one-time cache and does not get updated when editor text changes.
	/// </summary>
	public class EditorContext
	{
		public ITextEditor Editor { get; private set; }
		public SnippetParser snippetParser { get; private set; }
		public NRefactoryResolver resolver { get; private set; }
		
		public EditorContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.snippetParser = GetSnippetParser(editor);
			this.resolver = GetResolver(editor);
		}
		
		// TODO make all reference types cached ResolveResult? - implement own Nullable<T>
		ResolveResult symbolUnderCaret;
		public ResolveResult SymbolUnderCaret
		{
			get
			{
				if (symbolUnderCaret != null)
					return symbolUnderCaret;
				// workaround so that Resolve works when the caret is placed also at the end of the word
				symbolUnderCaret = ParserService.Resolve(Editor.Caret.Line, Editor.Caret.Column - 1, Editor.Document, Editor.FileName);
				if (symbolUnderCaret == null)
					symbolUnderCaret = ParserService.Resolve(Editor.Caret.Line, Editor.Caret.Column, Editor.Document, Editor.FileName);
				return symbolUnderCaret;
			}
		}
		
		IDocumentLine currentLine;
		public IDocumentLine CurrentLine
		{
			get
			{
				if (currentLine != null)
					return currentLine;
				try
				{
					return (currentLine = Editor.Document.GetLine(Editor.Caret.Line));
				}
				catch
				{
					return null;
				}
			}
		}
		
		INode currentLineAST;
		public INode CurrentLineAST
		{
			get
			{
				if (currentLineAST != null)
					return currentLineAST;
				if (this.snippetParser == null || this.CurrentLine == null)
					return null;
				try {
					return (currentLineAST = snippetParser.Parse(this.CurrentLine.Text));
				}
				catch {
					return null;
				}
			}
		}
		
		INode currentMemberAST;
		public INode CurrentMemberAST
		{
			get
			{
				if (resolver == null)
					return null;
				if (currentMemberAST != null)
					return currentMemberAST;
				try {
					resolver.Initialize(ParserService.GetParseInformation(Editor.FileName), Editor.Caret.Line, Editor.Caret.Column);
					return (currentMemberAST = resolver.ParseCurrentMember(Editor.Document.Text));
				}
				catch {
					return null;
				}
			}
		}

		SnippetParser GetSnippetParser(ITextEditor editor)
		{
			var lang = GetEditorLanguage(editor);
			if (lang != null) {
				return new SnippetParser(lang.Value);
			}
			return null;
		}
		
		NRefactoryResolver GetResolver(ITextEditor editor)
		{
			if (editor == null || editor.Language == null)
				return null;
			try
			{
				return new NRefactoryResolver(editor.Language.Properties);
			}
			catch(NotSupportedException)
			{
				return null;
			}
		}
		
		SupportedLanguage? GetEditorLanguage(ITextEditor editor)
		{
			if (editor == null || editor.Language == null)
				return null;
			if (editor.Language.Properties == LanguageProperties.CSharp)
				return SupportedLanguage.CSharp;
			if (editor.Language.Properties == LanguageProperties.VBNet)
				return SupportedLanguage.VBNet;
			return null;
		}
	}
}
