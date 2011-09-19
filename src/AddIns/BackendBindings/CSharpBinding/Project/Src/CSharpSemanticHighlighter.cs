// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding
{
	/// <summary>
	/// Semantic highlighting for C#.
	/// </summary>
	public class CSharpSemanticHighlighter : DepthFirstAstVisitor<object, object>, IHighlighter, IResolveVisitorNavigator, IDisposable
	{
		readonly ITextEditor textEditor;
		readonly ISyntaxHighlighter syntaxHighlighter;
		readonly HighlightingColor typeReferenceColor;
		readonly HighlightingColor methodCallColor;
		readonly HighlightingColor fieldAccessColor;
		readonly HighlightingColor valueKeywordColor;
		
		HashSet<IDocumentLine> invalidLines = new HashSet<IDocumentLine>();
		
		int lineNumber;
		HighlightedLine line;
		ResolveVisitor resolveVisitor;
		
		bool isInAccessor;
		
		public CSharpSemanticHighlighter(ITextEditor textEditor, ISyntaxHighlighter syntaxHighlighter)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			if (syntaxHighlighter == null)
				throw new ArgumentNullException("syntaxHighlighter");
			this.textEditor = textEditor;
			this.syntaxHighlighter = syntaxHighlighter;
			
			IHighlightingDefinition highlightingDefinition = syntaxHighlighter.HighlightingDefinition;
			this.typeReferenceColor = highlightingDefinition.GetNamedColor("TypeReferences");
			this.methodCallColor = highlightingDefinition.GetNamedColor("MethodCall");
			this.fieldAccessColor = highlightingDefinition.GetNamedColor("FieldAccess");
			this.valueKeywordColor = highlightingDefinition.GetNamedColor("NullOrValueKeywords");
			
			ParserService.ParserUpdateStepFinished += ParserService_ParserUpdateStepFinished;
			ParserService.LoadSolutionProjectsThreadEnded += ParserService_LoadSolutionProjectsThreadEnded;
		}
		
		public void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= ParserService_ParserUpdateStepFinished;
			ParserService.LoadSolutionProjectsThreadEnded -= ParserService_LoadSolutionProjectsThreadEnded;
		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			syntaxHighlighter.InvalidateAll();
		}
		
		void ParserService_ParserUpdateStepFinished(object sender, ParserUpdateStepEventArgs e)
		{
			if (e.FileName == textEditor.FileName && invalidLines.Count > 0) {
				foreach (IDocumentLine line in invalidLines) {
					if (!line.IsDeleted) {
						syntaxHighlighter.InvalidateLine(line);
					}
				}
				invalidLines.Clear();
			}
		}
		
		IDocument IHighlighter.Document {
			get { return textEditor.Document; }
		}
		
		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber)
		{
			return null;
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			ParseInformation parseInfo = ParserService.GetCachedParseInformation(textEditor.FileName, textEditor.Document.Version);
			if (parseInfo == null) {
				invalidLines.Add(textEditor.Document.GetLineByNumber(lineNumber));
				Debug.WriteLine("Semantic highlighting for line {0} - marking as invalid", lineNumber);
				return null;
			}
			CSharpParsedFile parsedFile = parseInfo.ParsedFile as CSharpParsedFile;
			CompilationUnit cu = parseInfo.Annotation<CompilationUnit>();
			if (cu == null || parsedFile == null) {
				Debug.WriteLine("Semantic highlighting for line {0} - not a C# file?", lineNumber);
				return null;
			}
			
			using (var ctx = ParserService.GetTypeResolveContext(parseInfo.ProjectContent).Synchronize()) {
				CSharpResolver resolver = new CSharpResolver(ctx);
				resolveVisitor = new ResolveVisitor(resolver, parsedFile, this);
				
				resolveVisitor.Scan(cu);
				
				HighlightedLine line = new HighlightedLine(textEditor.Document, textEditor.Document.GetLineByNumber(lineNumber));
				this.line = line;
				this.lineNumber = lineNumber;
				cu.AcceptVisitor(this);
				this.line = null;
				this.resolveVisitor = null;
				Debug.WriteLine("Semantic highlighting for line {0} - added {1} sections", lineNumber, line.Sections.Count);
				return line;
			}
		}
		
		HighlightingColor GetColor(ResolveResult rr)
		{
			if (rr is TypeResolveResult)
				return typeReferenceColor;
			MemberResolveResult mrr = rr as MemberResolveResult;
			if (mrr != null) {
				if (mrr.Member is IField)
					return fieldAccessColor;
			}
			return null;
		}
		
		void Colorize(AstNode node, HighlightingColor color)
		{
			if (node.IsNull || color == null)
				return;
			Colorize(node.StartLocation, node.EndLocation, color);
		}
		
		void Colorize(TextLocation start, TextLocation end, HighlightingColor color)
		{
			if (color == null)
				return;
			if (start.Line == lineNumber && end.Line == lineNumber) {
				int lineStartOffset = line.DocumentLine.Offset;
				int startOffset = lineStartOffset + start.Column - 1;
				int endOffset = lineStartOffset + end.Column - 1;
				line.Sections.Add(new HighlightedSection {
				                  	Offset = startOffset,
				                  	Length = endOffset - startOffset,
				                  	Color = color
				                  });
			}
		}
		
		ResolveVisitorNavigationMode IResolveVisitorNavigator.Scan(AstNode node)
		{
			if (node.StartLocation.Line <= lineNumber && node.EndLocation.Line >= lineNumber) {
				if (node is SimpleType || node is MemberType
				    || node is IdentifierExpression || node is MemberReferenceExpression
				    || node is InvocationExpression)
				{
					return ResolveVisitorNavigationMode.Resolve;
				} else {
					return ResolveVisitorNavigationMode.Scan;
				}
			} else {
				return ResolveVisitorNavigationMode.Skip;
			}
		}
		
		void IResolveVisitorNavigator.Resolved(AstNode node, ResolveResult result)
		{
		}
		
		void IResolveVisitorNavigator.ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType)
		{
		}
		
		protected override object VisitChildren(AstNode node, object data)
		{
			for (var child = node.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= lineNumber && child.EndLocation.Line >= lineNumber)
					child.AcceptVisitor(this);
			}
			return null;
		}
		
		public override object VisitSimpleType(SimpleType simpleType, object data)
		{
			if (resolveVisitor.GetResolveResult(simpleType) is TypeResolveResult)
				Colorize(simpleType.IdentifierToken, typeReferenceColor);
			foreach (AstNode node in simpleType.TypeArguments)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitMemberType(MemberType memberType, object data)
		{
			// Ensure we visit/colorize the children in the correct order.
			// This is required so that the resulting HighlightedSections are sorted correctly.
			memberType.Target.AcceptVisitor(this);
			if (resolveVisitor.GetResolveResult(memberType) is TypeResolveResult)
				Colorize(memberType.MemberNameToken, typeReferenceColor);
			foreach (AstNode node in memberType.TypeArguments)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			Identifier ident = identifierExpression.GetChildByRole(IdentifierExpression.Roles.Identifier);
			if (isInAccessor && identifierExpression.Identifier == "value") {
				Colorize(ident, valueKeywordColor);
			} else {
				ResolveResult rr = resolveVisitor.GetResolveResult(identifierExpression);
				Colorize(ident, GetColor(rr));
			}
			
			foreach (AstNode node in identifierExpression.TypeArguments)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			memberReferenceExpression.Target.AcceptVisitor(this);
			
			ResolveResult rr = resolveVisitor.GetResolveResult(memberReferenceExpression);
			Colorize(memberReferenceExpression.MemberNameToken, GetColor(rr));
			
			foreach (AstNode node in memberReferenceExpression.TypeArguments)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Expression target = invocationExpression.Target;
			target.AcceptVisitor(this);
			
			var rr = resolveVisitor.GetResolveResult(invocationExpression) as CSharpInvocationResolveResult;
			if (rr != null && !rr.IsDelegateInvocation) {
				if (target is IdentifierExpression || target is MemberReferenceExpression || target is PointerReferenceExpression) {
					Colorize(target.GetChildByRole(AstNode.Roles.Identifier),  methodCallColor);
				}
			}
			
			foreach (AstNode node in invocationExpression.Arguments)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitAccessor(Accessor accessor, object data)
		{
			isInAccessor = true;
			try {
				return base.VisitAccessor(accessor, data);
			} finally {
				isInAccessor = false;
			}
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			methodDeclaration.ReturnType.AcceptVisitor(this);
			Colorize(methodDeclaration.NameToken, methodCallColor);
			foreach (var node in methodDeclaration.TypeParameters)
				node.AcceptVisitor(this);
			foreach (var node in methodDeclaration.Parameters)
				node.AcceptVisitor(this);
			foreach (var node in methodDeclaration.Constraints)
				node.AcceptVisitor(this);
			methodDeclaration.Body.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			Colorize(typeDeclaration.NameToken, typeReferenceColor);
			foreach (var node in typeDeclaration.TypeParameters)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.BaseTypes)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.Constraints)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.Members)
				node.AcceptVisitor(this);
			return null;
		}
		
		public override object VisitVariableInitializer(VariableInitializer variableInitializer, object data)
		{
			if (variableInitializer.Parent is FieldDeclaration) {
				Colorize(variableInitializer.NameToken, fieldAccessColor);
			}
			variableInitializer.Initializer.AcceptVisitor(this);
			return null;
		}
	}
}
