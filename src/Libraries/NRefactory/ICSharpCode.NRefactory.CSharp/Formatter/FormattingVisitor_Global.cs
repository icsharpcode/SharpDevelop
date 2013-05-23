//
// AstFormattingVisitor_Global.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp
{
	partial class FormattingVisitor : DepthFirstAstVisitor
	{
		int GetGlobalNewLinesFor(AstNode child)
		{
			int newLines = 1;
			var nextSibling = child.GetNextSibling(NoWhitespacePredicate);
			if ((child is UsingDeclaration || child is UsingAliasDeclaration) && !(nextSibling is UsingDeclaration || nextSibling is UsingAliasDeclaration)) {
				newLines += policy.BlankLinesAfterUsings;
			} else if ((child is TypeDeclaration) && (nextSibling is TypeDeclaration)) {
				newLines += policy.BlankLinesBetweenTypes;
			}

			return newLines;
		}

		public override void VisitSyntaxTree(SyntaxTree unit)
		{
			bool first = true;
			VisitChildrenToFormat (unit, child => {
				if (first && (child is UsingDeclaration || child is UsingAliasDeclaration)) {
					EnsureBlankLinesBefore (child, policy.BlankLinesBeforeUsings);
					first = false;
				}
				if (NoWhitespacePredicate(child))
					FixIndentation(child);
				child.AcceptVisitor(this);
				if (NoWhitespacePredicate(child))
					EnsureNewLinesAfter(child, GetGlobalNewLinesFor (child));
			});
		}

		public override void VisitUsingDeclaration(UsingDeclaration usingDeclaration)
		{
			ForceSpacesAfter(usingDeclaration.UsingToken, true);
			FixSemicolon(usingDeclaration.SemicolonToken);
		}

		public override void VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration)
		{
			ForceSpacesAfter(usingDeclaration.UsingToken, true);
			FixSemicolon(usingDeclaration.SemicolonToken);
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			FixOpenBrace(policy.NamespaceBraceStyle, namespaceDeclaration.LBraceToken);
			if (policy.IndentNamespaceBody)
				curIndent.Push(IndentType.Block);

			bool first = true;
			VisitChildrenToFormat (namespaceDeclaration, child => {
				if (child.Role == Roles.LBrace) {
					var next = child.GetNextSibling (NoWhitespacePredicate);
					var blankLines = 1;
					if (next is UsingDeclaration || next is UsingAliasDeclaration) {
						blankLines += policy.BlankLinesBeforeUsings;
					} else {
						blankLines += policy.BlankLinesBeforeFirstDeclaration;
					}
					EnsureNewLinesAfter(child, blankLines);
					return;
				}
				if (child.Role != NamespaceDeclaration.MemberRole)
					return;
				if (first && (child is UsingDeclaration || child is UsingAliasDeclaration)) {
					// TODO: policy.BlankLinesBeforeUsings
					first = false;
				}
				if (NoWhitespacePredicate(child))
					FixIndentationForceNewLine(child);
				child.AcceptVisitor(this);
				if (NoWhitespacePredicate(child))
					EnsureNewLinesAfter(child, GetGlobalNewLinesFor (child));
			});

			if (policy.IndentNamespaceBody)
				curIndent.Pop ();

			FixClosingBrace(policy.NamespaceBraceStyle, namespaceDeclaration.RBraceToken);
		}

		void FixAttributes(EntityDeclaration entity)
		{
			if (entity.Attributes.Count > 0) {
				AstNode n = null;;
				foreach (var attr in entity.Attributes.Skip (1)) {
					FixIndentation(attr);
					n = attr;
				}
				if (n != null)
					FixIndentation(n.GetNextNode (NoWhitespacePredicate));
			}
		}
	
		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			FixAttributes(typeDeclaration);
			BraceStyle braceStyle;
			bool indentBody = false;
			switch (typeDeclaration.ClassType) {
				case ClassType.Class:
					braceStyle = policy.ClassBraceStyle;
					indentBody = policy.IndentClassBody;
					break;
				case ClassType.Struct:
					braceStyle = policy.StructBraceStyle;
					indentBody = policy.IndentStructBody;
					break;
				case ClassType.Interface:
					braceStyle = policy.InterfaceBraceStyle;
					indentBody = policy.IndentInterfaceBody;
					break;
				case ClassType.Enum:
					braceStyle = policy.EnumBraceStyle;
					indentBody = policy.IndentEnumBody;
					break;
				default:
					throw new InvalidOperationException("unsupported class type : " + typeDeclaration.ClassType);
			}

			FixOpenBrace(braceStyle, typeDeclaration.LBraceToken);

			if (indentBody)
				curIndent.Push(IndentType.Block);

			VisitChildrenToFormat (typeDeclaration, child => {
				if (child.Role != Roles.TypeMemberRole)
					return;
				if (NoWhitespacePredicate(child))
					FixIndentationForceNewLine(child);
				child.AcceptVisitor (this);
				if (NoWhitespacePredicate(child))
					EnsureNewLinesAfter(child, GetTypeLevelNewLinesFor (child));
			});

			if (indentBody)
				curIndent.Pop();

			FixClosingBrace(braceStyle, typeDeclaration.RBraceToken);
		}

		int GetTypeLevelNewLinesFor(AstNode child)
		{
			var blankLines = 1;
			var nextSibling = child.GetNextSibling(NoWhitespacePredicate);

			if (child is EventDeclaration) {
				if (nextSibling is EventDeclaration) {
					blankLines += policy.BlankLinesBetweenEventFields;
					return blankLines;
				}
			}

			if (child is FieldDeclaration || child is FixedFieldDeclaration) {
				if (nextSibling is FieldDeclaration || nextSibling is FixedFieldDeclaration) {
					blankLines += policy.BlankLinesBetweenFields;
					return blankLines;
				}
			}
			
			if (child is TypeDeclaration) {
				if (nextSibling is TypeDeclaration) {
					blankLines += policy.BlankLinesBetweenTypes;
					return blankLines;
				}
			}

			if (nextSibling.Role == Roles.TypeMemberRole)
				blankLines += policy.BlankLinesBetweenMembers;

			return blankLines;
		}

		public override void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
		{
			ForceSpacesBefore(delegateDeclaration.LParToken, policy.SpaceBeforeDelegateDeclarationParentheses);
			if (delegateDeclaration.Parameters.Any()) {
				ForceSpacesAfter(delegateDeclaration.LParToken, policy.SpaceWithinDelegateDeclarationParentheses);
				ForceSpacesBefore(delegateDeclaration.RParToken, policy.SpaceWithinDelegateDeclarationParentheses);
			} else {
				ForceSpacesAfter(delegateDeclaration.LParToken, policy.SpaceBetweenEmptyDelegateDeclarationParentheses);
				ForceSpacesBefore(delegateDeclaration.RParToken, policy.SpaceBetweenEmptyDelegateDeclarationParentheses);
			}
			FormatCommas(delegateDeclaration, policy.SpaceBeforeDelegateDeclarationParameterComma, policy.SpaceAfterDelegateDeclarationParameterComma);

			base.VisitDelegateDeclaration(delegateDeclaration);
		}
		
		public override void VisitComment(Comment comment)
		{
			if (comment.StartsLine && !HadErrors && (!policy.KeepCommentsAtFirstColumn || comment.StartLocation.Column > 1))
				FixIndentation(comment);
		}
	}
}