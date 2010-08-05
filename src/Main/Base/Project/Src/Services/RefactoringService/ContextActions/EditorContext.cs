// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
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
	/// Helper class for <see cref="IContextActionsProvider.GetAvailableActions"></see>.
	/// Never keep long-lived references to this class
	/// - the AST serves as one-time cache and does not get updated when editor text changes.
	/// </summary>
	public class EditorContext
	{
		public ITextEditor Editor { get; private set; }
		int CaretLine { get; set; }
		int CaretColumn { get; set; }
		
		/// <summary>
		/// Language independent.
		/// </summary>
		public ExpressionResult CurrentExpression { get; private set; }
		/// <summary>
		/// Language independent.
		/// </summary>
		public ResolveResult CurrentSymbol { get; private set; }
		
		public IDocumentLine CurrentLine { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentLineAST { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentMemberAST { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentElement { get; private set; }
		
		NRefactoryResolver Resolver { get; set; }
		
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
				// This is equivalent to pretending that ; don't exist, and actually is not such a bad idea.
				CaretColumn -= 1;
			}
			
			this.CurrentExpression = GetExpressionAtCaret(editor);
			this.CurrentSymbol = ResolveExpression(editor);
			
			this.CurrentLine = editor.Document.GetLine(CaretLine);
			this.CurrentLineAST = GetCurrentLineAst(this.CurrentLine, editor);
			
			this.CurrentMemberAST = GetCurrentMemberAST(editor);
			
			this.CurrentElement = FindInnermostNodeAtLocation(this.CurrentMemberAST, new Location(CaretColumn, CaretLine));
			
			//DebugLog();
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
	AstNodeAtCaret: {3}
	----------------------
	CurrentMemberAST: {4}
	----------------------",
				CurrentExpression, CurrentSymbol, CurrentLineAST,
				CurrentElement == null ? "" : CurrentElement.ToString().TakeStartEllipsis(400),
				CurrentMemberAST == null ? "" : CurrentMemberAST.ToString().TakeStartEllipsis(400)));
		}
		
		public TNode GetCurrentElement<TNode>() where TNode : class, INode
		{
			if (this.CurrentElement is TNode)
				return (TNode)this.CurrentElement;
			return null;
		}
		
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
		
//		public TNode GetInnerElement<TNode>() where TNode : class, INode
//		{
//			var findChildVisitor = new FindOutermostNodeVisitor<TNode>();
//			this.CurrentElement.AcceptVisitor(findChildVisitor, null);
//			return findChildVisitor.FoundNode;
//		}
		
		INode FindInnermostNodeAtLocation(INode memberDecl, Location position)
		{
			if (memberDecl == null)
				return null;
			if (memberDecl is MethodDeclaration) {
				return FindInnermostNodeInBlock(((MethodDeclaration)memberDecl).Body, position);
			} else if (memberDecl is PropertyDeclaration) {
				var propertyDecl = (PropertyDeclaration)memberDecl;
				if (propertyDecl.HasGetRegion && position >= propertyDecl.GetRegion.StartLocation && position <= propertyDecl.GetRegion.EndLocation) {
					return FindInnermostNodeInBlock(propertyDecl.GetRegion.Block, position);
				}
				if (propertyDecl.HasSetRegion && position >= propertyDecl.SetRegion.StartLocation && position <= propertyDecl.SetRegion.EndLocation) {
					return FindInnermostNodeInBlock(propertyDecl.SetRegion.Block, position);
				}
			}
			return null;
		}
		
		INode FindInnermostNodeInBlock(BlockStatement node, Location position)
		{
			if (node == null)
				return null;
			var findInnermostVisitor = new FindInnermostNodeVisitor(position);
			node.AcceptVisitor(findInnermostVisitor, null);
			return findInnermostVisitor.InnermostNode;
		}
		
		class FindInnermostNodeVisitor : NodeTrackingAstVisitor
		{
			public Location CaretLocation { get; private set; }
			public INode InnermostNode { get; private set; }
			
			public FindInnermostNodeVisitor(Location caretLocation)
			{
				this.CaretLocation = caretLocation;
			}
			
			protected override void BeginVisit(INode node)
			{
				if (node.StartLocation <= CaretLocation && node.EndLocation >= CaretLocation) {
					// the node visited last will be the innermost
					this.InnermostNode = node;
				}
				base.BeginVisit(node);
			}
		}
		
//		class FindOutermostNodeVisitor<TNode> : NodeTrackingAstVisitor where TNode : class, INode
//		{
//			public TNode FoundNode { get; private set; }
//
//			protected override void BeginVisit(INode node)
//			{
//				if (node is TNode && FoundNode == null) {
//					FoundNode = (TNode)node;
//				}
//				base.BeginVisit(node);
//			}
//		}

		ResolveResult ResolveExpression(ITextEditor editor)
		{
			return ParserService.Resolve(this.CurrentExpression, CaretLine, CaretColumn, editor.FileName, editor.Document.Text);
		}

		ExpressionResult GetExpressionAtCaret(ITextEditor editor)
		{
			ExpressionResult expr = ParserService.FindFullExpression(CaretLine, CaretColumn, editor.Document, editor.FileName);
			// if no expression, look one character back (works better with method calls - Foo()(*caret*))
			if (string.IsNullOrWhiteSpace(expr.Expression) && CaretColumn > 1)
				expr = ParserService.FindFullExpression(CaretLine, CaretColumn - 1, editor.Document, editor.FileName);
			return expr;
		}
		
		
		INode GetCurrentLineAst(IDocumentLine currentLine, ITextEditor editor)
		{
			if (currentLine == null)
				return null;
			var snippetParser = GetSnippetParser(editor);
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
		
		
		INode GetCurrentMemberAST(ITextEditor editor)
		{
			try {
				var resolver = GetNRefactoryResolver(editor);
				resolver.Initialize(ParserService.GetParseInformation(editor.FileName), CaretLine, CaretColumn);
				return resolver.ParseCurrentMember(editor.Document.Text);
			}
			catch {
				return null;
			}
		}
		
		NRefactoryResolver GetNRefactoryResolver(ITextEditor editor)
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
	}
}
