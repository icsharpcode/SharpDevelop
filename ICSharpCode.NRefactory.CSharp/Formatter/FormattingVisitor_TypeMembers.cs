//
// AstFormattingVisitor_TypeMembers.cs
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
		public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			FixAttributesAndDocComment(propertyDeclaration);
			bool oneLine = false;
			bool fixClosingBrace = false;
			switch (policy.PropertyFormatting) {
				case PropertyFormatting.AllowOneLine:
					bool isSimple = IsSimpleAccessor(propertyDeclaration.Getter) && IsSimpleAccessor(propertyDeclaration.Setter);
					int accessorLine = propertyDeclaration.RBraceToken.StartLocation.Line;
					if (!propertyDeclaration.Getter.IsNull && propertyDeclaration.Setter.IsNull) {
						accessorLine = propertyDeclaration.Getter.StartLocation.Line;
					} else if (propertyDeclaration.Getter.IsNull && !propertyDeclaration.Setter.IsNull) {
						accessorLine = propertyDeclaration.Setter.StartLocation.Line;
					} else {
						var acc = propertyDeclaration.Getter.StartLocation < propertyDeclaration.Setter.StartLocation ?
							propertyDeclaration.Getter : propertyDeclaration.Setter;
						accessorLine = acc.StartLocation.Line;
					}
					if (!isSimple || propertyDeclaration.LBraceToken.StartLocation.Line != accessorLine) {
						fixClosingBrace = true;
						FixOpenBrace(policy.PropertyBraceStyle, propertyDeclaration.LBraceToken);
					} else {
						ForceSpacesBefore(propertyDeclaration.Getter, true);
						ForceSpacesBefore(propertyDeclaration.Setter, true);
						ForceSpacesBeforeRemoveNewLines(propertyDeclaration.RBraceToken, true);
						oneLine = true;
					}
					break;
					case PropertyFormatting.ForceNewLine:
						fixClosingBrace = true;
						FixOpenBrace(policy.PropertyBraceStyle, propertyDeclaration.LBraceToken);
						break;
					case PropertyFormatting.ForceOneLine:
						isSimple = IsSimpleAccessor(propertyDeclaration.Getter) && IsSimpleAccessor(propertyDeclaration.Setter);
						if (isSimple) {
							int offset = this.document.GetOffset(propertyDeclaration.LBraceToken.StartLocation);

							int start = SearchWhitespaceStart(offset);
							int end = SearchWhitespaceEnd(offset);
							AddChange(start, offset - start, " ");
							AddChange(offset + 1, end - offset - 2, " ");

							offset = this.document.GetOffset(propertyDeclaration.RBraceToken.StartLocation);
							start = SearchWhitespaceStart(offset);
							AddChange(start, offset - start, " ");
							oneLine = true;

						} else {
							fixClosingBrace = true;
							FixOpenBrace(policy.PropertyBraceStyle, propertyDeclaration.LBraceToken);
						}
					break;
			}
			if (policy.IndentPropertyBody)
				curIndent.Push(IndentType.Block);

			///System.Console.WriteLine ("one line: " + oneLine);
			if (!propertyDeclaration.Getter.IsNull) {
				if (!oneLine) {
					if (!IsLineIsEmptyUpToEol(propertyDeclaration.Getter.StartLocation)) {
						int offset = this.document.GetOffset(propertyDeclaration.Getter.StartLocation);
						int start = SearchWhitespaceStart(offset);
						string indentString = this.curIndent.IndentString;
						AddChange(start, offset - start, this.options.EolMarker + indentString);
					} else {
						FixIndentation(propertyDeclaration.Getter);
					}
				} else {
					int offset = this.document.GetOffset(propertyDeclaration.Getter.StartLocation);
					int start = SearchWhitespaceStart(offset);
					AddChange(start, offset - start, " ");

					ForceSpacesBefore(propertyDeclaration.Getter.Body.LBraceToken, true);
					ForceSpacesBefore(propertyDeclaration.Getter.Body.RBraceToken, true);
				}
				if (!propertyDeclaration.Getter.Body.IsNull) {
					if (!policy.AllowPropertyGetBlockInline || propertyDeclaration.Getter.Body.LBraceToken.StartLocation.Line != propertyDeclaration.Getter.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.PropertyGetBraceStyle, propertyDeclaration.Getter.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(propertyDeclaration.Getter.Body, policy.IndentBlocks);
						FixClosingBrace(policy.PropertyGetBraceStyle, propertyDeclaration.Getter.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(propertyDeclaration.Getter.Body, policy.IndentBlocks);

					}
				}
			}

			if (!propertyDeclaration.Setter.IsNull) {
				if (!oneLine) {
					if (!IsLineIsEmptyUpToEol(propertyDeclaration.Setter.StartLocation)) {
						int offset = document.GetOffset(propertyDeclaration.Setter.StartLocation);
						int start = SearchWhitespaceStart(offset);
						string indentString = curIndent.IndentString;
						AddChange(start, offset - start, options.EolMarker + indentString);
					} else {
						FixIndentation(propertyDeclaration.Setter);
					}
				} else {
					int offset = this.document.GetOffset(propertyDeclaration.Setter.StartLocation);
					int start = SearchWhitespaceStart(offset);
					AddChange(start, offset - start, " ");

					ForceSpacesBefore(propertyDeclaration.Setter.Body.LBraceToken, true);
					ForceSpacesBefore(propertyDeclaration.Setter.Body.RBraceToken, true);
				}
				if (!propertyDeclaration.Setter.Body.IsNull) {
					if (!policy.AllowPropertySetBlockInline || propertyDeclaration.Setter.Body.LBraceToken.StartLocation.Line != propertyDeclaration.Setter.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.PropertySetBraceStyle, propertyDeclaration.Setter.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(propertyDeclaration.Setter.Body, policy.IndentBlocks);
						FixClosingBrace(policy.PropertySetBraceStyle, propertyDeclaration.Setter.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(propertyDeclaration.Setter.Body, policy.IndentBlocks);
					}
				}
			}

			if (policy.IndentPropertyBody) {
				curIndent.Pop ();
			}
			if (fixClosingBrace)
				FixClosingBrace(policy.PropertyBraceStyle, propertyDeclaration.RBraceToken);

		}

		public override void VisitAccessor(Accessor accessor)
		{
			FixAttributesAndDocComment(accessor);

			base.VisitAccessor(accessor);
		}


		public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			FixAttributesAndDocComment(indexerDeclaration);

			ForceSpacesBefore(indexerDeclaration.LBracketToken, policy.SpaceBeforeIndexerDeclarationBracket);
			ForceSpacesAfter(indexerDeclaration.LBracketToken, policy.SpaceWithinIndexerDeclarationBracket);

			FormatArguments(indexerDeclaration);

			FixOpenBrace(policy.PropertyBraceStyle, indexerDeclaration.LBraceToken);
			if (policy.IndentPropertyBody)
				curIndent.Push(IndentType.Block);

			if (!indexerDeclaration.Getter.IsNull) {
				FixIndentation(indexerDeclaration.Getter);
				if (!indexerDeclaration.Getter.Body.IsNull) {
					if (!policy.AllowPropertyGetBlockInline || indexerDeclaration.Getter.Body.LBraceToken.StartLocation.Line != indexerDeclaration.Getter.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.PropertyGetBraceStyle, indexerDeclaration.Getter.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(indexerDeclaration.Getter.Body, policy.IndentBlocks);
						FixClosingBrace(policy.PropertyGetBraceStyle, indexerDeclaration.Getter.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(indexerDeclaration.Getter.Body, policy.IndentBlocks);
					}
				}
			}

			if (!indexerDeclaration.Setter.IsNull) {
				FixIndentation(indexerDeclaration.Setter);
				if (!indexerDeclaration.Setter.Body.IsNull) {
					if (!policy.AllowPropertySetBlockInline || indexerDeclaration.Setter.Body.LBraceToken.StartLocation.Line != indexerDeclaration.Setter.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.PropertySetBraceStyle, indexerDeclaration.Setter.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(indexerDeclaration.Setter.Body, policy.IndentBlocks);
						FixClosingBrace(policy.PropertySetBraceStyle, indexerDeclaration.Setter.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(indexerDeclaration.Setter.Body, policy.IndentBlocks);
					}
				}
			}
			if (policy.IndentPropertyBody)
				curIndent.Pop();

			FixClosingBrace(policy.PropertyBraceStyle, indexerDeclaration.RBraceToken);
		}

		static bool IsSimpleEvent(AstNode node)
		{
			return node is EventDeclaration;
		}

		public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
		{
			FixAttributesAndDocComment(eventDeclaration);

			FixOpenBrace(policy.EventBraceStyle, eventDeclaration.LBraceToken);
			if (policy.IndentEventBody)
				curIndent.Push(IndentType.Block);

			if (!eventDeclaration.AddAccessor.IsNull) {
				FixIndentation(eventDeclaration.AddAccessor);
				if (!eventDeclaration.AddAccessor.Body.IsNull) {
					if (!policy.AllowEventAddBlockInline || eventDeclaration.AddAccessor.Body.LBraceToken.StartLocation.Line != eventDeclaration.AddAccessor.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.EventAddBraceStyle, eventDeclaration.AddAccessor.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(eventDeclaration.AddAccessor.Body, policy.IndentBlocks);
						FixClosingBrace(policy.EventAddBraceStyle, eventDeclaration.AddAccessor.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(eventDeclaration.AddAccessor.Body, policy.IndentBlocks);
					}
				}
			}

			if (!eventDeclaration.RemoveAccessor.IsNull) {
				FixIndentation(eventDeclaration.RemoveAccessor);
				if (!eventDeclaration.RemoveAccessor.Body.IsNull) {
					if (!policy.AllowEventRemoveBlockInline || eventDeclaration.RemoveAccessor.Body.LBraceToken.StartLocation.Line != eventDeclaration.RemoveAccessor.Body.RBraceToken.StartLocation.Line) {
						FixOpenBrace(policy.EventRemoveBraceStyle, eventDeclaration.RemoveAccessor.Body.LBraceToken);
						VisitBlockWithoutFixingBraces(eventDeclaration.RemoveAccessor.Body, policy.IndentBlocks);
						FixClosingBrace(policy.EventRemoveBraceStyle, eventDeclaration.RemoveAccessor.Body.RBraceToken);
					} else {
						nextStatementIndent = " ";
						VisitBlockWithoutFixingBraces(eventDeclaration.RemoveAccessor.Body, policy.IndentBlocks);
					}
				}
			}

			if (policy.IndentEventBody)
				curIndent.Pop();

			FixClosingBrace(policy.EventBraceStyle, eventDeclaration.RBraceToken);
		}

		public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			FixAttributesAndDocComment(eventDeclaration);

			foreach (var m in eventDeclaration.ModifierTokens) {
				ForceSpacesAfter(m, true);
			}

			ForceSpacesBeforeRemoveNewLines(eventDeclaration.EventToken.GetNextSibling (NoWhitespacePredicate), true);
			eventDeclaration.ReturnType.AcceptVisitor(this);
			ForceSpacesAfter(eventDeclaration.ReturnType, true);
			/*
			var lastLoc = eventDeclaration.StartLocation;
			curIndent.Push(IndentType.Block);
			foreach (var initializer in eventDeclaration.Variables) {
				if (lastLoc.Line != initializer.StartLocation.Line) {
					FixStatementIndentation(initializer.StartLocation);
					lastLoc = initializer.StartLocation;
				}
				initializer.AcceptVisitor(this);
			}
			curIndent.Pop ();
			*/
			FixSemicolon(eventDeclaration.SemicolonToken);
		}

		
		public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			FixAttributesAndDocComment(fieldDeclaration);

			fieldDeclaration.ReturnType.AcceptVisitor(this);
			ForceSpacesAfter(fieldDeclaration.ReturnType, true);

			FormatCommas(fieldDeclaration, policy.SpaceBeforeFieldDeclarationComma, policy.SpaceAfterFieldDeclarationComma);

			var lastLoc = fieldDeclaration.ReturnType.StartLocation;
			foreach (var initializer in fieldDeclaration.Variables) {
				if (lastLoc.Line != initializer.StartLocation.Line) {
					curIndent.Push(IndentType.Block);
					FixStatementIndentation(initializer.StartLocation);
					curIndent.Pop ();
					lastLoc = initializer.StartLocation;
				}
				initializer.AcceptVisitor(this);
			}
			FixSemicolon(fieldDeclaration.SemicolonToken);
		}

		public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
		{
			FixAttributesAndDocComment(fixedFieldDeclaration);

			FormatCommas(fixedFieldDeclaration, policy.SpaceBeforeFieldDeclarationComma, policy.SpaceAfterFieldDeclarationComma);

			var lastLoc = fixedFieldDeclaration.StartLocation;
			curIndent.Push(IndentType.Block);
			foreach (var initializer in fixedFieldDeclaration.Variables) {
				if (lastLoc.Line != initializer.StartLocation.Line) {
					FixStatementIndentation(initializer.StartLocation);
					lastLoc = initializer.StartLocation;
				}
				initializer.AcceptVisitor(this);
			}
			curIndent.Pop ();
			FixSemicolon(fixedFieldDeclaration.SemicolonToken);
		}

		public override void VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
		{
			FixAttributesAndDocComment(enumMemberDeclaration);
			var initializer = enumMemberDeclaration.Initializer;
			if (!initializer.IsNull) {
				ForceSpacesAround(enumMemberDeclaration.AssignToken, policy.SpaceAroundAssignment);
				initializer.AcceptVisitor (this);
			}
		}

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			FixAttributesAndDocComment(methodDeclaration);

			ForceSpacesBefore(methodDeclaration.LParToken, policy.SpaceBeforeMethodDeclarationParentheses);
			if (methodDeclaration.Parameters.Any()) {
				ForceSpacesAfter(methodDeclaration.LParToken, policy.SpaceWithinMethodDeclarationParentheses);
				FormatArguments(methodDeclaration);
			} else {
				ForceSpacesAfter(methodDeclaration.LParToken, policy.SpaceBetweenEmptyMethodDeclarationParentheses);
				ForceSpacesBefore(methodDeclaration.RParToken, policy.SpaceBetweenEmptyMethodDeclarationParentheses);
			}
			
			foreach (var constraint in methodDeclaration.Constraints)
				constraint.AcceptVisitor (this);

			if (!methodDeclaration.Body.IsNull) {
				FixOpenBrace(policy.MethodBraceStyle, methodDeclaration.Body.LBraceToken);
				VisitBlockWithoutFixingBraces(methodDeclaration.Body, policy.IndentMethodBody);
				FixClosingBrace(policy.MethodBraceStyle, methodDeclaration.Body.RBraceToken);
			}
		}

		public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			FixAttributesAndDocComment(operatorDeclaration);

			ForceSpacesBefore(operatorDeclaration.LParToken, policy.SpaceBeforeMethodDeclarationParentheses);
			if (operatorDeclaration.Parameters.Any()) {
				ForceSpacesAfter(operatorDeclaration.LParToken, policy.SpaceWithinMethodDeclarationParentheses);
				FormatArguments(operatorDeclaration);
			} else {
				ForceSpacesAfter(operatorDeclaration.LParToken, policy.SpaceBetweenEmptyMethodDeclarationParentheses);
				ForceSpacesBefore(operatorDeclaration.RParToken, policy.SpaceBetweenEmptyMethodDeclarationParentheses);
			}

			if (!operatorDeclaration.Body.IsNull) {
				FixOpenBrace(policy.MethodBraceStyle, operatorDeclaration.Body.LBraceToken);
				VisitBlockWithoutFixingBraces(operatorDeclaration.Body, policy.IndentMethodBody);
				FixClosingBrace(policy.MethodBraceStyle, operatorDeclaration.Body.RBraceToken);
			}
		}

		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			FixAttributesAndDocComment(constructorDeclaration);

			ForceSpacesBefore(constructorDeclaration.LParToken, policy.SpaceBeforeConstructorDeclarationParentheses);
			if (constructorDeclaration.Parameters.Any()) {
				ForceSpacesAfter(constructorDeclaration.LParToken, policy.SpaceWithinConstructorDeclarationParentheses);
				FormatArguments(constructorDeclaration);
			} else {
				ForceSpacesAfter(constructorDeclaration.LParToken, policy.SpaceBetweenEmptyConstructorDeclarationParentheses);
				ForceSpacesBefore(constructorDeclaration.RParToken, policy.SpaceBetweenEmptyConstructorDeclarationParentheses);
			}

			if (!constructorDeclaration.Body.IsNull) {
				FixOpenBrace(policy.ConstructorBraceStyle, constructorDeclaration.Body.LBraceToken);
				VisitBlockWithoutFixingBraces(constructorDeclaration.Body, policy.IndentMethodBody);
				FixClosingBrace(policy.ConstructorBraceStyle, constructorDeclaration.Body.RBraceToken);
			}
		}

		public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			FixAttributesAndDocComment(destructorDeclaration);

			CSharpTokenNode lParen = destructorDeclaration.LParToken;
			ForceSpaceBefore(lParen, policy.SpaceBeforeConstructorDeclarationParentheses);

			if (!destructorDeclaration.Body.IsNull) {
				FixOpenBrace(policy.DestructorBraceStyle, destructorDeclaration.Body.LBraceToken);
				VisitBlockWithoutFixingBraces(destructorDeclaration.Body, policy.IndentMethodBody);
				FixClosingBrace(policy.DestructorBraceStyle, destructorDeclaration.Body.RBraceToken);
			}
		}
	}
}

