//
// AbstractAndVirtualConversionAction.cs
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
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Make abstract member virtual", Description = "Implements an abstract member as a virtual one")]
	public class AbstractAndVirtualConversionAction : CodeActionProvider
	{
		static BlockStatement CreateNotImplementedBody(RefactoringContext context, out ThrowStatement throwStatement)
		{
			throwStatement = new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")));
			return new BlockStatement { throwStatement };
		}

		static ThrowStatement ImplementStub (RefactoringContext context, EntityDeclaration newNode)
		{
			ThrowStatement throwStatement = null;
			if (newNode is PropertyDeclaration || newNode is IndexerDeclaration) {
				var setter = newNode.GetChildByRole(PropertyDeclaration.SetterRole);
				if (!setter.IsNull)
					setter.AddChild(CreateNotImplementedBody(context, out throwStatement), Roles.Body); 

				var getter = newNode.GetChildByRole(PropertyDeclaration.GetterRole);
				if (!getter.IsNull)
					getter.AddChild(CreateNotImplementedBody(context, out throwStatement), Roles.Body); 
			} else {
				newNode.AddChild(CreateNotImplementedBody(context, out throwStatement), Roles.Body); 
			}
			return throwStatement;
		}

		static bool CheckBody(EntityDeclaration node)
		{
			var custom = node as CustomEventDeclaration;
			if (custom != null && !(IsValidBody (custom.AddAccessor.Body) || IsValidBody (custom.RemoveAccessor.Body)))
			    return false;
			if (node is PropertyDeclaration || node is IndexerDeclaration) {
				var setter = node.GetChildByRole(PropertyDeclaration.SetterRole);
				var getter = node.GetChildByRole(PropertyDeclaration.GetterRole);
				return IsValidBody(setter.Body) && IsValidBody(getter.Body);
			} 
			return IsValidBody(node.GetChildByRole(Roles.Body));
		}

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var node = context.GetNode<EntityDeclaration>();
			if (node == null || node.HasModifier(Modifiers.Override))
				yield break;

			var parent = node.Parent as TypeDeclaration;
			if (parent == null)
				yield break;

			var custom = node as CustomEventDeclaration;
			if (custom != null) {
				if (!custom.PrivateImplementationType.IsNull)
					yield break;
			}

			var selectedNode = node.GetNodeAt(context.Location);
			if (selectedNode == null)
				yield break;
			if (selectedNode != node.NameToken) {
				if ((node is EventDeclaration && node is CustomEventDeclaration || selectedNode.Role != Roles.Identifier) && 
				    selectedNode.Role != IndexerDeclaration.ThisKeywordRole) {
					var modToken = selectedNode as CSharpModifierToken;
					if (modToken == null || (modToken.Modifier & (Modifiers.Abstract | Modifiers.Virtual)) == 0)
						yield break;
				} else {
					if (!(node is EventDeclaration || node is CustomEventDeclaration) && selectedNode.Parent != node)
						yield break;
				}
			}
			if (!node.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole).IsNull)
				yield break;

			if (parent.HasModifier(Modifiers.Abstract)) {
				if ((node.Modifiers & Modifiers.Abstract) != 0) {
					yield return new CodeAction(context.TranslateString("To non-abstract"), script => {
						var newNode = (EntityDeclaration)node.Clone();
						newNode.Modifiers &= ~Modifiers.Abstract;
						var throwStmt = ImplementStub (context, newNode);
						script.Replace(node, newNode);
						if (throwStmt != null)
							script.Select(throwStmt); 
					}, selectedNode);
				} else {
					if (CheckBody(node)) {
						yield return new CodeAction(context.TranslateString("To abstract"), script => {
							var newNode = CloneNodeWithoutBodies(node);
							newNode.Modifiers &= ~Modifiers.Virtual;
							newNode.Modifiers &= ~Modifiers.Static;
							newNode.Modifiers |= Modifiers.Abstract;
							script.Replace(node, newNode);
						}, selectedNode);
					}
				}
			}

			if ((node.Modifiers & Modifiers.Virtual) != 0) {
				yield return new CodeAction(context.TranslateString("To non-virtual"), script => {
					script.ChangeModifier(node, node.Modifiers & ~Modifiers.Virtual);
				}, selectedNode);
			} else {
				if ((node.Modifiers & Modifiers.Abstract) != 0) {
					yield return new CodeAction(context.TranslateString("To virtual"), script => {
						var newNode = CloneNodeWithoutBodies(node);
						newNode.Modifiers &= ~Modifiers.Abstract;
						newNode.Modifiers &= ~Modifiers.Static;
						newNode.Modifiers |= Modifiers.Virtual;
						var throwStmt = ImplementStub (context, newNode);
						script.Replace(node, newNode);
						if (throwStmt != null)
							script.Select(throwStmt);
					}, selectedNode);
				} else {
					yield return new CodeAction(context.TranslateString("To virtual"), script => {
						script.ChangeModifier(node, (node.Modifiers & ~Modifiers.Static)  | Modifiers.Virtual);
					}, selectedNode);
				}
			}

		}

		static EntityDeclaration CloneNodeWithoutBodies(EntityDeclaration node)
		{
			EntityDeclaration newNode;
			var custom = node as CustomEventDeclaration;

			if (custom == null) {
				newNode = (EntityDeclaration)node.Clone();

				if (newNode is PropertyDeclaration || node is IndexerDeclaration) {
					var getter = newNode.GetChildByRole(PropertyDeclaration.GetterRole);
					if (!getter.IsNull)
						getter.Body.Remove();
					var setter = newNode.GetChildByRole(PropertyDeclaration.SetterRole);
					if (!setter.IsNull)
						setter.Body.Remove();
				} else {
					newNode.GetChildByRole(Roles.Body).Remove();
				}
			} else {
				newNode = new EventDeclaration {
					Modifiers = custom.Modifiers,
					ReturnType = custom.ReturnType.Clone(),
					Variables =  {
						new VariableInitializer {
							Name = custom.Name
						}
					}
				};
			}
			return newNode;
		}

		static bool IsValidBody(BlockStatement body)
		{
			if (body.IsNull)
				return true;
			var first = body.Statements.FirstOrDefault();
			if (first == null)
				return true;
			if (first.GetNextSibling(s => s.Role == BlockStatement.StatementRole) != null)
				return false;
			return first is EmptyStatement || first is ThrowStatement;
		}
	}
}

