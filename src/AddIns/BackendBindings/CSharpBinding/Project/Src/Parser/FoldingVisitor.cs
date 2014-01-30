// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;

namespace CSharpBinding.Parser
{
	// TODO: move this class + NewFolding to NRefactory
	class FoldingVisitor : DepthFirstAstVisitor
	{
		internal List<NewFolding> foldings = new List<NewFolding> ();
		internal IDocument document;
		
		int GetOffset(TextLocation location)
		{
			return document.GetOffset(location);
		}
		
		NewFolding AddFolding(TextLocation start, TextLocation end, bool isDefinition = false)
		{
			if (end.Line <= start.Line || start.IsEmpty || end.IsEmpty)
				return null;
			NewFolding folding = new NewFolding(GetOffset(start), GetOffset(end));
			folding.IsDefinition = isDefinition;
			foldings.Add(folding);
			return folding;
		}
		
		TextLocation GetEndOfPrev(AstNode node)
		{
			do {
				node = node.GetPrevNode();
			} while (node.NodeType == NodeType.Whitespace);
			return node.EndLocation;
		}
		
		#region usings
		void AddUsings (AstNode parent)
		{
			var firstChild = parent.Children.FirstOrDefault (child => child is UsingDeclaration || child is UsingAliasDeclaration);
			var node = firstChild;
			while (node != null) {
				var next = node.GetNextNode ();
				if (next is UsingDeclaration || next is UsingAliasDeclaration) {
					node = next;
				} else {
					break;
				}
			}
			if (firstChild != node) {
				NewFolding folding = AddFolding(firstChild.StartLocation, node.EndLocation);
				if (folding != null) {
					folding.Name = "using...";
					folding.DefaultClosed = true;
				}
			}
		}
		public override void VisitSyntaxTree (SyntaxTree unit)
		{
			AddUsings (unit);
			base.VisitSyntaxTree (unit);
		}

		public override void VisitNamespaceDeclaration (NamespaceDeclaration namespaceDeclaration)
		{
			AddUsings (namespaceDeclaration);
			if (!namespaceDeclaration.RBraceToken.IsNull)
				AddFolding (namespaceDeclaration.LBraceToken.GetPrevNode ().EndLocation, namespaceDeclaration.RBraceToken.EndLocation);
			base.VisitNamespaceDeclaration (namespaceDeclaration);
		}
		#endregion
		
		#region Entity Declarations
		public override void VisitTypeDeclaration (TypeDeclaration typeDeclaration)
		{
			if (!typeDeclaration.RBraceToken.IsNull)
				AddFolding (GetEndOfPrev(typeDeclaration.LBraceToken), typeDeclaration.RBraceToken.EndLocation);
			base.VisitTypeDeclaration (typeDeclaration);
		}
		
		public override void VisitMethodDeclaration (MethodDeclaration methodDeclaration)
		{
			if (!methodDeclaration.Body.IsNull) {
				AddFolding (GetEndOfPrev(methodDeclaration.Body.LBraceToken),
				            methodDeclaration.Body.RBraceToken.EndLocation, true);
			}
			base.VisitMethodDeclaration (methodDeclaration);
		}
		
		public override void VisitConstructorDeclaration (ConstructorDeclaration constructorDeclaration)
		{
			if (!constructorDeclaration.Body.IsNull)
				AddFolding (GetEndOfPrev(constructorDeclaration.Body.LBraceToken),
				            constructorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitConstructorDeclaration (constructorDeclaration);
		}
		
		public override void VisitDestructorDeclaration (DestructorDeclaration destructorDeclaration)
		{
			if (!destructorDeclaration.Body.IsNull)
				AddFolding (GetEndOfPrev(destructorDeclaration.Body.LBraceToken),
				            destructorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitDestructorDeclaration (destructorDeclaration);
		}
		
		public override void VisitOperatorDeclaration (OperatorDeclaration operatorDeclaration)
		{
			if (!operatorDeclaration.Body.IsNull)
				AddFolding (GetEndOfPrev(operatorDeclaration.Body.LBraceToken),
				            operatorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitOperatorDeclaration (operatorDeclaration);
		}
		
		public override void VisitPropertyDeclaration (PropertyDeclaration propertyDeclaration)
		{
			if (!propertyDeclaration.LBraceToken.IsNull)
				AddFolding (GetEndOfPrev(propertyDeclaration.LBraceToken),
				            propertyDeclaration.RBraceToken.EndLocation, true);
			base.VisitPropertyDeclaration (propertyDeclaration);
		}
		
		public override void VisitIndexerDeclaration (IndexerDeclaration indexerDeclaration)
		{
			if (!indexerDeclaration.LBraceToken.IsNull)
				AddFolding (GetEndOfPrev(indexerDeclaration.LBraceToken),
				            indexerDeclaration.RBraceToken.EndLocation, true);
			base.VisitIndexerDeclaration (indexerDeclaration);
		}
		
		public override void VisitCustomEventDeclaration (CustomEventDeclaration eventDeclaration)
		{
			if (!eventDeclaration.LBraceToken.IsNull)
				AddFolding (GetEndOfPrev(eventDeclaration.LBraceToken),
				            eventDeclaration.RBraceToken.EndLocation, true);
			base.VisitCustomEventDeclaration (eventDeclaration);
		}
		#endregion
		
		#region Statements
		public override void VisitSwitchStatement (SwitchStatement switchStatement)
		{
			if (!switchStatement.RBraceToken.IsNull)
				AddFolding (GetEndOfPrev(switchStatement.LBraceToken),
				            switchStatement.RBraceToken.EndLocation);
			base.VisitSwitchStatement (switchStatement);
		}
		
		public override void VisitBlockStatement (BlockStatement blockStatement)
		{
			if (!(blockStatement.Parent is EntityDeclaration) && blockStatement.EndLocation.Line - blockStatement.StartLocation.Line > 2) {
				AddFolding (GetEndOfPrev(blockStatement), blockStatement.EndLocation);
			}

			base.VisitBlockStatement (blockStatement);
		}
		#endregion
		
		#region Preprocessor directives
		Stack<NewFolding> regions = new Stack<NewFolding>();
		
		public override void VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
		{
			switch (preProcessorDirective.Type) {
				case PreProcessorDirectiveType.Region:
					NewFolding folding = new NewFolding();
					folding.DefaultClosed = true;
					folding.Name = preProcessorDirective.Argument;
					folding.StartOffset = GetOffset(preProcessorDirective.StartLocation);
					regions.Push(folding);
					break;
				case PreProcessorDirectiveType.Endregion:
					if (regions.Count > 0) {
						folding = regions.Pop();
						folding.EndOffset = GetOffset(preProcessorDirective.EndLocation);
						foldings.Add(folding);
					}
					break;
			}
		}
		#endregion
		
		#region Comments
		public override void VisitComment(Comment comment)
		{
			if (comment.CommentType == CommentType.InactiveCode)
				return; // don't fold the inactive code comment; instead fold the preprocessor directives
			if (AreTwoSinglelineCommentsInConsecutiveLines(comment.PrevSibling as Comment, comment))
				return; // already handled by previous comment
			Comment lastComment = comment;
			Comment nextComment;
			while (true) {
				nextComment = lastComment.NextSibling as Comment;
				if (!AreTwoSinglelineCommentsInConsecutiveLines(lastComment, nextComment))
					break;
				lastComment = nextComment;
			}
			if (lastComment.EndLocation.Line - comment.StartLocation.Line > 2) {
				var folding = AddFolding(comment.StartLocation, lastComment.EndLocation);
				if (folding != null) {
					switch (comment.CommentType) {
						case CommentType.SingleLine:
							folding.Name = "// ...";
							break;
						case CommentType.MultiLine:
							folding.Name = "/* ... */";
							break;
						case CommentType.Documentation:
							folding.Name = "/// ...";
							break;
						case CommentType.MultiLineDocumentation:
							folding.Name = "/** ... */";
							break;
					}
				}
			}
		}
		
		bool AreTwoSinglelineCommentsInConsecutiveLines(Comment comment1, Comment comment2)
		{
			if (comment1 == null || comment2 == null)
				return false;
			return comment1.CommentType == comment2.CommentType
				&& comment1.StartLocation.Line == comment1.EndLocation.Line
				&& comment1.EndLocation.Line + 1 == comment2.StartLocation.Line
				&& comment1.StartLocation.Column == comment2.StartLocation.Column
				&& comment2.StartLocation.Line == comment2.EndLocation.Line;
		}
		#endregion
	}
}
