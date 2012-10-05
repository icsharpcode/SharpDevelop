// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding
{
	/// <summary>
	/// Semantic highlighting for C#.
	/// </summary>
	public class CSharpSemanticHighlighter : DepthFirstAstVisitor, IHighlighter
	{
		readonly IDocument document;
		readonly HighlightingColor defaultTextColor;
		readonly HighlightingColor referenceTypeColor;
		readonly HighlightingColor valueTypeColor;
		readonly HighlightingColor methodCallColor;
		readonly HighlightingColor fieldAccessColor;
		readonly HighlightingColor valueKeywordColor;
		readonly HighlightingColor parameterModifierColor;
		readonly HighlightingColor inactiveCodeColor;
		
		List<IDocumentLine> invalidLines = new List<IDocumentLine>();
		List<CachedLine> cachedLines = new List<CachedLine>();
		bool hasCrashed;
		bool forceParseOnNextRefresh;
		
		int lineNumber;
		HighlightedLine line;
		CSharpAstResolver resolver;
		
		bool isInAccessor;
		
		#region Constructor + Dispose
		public CSharpSemanticHighlighter(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			
			var highlighting = HighlightingManager.Instance.GetDefinition("C#");
			this.defaultTextColor = highlighting.GetNamedColor("DefaultTextAndBackground");
			this.referenceTypeColor = highlighting.GetNamedColor("ReferenceTypes");
			this.valueTypeColor = highlighting.GetNamedColor("ValueTypes");
			this.methodCallColor = highlighting.GetNamedColor("MethodCall");
			this.fieldAccessColor = highlighting.GetNamedColor("FieldAccess");
			this.valueKeywordColor = highlighting.GetNamedColor("NullOrValueKeywords");
			this.parameterModifierColor = highlighting.GetNamedColor("ParameterModifiers");
			this.inactiveCodeColor = highlighting.GetNamedColor("InactiveCode");
			
			SD.ParserService.ParseInformationUpdated += ParserService_ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished += ParserService_LoadSolutionProjectsThreadEnded;
		}
		
		public void Dispose()
		{
			SD.ParserService.ParseInformationUpdated -= ParserService_ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished -= ParserService_LoadSolutionProjectsThreadEnded;
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

		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			cachedLines.Clear();
			invalidLines.Clear();
			forceParseOnNextRefresh = true;
			OnHighlightingStateChanged(0, document.LineCount);
		}
		
		void ParserService_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, document.FileName) && invalidLines.Count > 0) {
				cachedLines.Clear();
				foreach (IDocumentLine line in invalidLines) {
					if (!line.IsDeleted) {
						OnHighlightingStateChanged(line.LineNumber, line.LineNumber);
					}
				}
				invalidLines.Clear();
			}
		}
		#endregion
		
		#region IHighlighter implementation
		public event HighlightingStateChangedEventHandler HighlightingStateChanged;
		
		protected virtual void OnHighlightingStateChanged(int fromLineNumber, int toLineNumber)
		{
			if (HighlightingStateChanged != null) {
				HighlightingStateChanged(this, fromLineNumber, toLineNumber);
			}
		}
		
		IDocument IHighlighter.Document {
			get { return document; }
		}
		
		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber)
		{
			return null;
		}
		
		void IHighlighter.UpdateHighlightingState(int lineNumber)
		{
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			IDocumentLine documentLine = document.GetLineByNumber(lineNumber);
			if (hasCrashed) {
				// don't highlight anymore after we've crashed
				return new HighlightedLine(document, documentLine);
			}
			ITextSourceVersion newVersion = document.Version;
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
			
			CSharpFullParseInformation parseInfo;
			if (forceParseOnNextRefresh) {
				forceParseOnNextRefresh = false;
				parseInfo = SD.ParserService.Parse(FileName.Create(document.FileName), document) as CSharpFullParseInformation;
			} else {
				parseInfo = SD.ParserService.GetCachedParseInformation(FileName.Create(document.FileName), document.Version) as CSharpFullParseInformation;
			}
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
			
			var compilation = SD.ParserService.GetCompilationForFile(parseInfo.FileName);
			this.resolver = parseInfo.GetResolver(compilation);
			
			HighlightedLine line = new HighlightedLine(document, documentLine);
			this.line = line;
			this.lineNumber = lineNumber;
			if (Debugger.IsAttached) {
				parseInfo.SyntaxTree.AcceptVisitor(this);
			} else {
				try {
					parseInfo.SyntaxTree.AcceptVisitor(this);
				} catch (Exception ex) {
					hasCrashed = true;
					throw new ApplicationException("Error highlighting line " + lineNumber, ex);
				}
			}
			this.line = null;
			this.resolver = null;
			//Debug.WriteLine("Semantic highlighting for line {0} - added {1} sections", lineNumber, line.Sections.Count);
			if (document.Version != null) {
				cachedLines.Add(new CachedLine(line, document.Version));
			}
			return line;
		}
		
		public HighlightingColor DefaultTextColor {
			get {
				return defaultTextColor;
			}
		}
		
		public void BeginHighlighting()
		{
			
		}
		
		public void EndHighlighting()
		{
			// use this event to remove cached lines which are no longer visible
//			var visibleDocumentLines = new HashSet<IDocumentLine>(syntaxHighlighter.GetVisibleDocumentLines());
//			cachedLines.RemoveAll(c => !visibleDocumentLines.Contains(c.DocumentLine));
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			return null;
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
			if (start.Line <= lineNumber && end.Line >= lineNumber) {
				int lineStartOffset = line.DocumentLine.Offset;
				int startOffset = lineStartOffset + (start.Line == lineNumber ? start.Column - 1 : 0);
				int endOffset = lineStartOffset + (end.Line == lineNumber ? end.Column - 1 : line.DocumentLine.Length);
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
			simpleType.TypeArguments.AcceptVisitor(this);
		}
		
		public override void VisitMemberType(MemberType memberType)
		{
			// Ensure we visit/colorize the children in the correct order.
			// This is required so that the resulting HighlightedSections are sorted correctly.
			memberType.Target.AcceptVisitor(this);
			Colorize(memberType.MemberNameToken, GetColor(resolver.Resolve(memberType)));
			memberType.TypeArguments.AcceptVisitor(this);
		}
		
		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			Identifier ident = identifierExpression.GetChildByRole(Roles.Identifier);
			if (isInAccessor && identifierExpression.Identifier == "value") {
				Colorize(ident, valueKeywordColor);
			} else {
				ResolveResult rr = resolver.Resolve(identifierExpression);
				Colorize(ident, GetColor(rr));
			}
			
			identifierExpression.TypeArguments.AcceptVisitor(this);
		}
		
		public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			memberReferenceExpression.Target.AcceptVisitor(this);
			
			ResolveResult rr = resolver.Resolve(memberReferenceExpression);
			Colorize(memberReferenceExpression.MemberNameToken, GetColor(rr));
			
			memberReferenceExpression.TypeArguments.AcceptVisitor(this);
		}
		
		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			Expression target = invocationExpression.Target;
			if (target is IdentifierExpression || target is MemberReferenceExpression || target is PointerReferenceExpression) {
				var invocationRR = resolver.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (invocationRR != null && IsInactiveConditionalMethod(invocationRR.Member)) {
					// mark the whole invocation expression as inactive code
					Colorize(invocationExpression, inactiveCodeColor);
					return;
				}
				
				// apply color to target's target
				target.GetChildByRole(Roles.TargetExpression).AcceptVisitor(this);
				
				// highlight the method call
				var identifier = target.GetChildByRole(Roles.Identifier);
				if (invocationRR != null && !invocationRR.IsDelegateInvocation) {
					Colorize(identifier, methodCallColor);
				} else {
					ResolveResult targetRR = resolver.Resolve(target);
					Colorize(identifier, GetColor(targetRR));
				}
				
				target.GetChildrenByRole(Roles.TypeArgument).AcceptVisitor(this);
			} else {
				target.AcceptVisitor(this);
			}
			// Visit arguments and comments within the arguments:
			for (AstNode child = target.NextSibling; child != null; child = child.NextSibling) {
				child.AcceptVisitor(this);
			}
		}
		
		bool IsInactiveConditionalMethod(IParameterizedMember member)
		{
			if (member.EntityType != EntityType.Method || member.ReturnType.Kind != TypeKind.Void)
				return false;
			while (member.IsOverride)
				member = (IParameterizedMember)InheritanceHelper.GetBaseMember(member);
			return IsInactiveConditional(member.Attributes);
		}
		
		bool IsInactiveConditional(IList<IAttribute> attributes)
		{
			bool hasConditionalAttribute = false;
			foreach (var attr in attributes) {
				if (attr.AttributeType.Name == "ConditionalAttribute" && attr.AttributeType.Namespace == "System.Diagnostics" && attr.PositionalArguments.Count == 1) {
					string symbol = attr.PositionalArguments[0].ConstantValue as string;
					if (symbol != null) {
						hasConditionalAttribute = true;
						var cu = this.resolver.RootNode as SyntaxTree;
						if (cu != null) {
							if (cu.ConditionalSymbols.Contains(symbol))
								return false; // conditional is active
						}
					}
				}
			}
			return hasConditionalAttribute;
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
			for (AstNode child = methodDeclaration.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= lineNumber && child.EndLocation.Line >= lineNumber) {
					if (child.Role == Roles.Identifier) {
						// child == methodDeclaration.NameToken
						Colorize(child, methodCallColor);
					} else {
						child.AcceptVisitor(this);
					}
				}
			}
		}
		
		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			HandleConstructorOrDestructor(constructorDeclaration);
		}
		
		public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			HandleConstructorOrDestructor(destructorDeclaration);
		}
		
		void HandleConstructorOrDestructor(AstNode constructorDeclaration)
		{
			for (AstNode child = constructorDeclaration.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= lineNumber && child.EndLocation.Line >= lineNumber) {
					if (child.Role == Roles.Identifier) {
						// child == constructorDeclaration.NameToken
						var currentTypeDef = resolver.GetResolverStateBefore(constructorDeclaration).CurrentTypeDefinition;
						if (currentTypeDef != null && ((Identifier)child).Name == currentTypeDef.Name) {
							if (currentTypeDef.IsReferenceType == true)
								Colorize(child, referenceTypeColor);
							else if (currentTypeDef.IsReferenceType == false)
								Colorize(child, valueTypeColor);
						}
					} else {
						child.AcceptVisitor(this);
					}
				}
			}
		}
		
		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			// Type declarations often contain #if directives, so we must make sure
			// to also visit the comments.
			for (AstNode child = typeDeclaration.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= lineNumber && child.EndLocation.Line >= lineNumber) {
					if (child.Role == Roles.Identifier) {
						// child == typeDeclaration.NameToken
						if (typeDeclaration.ClassType == ClassType.Enum || typeDeclaration.ClassType == ClassType.Struct)
							Colorize(typeDeclaration.NameToken, valueTypeColor);
						else
							Colorize(typeDeclaration.NameToken, referenceTypeColor);
					} else {
						child.AcceptVisitor(this);
					}
				}
			}
		}
		
		public override void VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration)
		{
			typeParameterDeclaration.Attributes.AcceptVisitor(this);
			
			if (typeParameterDeclaration.Variance == VarianceModifier.Contravariant)
				Colorize(typeParameterDeclaration.VarianceToken, parameterModifierColor);
			
			bool isValueType = false;
			if (typeParameterDeclaration.Parent != null) {
				foreach (var constraint in typeParameterDeclaration.Parent.GetChildrenByRole(Roles.Constraint)) {
					if (constraint.TypeParameter.Identifier == typeParameterDeclaration.Name) {
						isValueType = constraint.BaseTypes.OfType<PrimitiveType>().Any(p => p.Keyword == "struct");
					}
				}
			}
			Colorize(typeParameterDeclaration.NameToken, isValueType ? valueTypeColor : referenceTypeColor);
		}
		
		public override void VisitConstraint(Constraint constraint)
		{
			if (constraint.Parent != null && constraint.Parent.GetChildrenByRole(Roles.TypeParameter).Any(tp => tp.Name == constraint.TypeParameter.Identifier)) {
				bool isValueType = constraint.BaseTypes.OfType<PrimitiveType>().Any(p => p.Keyword == "struct");
				Colorize(constraint.GetChildByRole(Roles.Identifier), isValueType ? valueTypeColor : referenceTypeColor);
			}
			
			constraint.BaseTypes.AcceptVisitor(this);
		}
		
		public override void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			if (variableInitializer.Parent is FieldDeclaration) {
				Colorize(variableInitializer.NameToken, fieldAccessColor);
			}
			variableInitializer.Initializer.AcceptVisitor(this);
		}
		
		public override void VisitComment(Comment comment)
		{
			if (comment.CommentType == CommentType.InactiveCode) {
				Colorize(comment, inactiveCodeColor);
			}
		}
		
		public override void VisitAttribute(ICSharpCode.NRefactory.CSharp.Attribute attribute)
		{
			ITypeDefinition attrDef = resolver.Resolve(attribute.Type).Type.GetDefinition();
			if (attrDef != null && IsInactiveConditional(attrDef.Attributes)) {
				Colorize(attribute, inactiveCodeColor);
				return;
			}
			VisitChildren(attribute);
		}
		#endregion
	}
}
