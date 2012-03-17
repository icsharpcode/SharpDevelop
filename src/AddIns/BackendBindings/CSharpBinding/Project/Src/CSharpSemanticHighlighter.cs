// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
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
	public class CSharpSemanticHighlighter : DepthFirstAstVisitor, IHighlighter, IDisposable
	{
		readonly ITextEditor textEditor;
		readonly ISyntaxHighlighter syntaxHighlighter;
		readonly HighlightingColor referenceTypeColor;
		readonly HighlightingColor valueTypeColor;
		readonly HighlightingColor methodCallColor;
		readonly HighlightingColor fieldAccessColor;
		readonly HighlightingColor valueKeywordColor;
		readonly HighlightingColor parameterModifierColor;
		
		List<IDocumentLine> invalidLines = new List<IDocumentLine>();
		List<CachedLine> cachedLines = new List<CachedLine>();
		
		int lineNumber;
		HighlightedLine line;
		CSharpAstResolver resolver;
		
		bool isInAccessor;
		
		#region Constructor + Dispose
		public CSharpSemanticHighlighter(ITextEditor textEditor, ISyntaxHighlighter syntaxHighlighter)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			if (syntaxHighlighter == null)
				throw new ArgumentNullException("syntaxHighlighter");
			this.textEditor = textEditor;
			this.syntaxHighlighter = syntaxHighlighter;
			
			IHighlightingDefinition highlightingDefinition = syntaxHighlighter.HighlightingDefinition;
			this.referenceTypeColor = highlightingDefinition.GetNamedColor("ReferenceTypes");
			this.valueTypeColor = highlightingDefinition.GetNamedColor("ValueTypes");
			this.methodCallColor = highlightingDefinition.GetNamedColor("MethodCall");
			this.fieldAccessColor = highlightingDefinition.GetNamedColor("FieldAccess");
			this.valueKeywordColor = highlightingDefinition.GetNamedColor("NullOrValueKeywords");
			this.parameterModifierColor = highlightingDefinition.GetNamedColor("ParameterModifiers");
			
			ParserService.ParseInformationUpdated += ParserService_ParseInformationUpdated;
			ParserService.LoadSolutionProjectsThreadEnded += ParserService_LoadSolutionProjectsThreadEnded;
			syntaxHighlighter.VisibleDocumentLinesChanged += syntaxHighlighter_VisibleDocumentLinesChanged;
		}
		
		public void Dispose()
		{
			ParserService.ParseInformationUpdated -= ParserService_ParseInformationUpdated;
			ParserService.LoadSolutionProjectsThreadEnded -= ParserService_LoadSolutionProjectsThreadEnded;
			syntaxHighlighter.VisibleDocumentLinesChanged -= syntaxHighlighter_VisibleDocumentLinesChanged;
		}
		#endregion
		
		#region Caching
		// If a line gets edited and we need to display it while no parse information is ready for the
		// changed file, the line would flicker (semantic highlightings disappear temporarily).
		// We avoid this issue by storing the semantic highlightings and updating them on document changes
		// (using anchor movement)
		class CachedLine
		{
			public readonly HighlightedLine HighlightedLine;
			public ITextSourceVersion OldVersion;
			
			/// <summary>
			/// Gets whether the cache line is valid (no document changes since it was created).
			/// This field gets set to false when Update() is called.
			/// </summary>
			public bool IsValid;
			
			public IDocumentLine DocumentLine { get { return HighlightedLine.DocumentLine; } }
			
			public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion fileVersion)
			{
				if (highlightedLine == null)
					throw new ArgumentNullException("highlightedLine");
				if (fileVersion == null)
					throw new ArgumentNullException("fileVersion");
				
				this.HighlightedLine = highlightedLine;
				this.OldVersion = fileVersion;
				this.IsValid = true;
			}
			
			public void Update(ITextSourceVersion newVersion)
			{
				// Apply document changes to all highlighting sections:
				foreach (TextChangeEventArgs change in OldVersion.GetChangesTo(newVersion)) {
					foreach (HighlightedSection section in HighlightedLine.Sections) {
						int endOffset = section.Offset + section.Length;
						section.Offset = change.GetNewOffset(section.Offset);
						endOffset = change.GetNewOffset(endOffset);
						section.Length = endOffset - section.Offset;
					}
				}
				// The resulting sections might have become invalid:
				// - zero-length if section was deleted,
				// - a section might have moved outside the range of this document line (newline inserted in document = line split up)
				// So we will remove all highlighting sections which have become invalid.
				int lineStart = HighlightedLine.DocumentLine.Offset;
				int lineEnd = lineStart + HighlightedLine.DocumentLine.Length;
				for (int i = 0; i < HighlightedLine.Sections.Count; i++) {
					HighlightedSection section = HighlightedLine.Sections[i];
					if (section.Offset < lineStart || section.Offset + section.Length > lineEnd || section.Length <= 0)
						HighlightedLine.Sections.RemoveAt(i--);
				}
				
				this.OldVersion = newVersion;
				this.IsValid = false;
			}
		}
		#endregion
		
		#region Event Handlers
		void syntaxHighlighter_VisibleDocumentLinesChanged(object sender, EventArgs e)
		{
			// use this event to remove cached lines which are no longer visible
			var visibleDocumentLines = new HashSet<IDocumentLine>(syntaxHighlighter.GetVisibleDocumentLines());
			cachedLines.RemoveAll(c => !visibleDocumentLines.Contains(c.DocumentLine));
		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			cachedLines.Clear();
			invalidLines.Clear();
			syntaxHighlighter.InvalidateAll();
		}
		
		void ParserService_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (e.FileName == textEditor.FileName && invalidLines.Count > 0) {
				cachedLines.Clear();
				foreach (IDocumentLine line in invalidLines) {
					if (!line.IsDeleted) {
						syntaxHighlighter.InvalidateLine(line);
					}
				}
				invalidLines.Clear();
			}
		}
		#endregion
		
		#region IHighlighter implementation
		IDocument IHighlighter.Document {
			get { return textEditor.Document; }
		}
		
		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber)
		{
			return null;
		}
		
		event HighlightingStateChangedEventHandler IHighlighter.HighlightingStateChanged {
			add { }
			remove { }
		}
		
		void IHighlighter.UpdateHighlightingState(int lineNumber)
		{
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			IDocumentLine documentLine = textEditor.Document.GetLineByNumber(lineNumber);
			ITextSourceVersion newVersion = textEditor.Document.Version;
			CachedLine cachedLine = null;
			for (int i = 0; i < cachedLines.Count; i++) {
				if (cachedLines[i].DocumentLine == documentLine) {
					if (newVersion == null || !newVersion.BelongsToSameDocumentAs(cachedLines[i].OldVersion)) {
						// cannot list changes from old to new: we can't update the cache, so we'll remove it
						cachedLines.RemoveAt(i);
					} else {
						cachedLine = cachedLines[i];
					}
					break;
				}
			}
			
			if (cachedLine != null && cachedLine.IsValid && newVersion.CompareAge(cachedLine.OldVersion) == 0) {
				// the file hasn't changed since the cache was created, so just reuse the old highlighted line
				return cachedLine.HighlightedLine;
			}
			
			var parseInfo = ParserService.GetCachedParseInformation(textEditor.FileName, textEditor.Document.Version) as CSharpFullParseInformation;
			if (parseInfo == null) {
				if (!invalidLines.Contains(documentLine))
					invalidLines.Add(documentLine);
				Debug.WriteLine("Semantic highlighting for line {0} - marking as invalid", lineNumber);
				
				if (cachedLine != null) {
					// If there's a cached version, adjust it to the latest document changes and return it.
					// This avoids flickering when changing a line that contains semantic highlighting.
					cachedLine.Update(newVersion);
					return cachedLine.HighlightedLine;
				} else {
					return null;
				}
			}
			
			var compilation = ParserService.GetCompilationForFile(parseInfo.FileName);
			this.resolver = parseInfo.GetResolver(compilation);
			
			HighlightedLine line = new HighlightedLine(textEditor.Document, documentLine);
			this.line = line;
			this.lineNumber = lineNumber;
			parseInfo.CompilationUnit.AcceptVisitor(this);
			this.line = null;
			this.resolver = null;
			Debug.WriteLine("Semantic highlighting for line {0} - added {1} sections", lineNumber, line.Sections.Count);
			if (textEditor.Document.Version != null) {
				cachedLines.Add(new CachedLine(line, textEditor.Document.Version));
			}
			return line;
		}
		#endregion
		
		#region Colorize
		HighlightingColor GetColor(ResolveResult rr)
		{
			if (rr is TypeResolveResult) {
				if (rr.Type.IsReferenceType == false)
					return valueTypeColor;
				else
					return referenceTypeColor;
			}
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
				if (line.Sections.Count > 0) {
					HighlightedSection prevSection = line.Sections.Last();
					if (startOffset < prevSection.Offset + prevSection.Length)
						throw new InvalidOperationException("Cannot create unordered highlighting section");
				}
				line.Sections.Add(new HighlightedSection {
				                  	Offset = startOffset,
				                  	Length = endOffset - startOffset,
				                  	Color = color
				                  });
			}
		}
		#endregion
		
		#region AST Traversal
		protected override void VisitChildren(AstNode node)
		{
			for (var child = node.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= lineNumber && child.EndLocation.Line >= lineNumber)
					child.AcceptVisitor(this);
			}
		}
		
		public override void VisitSimpleType(SimpleType simpleType)
		{
			Colorize(simpleType.IdentifierToken, GetColor(resolver.Resolve(simpleType)));
			foreach (AstNode node in simpleType.TypeArguments)
				node.AcceptVisitor(this);
		}
		
		public override void VisitMemberType(MemberType memberType)
		{
			// Ensure we visit/colorize the children in the correct order.
			// This is required so that the resulting HighlightedSections are sorted correctly.
			memberType.Target.AcceptVisitor(this);
			Colorize(memberType.MemberNameToken, GetColor(resolver.Resolve(memberType)));
			foreach (AstNode node in memberType.TypeArguments)
				node.AcceptVisitor(this);
		}
		
		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			Identifier ident = identifierExpression.GetChildByRole(IdentifierExpression.Roles.Identifier);
			if (isInAccessor && identifierExpression.Identifier == "value") {
				Colorize(ident, valueKeywordColor);
			} else {
				ResolveResult rr = resolver.Resolve(identifierExpression);
				Colorize(ident, GetColor(rr));
			}
			
			foreach (AstNode node in identifierExpression.TypeArguments)
				node.AcceptVisitor(this);
		}
		
		public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			memberReferenceExpression.Target.AcceptVisitor(this);
			
			ResolveResult rr = resolver.Resolve(memberReferenceExpression);
			Colorize(memberReferenceExpression.MemberNameToken, GetColor(rr));
			
			foreach (AstNode node in memberReferenceExpression.TypeArguments)
				node.AcceptVisitor(this);
		}
		
		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			Expression target = invocationExpression.Target;
			target.AcceptVisitor(this);
			
			var rr = resolver.Resolve(invocationExpression) as CSharpInvocationResolveResult;
			if (rr != null && !rr.IsDelegateInvocation) {
				if (target is IdentifierExpression || target is MemberReferenceExpression || target is PointerReferenceExpression) {
					Colorize(target.GetChildByRole(AstNode.Roles.Identifier),  methodCallColor);
				}
			}
			
			foreach (AstNode node in invocationExpression.Arguments)
				node.AcceptVisitor(this);
		}
		
		public override void VisitAccessor(Accessor accessor)
		{
			isInAccessor = true;
			try {
				base.VisitAccessor(accessor);
			} finally {
				isInAccessor = false;
			}
		}
		
		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
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
		}
		
		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			if (typeDeclaration.ClassType == ClassType.Enum || typeDeclaration.ClassType == ClassType.Struct)
				Colorize(typeDeclaration.NameToken, valueTypeColor);
			else
				Colorize(typeDeclaration.NameToken, referenceTypeColor);
			
			foreach (var node in typeDeclaration.TypeParameters)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.BaseTypes)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.Constraints)
				node.AcceptVisitor(this);
			foreach (var node in typeDeclaration.Members)
				node.AcceptVisitor(this);
		}
		
		public override void VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration)
		{
			if (typeParameterDeclaration.Variance == VarianceModifier.Contravariant)
				Colorize(typeParameterDeclaration.VarianceToken, parameterModifierColor);
			
			bool isValueType = false;
			if (typeParameterDeclaration.Parent != null) {
				foreach (var constraint in typeParameterDeclaration.Parent.GetChildrenByRole(AstNode.Roles.Constraint)) {
					if (constraint.TypeParameter.Identifier == typeParameterDeclaration.Name) {
						isValueType = constraint.BaseTypes.OfType<PrimitiveType>().Any(p => p.Keyword == "struct");
					}
				}
			}
			Colorize(typeParameterDeclaration.NameToken, isValueType ? valueTypeColor : referenceTypeColor);
		}
		
		public override void VisitConstraint(Constraint constraint)
		{
			if (constraint.Parent != null && constraint.Parent.GetChildrenByRole(AstNode.Roles.TypeParameter).Any(tp => tp.Name == constraint.TypeParameter.Identifier)) {
				bool isValueType = constraint.BaseTypes.OfType<PrimitiveType>().Any(p => p.Keyword == "struct");
				Colorize(constraint.GetChildByRole(AstNode.Roles.Identifier), isValueType ? valueTypeColor : referenceTypeColor);
			}
			foreach (var baseType in constraint.BaseTypes)
				baseType.AcceptVisitor(this);
		}
		
		public override void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			if (variableInitializer.Parent is FieldDeclaration) {
				Colorize(variableInitializer.NameToken, fieldAccessColor);
			}
			variableInitializer.Initializer.AcceptVisitor(this);
		}
		#endregion
	}
}
