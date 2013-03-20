// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Contains information about code around the caret in the editor - useful for implementing Context actions.
	/// Do not keep your own references to EditorContext.
	/// It serves as one-time cache and does not get updated when editor text changes.
	/// </summary>
	public class EditorContext
	{
		public ITextEditor Editor { get; private set; }
		int CaretLine { get; set; }
		int CaretColumn { get; set; }
		
		/// <summary>
		/// The expression at editor caret. Language independent.
		/// </summary>
		public ExpressionResult CurrentExpression { get; private set; }
		/// <summary>
		/// The resolved symbol at editor caret. Language independent.
		/// </summary>
		public ResolveResult CurrentSymbol { get; private set; }
		
		/// <summary>
		/// ParseInformation for current file. Language independent.
		/// </summary>
		public ParseInformation CurrentParseInformation { get; private set; }
		
		public IProjectContent ProjectContent {
			get {
				if (CurrentParseInformation != null)
					return CurrentParseInformation.CompilationUnit.ProjectContent;
				else
					return null;
			}
		}
		
		/// <summary>
		/// The editor line containing the caret.
		/// </summary>
		public IDocumentLine CurrentLine { get; private set; }
		/// <summary>
		/// Parsed AST of the current editor line. Only available for C# and VB.
		/// </summary>
		public INode CurrentLineAST { get; private set; }
		/// <summary>
		/// Parsed AST of the member containing the editor caret. Only available for C# and VB.
		/// </summary>
		public INode CurrentMemberAST { get; private set; }
		/// <summary>
		/// Parsed AST of the element at editor caret. Only available for C# and VB.
		/// </summary>
		INode CurrentElement { get; set; }
		
		NRefactoryResolver Resolver { get; set; }
		
		/// <summary>
		/// Caches values shared by Context actions. Used in <see cref="GetCached"></see>
		/// </summary>
		Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
		
		/// <summary>
		/// Fully initializes the EditorContext.
		/// </summary>
		public EditorContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.CaretLine = editor.Caret.Line;
			this.CaretColumn = editor.Caret.Column;
			if (CaretColumn > 1 && editor.Document.GetText(editor.Document.PositionToOffset(CaretLine, CaretColumn - 1), 1) == ";") {
				// If caret is just after ';', pretend that caret is before ';'
				// (works well e.g. for this.Foo();(*caret*) - we want to get "this.Foo()")
				// This is equivalent to pretending that ; don't exist, and actually it's not such a bad idea.
				CaretColumn -= 1;
			}
			
			this.CurrentExpression = GetExpressionAtCaret(editor);
			this.CurrentSymbol = ResolveExpression(CurrentExpression, editor, CaretLine, CaretColumn);
			this.CurrentParseInformation = ParserService.GetExistingParseInformation(editor.FileName);
			
			this.CurrentLine = editor.Document.GetLine(CaretLine);
			this.CurrentLineAST = GetCurrentLineAst(this.CurrentLine, editor);
			
			this.CurrentMemberAST = GetCurrentMemberAST(editor);
			
			this.CurrentElement = FindInnermostNode(this.CurrentMemberAST, new Location(CaretColumn, CaretLine));
//			DebugLog();
		}
		
		public ResolveResult ResolveExpression(Expression expression)
		{
			if (expression.EndLocation.IsEmpty)
				return null;
			ExpressionResult expr = GetExpressionAt(this.Editor, expression.EndLocation.Line, expression.EndLocation.Column);
			return ResolveExpression(expr, this.Editor, expression.EndLocation.Line, expression.EndLocation.Column);
		}
		
		/// <summary>
		/// Do not call from your Context actions - used by SharpDevelop.
		/// Sets contents of editor context to null to prevent memory leaks. Used in case users implementing IContextActionProvider
		/// keep long-lived references to EditorContext even when warned not to do so.
		/// </summary>
		public void Clear()
		{
			this.Editor = null;
			this.CurrentElement = null;
			this.CurrentLineAST = null;
			this.CurrentMemberAST = null;
			this.CurrentParseInformation = null;
			this.CurrentSymbol = null;
			this.Resolver = null;
			this.cachedValues.Clear();
		}
		
		/// <summary>
		/// Gets cached value shared by context actions. Initializes a new value if not present.
		/// </summary>
		public T GetCached<T>() where T : IContextActionCache, new()
		{
			Type t = typeof(T);
			if (cachedValues.ContainsKey(t)) {
				return (T)cachedValues[t];
			} else {
				T cached = new T();
				cached.Initialize(this);
				cachedValues[t] = cached;
				return cached;
			}
		}
		
		/// <summary>
		/// Checks if the caret is exactly at AST node of given type (e.g. "if" of IfElseStatement).
		/// If yes, returns it. If not, returns null.
		/// </summary>
		public TNode GetCurrentElement<TNode>() where TNode : class, INode
		{
			if (this.CurrentElement is TNode)
				return (TNode)this.CurrentElement;
			return null;
		}
		
		/// <summary>
		/// Checks if caret is within AST node of given type (e.g. anywhere inside IfElseStatement).
		/// If yes, returns it. If not, returns null.
		/// </summary>
		public TNode GetContainingElement<TNode>() where TNode : class, INode
		{
			var node = this.CurrentElement;
			while(node != null)
			{
				if (node is TNode)
					return (TNode)node;
				node = node.Parent;
			}
			return null;
		}
		
		void DebugLog()
		{
			ICSharpCode.Core.LoggingService.Debug(string.Format(
				@"
	
	Context actions :
	ExprAtCaret: {0}
	----------------------
	SymbolAtCaret: {1}
	----------------------
	CurrentLineAST: {2}
	----------------------
	CurrentASTNode: [{3}] {4}
	----------------------
	CurrentMemberAST: {5}
	----------------------",
				CurrentExpression, CurrentSymbol, CurrentLineAST,
				CurrentElement == null ? "" : CurrentElement.GetType().ToString(),
				CurrentElement == null ? "" : CurrentElement.ToString().TakeStartEllipsis(400),
				CurrentMemberAST == null ? "" : CurrentMemberAST.ToString().TakeStartEllipsis(400)));
		}
		
		static INode FindInnermostNode(INode node, Location position)
		{
			if (node == null)
				return null;
			var findInnermostVisitor = new FindInnermostNodeByRangeVisitor(position);
			node.AcceptVisitor(findInnermostVisitor, null);
			return findInnermostVisitor.InnermostNode;
		}
		
		public static INode FindInnermostNodeContainingSelection(INode node, Location start, Location end)
		{
			if (node == null)
				return null;
			var findInnermostVisitor = new FindInnermostNodeByRangeVisitor(start, end);
			node.AcceptVisitor(findInnermostVisitor, null);
			return findInnermostVisitor.InnermostNode;
		}
		
		class FindInnermostNodeByRangeVisitor : NodeTrackingAstVisitor
		{
			public Location RangeStart { get; private set; }
			public Location RangeEnd { get; private set; }
			public INode InnermostNode { get; private set; }
			
			public FindInnermostNodeByRangeVisitor(Location caretPosition) : this(caretPosition, caretPosition)
			{
			}
			
			public FindInnermostNodeByRangeVisitor(Location selectionStart, Location selectionEnd)
			{
				this.RangeStart = selectionStart;
				this.RangeEnd = selectionEnd;
			}
			
			protected override void BeginVisit(INode node)
			{
				if (node.StartLocation <= RangeStart && node.EndLocation >= RangeEnd) {
					// the node visited last will be the innermost
					this.InnermostNode = node;
				}
				base.BeginVisit(node);
			}
		}

		ResolveResult ResolveExpression(ExpressionResult expression, ITextEditor editor, int caretLine, int caretColumn)
		{
			return ParserService.Resolve(expression, caretLine, caretColumn, editor.FileName, editor.Document.Text);
		}

		ExpressionResult GetExpressionAtCaret(ITextEditor editor)
		{
			return GetExpressionAt(editor, this.CaretLine, this.CaretColumn);
		}
		
		ExpressionResult GetExpressionAt(ITextEditor editor, int caretLine, int caretColumn)
		{
			ExpressionResult expr = ParserService.FindFullExpression(caretLine, caretColumn, editor.Document, editor.FileName);
			// if no expression, look one character back (works better with method calls, e.g. Foo()(*caret*))
			if (string.IsNullOrWhiteSpace(expr.Expression) && caretColumn > 1)
				expr = ParserService.FindFullExpression(caretLine, caretColumn - 1, editor.Document, editor.FileName);
			return expr;
		}
		
		
		INode GetCurrentLineAst(IDocumentLine currentLine, ITextEditor editor)
		{
			if (currentLine == null)
				return null;
			var snippetParser = GetSnippetParser(editor);
			if (snippetParser == null)
				return null;
			return snippetParser.Parse(currentLine.Text);
		}
		
		SnippetParser GetSnippetParser(ITextEditor editor)
		{
			var lang = GetEditorLanguage(editor);
			if (lang != null) {
				return new SnippetParser(lang.Value);
			}
			return null;
		}
		
		public static SupportedLanguage? GetEditorLanguage(ITextEditor editor)
		{
			if (editor == null || editor.Language == null)
				return null;
			if (editor.Language.Properties == LanguageProperties.CSharp)
				return SupportedLanguage.CSharp;
			if (editor.Language.Properties == LanguageProperties.VBNet)
				return SupportedLanguage.VBNet;
			return null;
		}
		
		
		INode GetCurrentMemberAST(ITextEditor editor)
		{
			var resolver = GetInitializedNRefactoryResolver(editor, this.CaretLine, this.CaretColumn);
			if (resolver == null)
				return null;
			return resolver.ParseCurrentMember(editor.Document.Text);
		}
		
		NRefactoryResolver GetInitializedNRefactoryResolver(ITextEditor editor, int caretLine, int caretColumn)
		{
			if (editor == null || editor.Language == null)
				return null;
			try
			{
				var resolver = new NRefactoryResolver(editor.Language.Properties);
				resolver.Initialize(ParserService.GetParseInformation(editor.FileName), caretLine, caretColumn);
				return resolver;
			}
			catch(NotSupportedException)
			{
				return null;
			}
		}
	}
}
