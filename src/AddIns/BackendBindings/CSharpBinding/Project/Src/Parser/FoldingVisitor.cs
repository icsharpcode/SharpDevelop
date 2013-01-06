// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;

namespace CSharpBinding.Parser
{
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
			if (end.Line <= start.Line)
				return null;
			NewFolding folding = new NewFolding(GetOffset(start), GetOffset(end));
			folding.IsDefinition = isDefinition;
			foldings.Add(folding);
			return folding;
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
				AddFolding (typeDeclaration.LBraceToken.GetPrevNode ().EndLocation, typeDeclaration.RBraceToken.EndLocation);
			base.VisitTypeDeclaration (typeDeclaration);
		}
		
		public override void VisitMethodDeclaration (MethodDeclaration methodDeclaration)
		{
			if (!methodDeclaration.Body.IsNull) {
				AddFolding (methodDeclaration.Body.LBraceToken.GetPrevNode ().EndLocation,
				            methodDeclaration.Body.RBraceToken.EndLocation, true);
			}
			base.VisitMethodDeclaration (methodDeclaration);
		}
		
		public override void VisitConstructorDeclaration (ConstructorDeclaration constructorDeclaration)
		{
			if (!constructorDeclaration.Body.IsNull)
				AddFolding (constructorDeclaration.Body.LBraceToken.GetPrevNode ().EndLocation,
				            constructorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitConstructorDeclaration (constructorDeclaration);
		}
		
		public override void VisitDestructorDeclaration (DestructorDeclaration destructorDeclaration)
		{
			if (!destructorDeclaration.Body.IsNull)
				AddFolding (destructorDeclaration.Body.LBraceToken.GetPrevNode ().EndLocation,
				            destructorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitDestructorDeclaration (destructorDeclaration);
		}
		
		public override void VisitOperatorDeclaration (OperatorDeclaration operatorDeclaration)
		{
			if (!operatorDeclaration.Body.IsNull)
				AddFolding (operatorDeclaration.Body.LBraceToken.GetPrevNode ().EndLocation,
				            operatorDeclaration.Body.RBraceToken.EndLocation, true);
			base.VisitOperatorDeclaration (operatorDeclaration);
		}
		
		public override void VisitPropertyDeclaration (PropertyDeclaration propertyDeclaration)
		{
			if (!propertyDeclaration.LBraceToken.IsNull)
				AddFolding (propertyDeclaration.LBraceToken.GetPrevNode ().EndLocation,
				            propertyDeclaration.RBraceToken.EndLocation, true);
			base.VisitPropertyDeclaration (propertyDeclaration);
		}
		
		public override void VisitIndexerDeclaration (IndexerDeclaration indexerDeclaration)
		{
			if (!indexerDeclaration.LBraceToken.IsNull)
				AddFolding (indexerDeclaration.LBraceToken.GetPrevNode ().EndLocation,
				            indexerDeclaration.RBraceToken.EndLocation, true);
			base.VisitIndexerDeclaration (indexerDeclaration);
		}
		
		public override void VisitCustomEventDeclaration (CustomEventDeclaration eventDeclaration)
		{
			if (!eventDeclaration.LBraceToken.IsNull)
				AddFolding (eventDeclaration.LBraceToken.GetPrevNode ().EndLocation,
				            eventDeclaration.RBraceToken.EndLocation, true);
			base.VisitCustomEventDeclaration (eventDeclaration);
		}
		#endregion
		
		#region Statements
		public override void VisitSwitchStatement (SwitchStatement switchStatement)
		{
			if (!switchStatement.RBraceToken.IsNull)
				AddFolding (switchStatement.LBraceToken.GetPrevNode ().EndLocation,
				            switchStatement.RBraceToken.EndLocation);
			base.VisitSwitchStatement (switchStatement);
		}
		
		public override void VisitBlockStatement (BlockStatement blockStatement)
		{
			if (!(blockStatement.Parent is EntityDeclaration) && blockStatement.EndLocation.Line - blockStatement.StartLocation.Line > 2) {
				AddFolding (blockStatement.GetPrevNode ().EndLocation, blockStatement.EndLocation);
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
