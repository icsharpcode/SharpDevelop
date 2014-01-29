//
// ChangeAccessModifierAction.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Changes the access level of an entity declaration
	/// </summary>
	[ContextAction("Change the access level of an entity declaration", Description = "Changes the access level of an entity declaration")]
	public class ChangeAccessModifierAction : CodeActionProvider
	{
		Dictionary<string, Modifiers> accessibilityLevels = new Dictionary<string, Modifiers> {
			{ "private", Modifiers.Private },
			{ "protected", Modifiers.Protected },
			{ "protected internal", Modifiers.Protected | Modifiers.Internal },
			{ "internal", Modifiers.Internal },
			{ "public", Modifiers.Public }
		};

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var node = context.GetNode<EntityDeclaration>();
			if (node == null)
				yield break;

			var selectedNode = node.GetNodeAt(context.Location);
			if (selectedNode == null)
				yield break;

			if (selectedNode.Role != PropertyDeclaration.SetKeywordRole && 
			    selectedNode.Role != PropertyDeclaration.GetKeywordRole && 
			    selectedNode != node.NameToken) {
				if (selectedNode.Role == EntityDeclaration.ModifierRole) {
					var mod = (CSharpModifierToken)selectedNode;
					if ((mod.Modifier & Modifiers.VisibilityMask) == 0)
						yield break;
				} else {
					yield break;
				}
			}

			if (node is EnumMemberDeclaration) {
				yield break;
			}

			if (node.HasModifier(Modifiers.Override))
				yield break;

			var parentTypeDeclaration = node.GetParent<TypeDeclaration>();
			if (parentTypeDeclaration != null && parentTypeDeclaration.ClassType == ClassType.Interface) {
				//Interface members have no access modifiers
				yield break;
			}

			var resolveResult = context.Resolve(node) as MemberResolveResult;
			if (resolveResult != null) {
				if (resolveResult.Member.ImplementedInterfaceMembers.Any())
					yield break;
			}

			foreach (var accessName in accessibilityLevels.Keys) {
				var access = accessibilityLevels [accessName];

				if (parentTypeDeclaration == null && ((access & (Modifiers.Private | Modifiers.Protected)) != 0)) {
					//Top-level declarations can only be public or internal
					continue;
				}

				var accessor = node as Accessor;
				if (accessor != null) {
					//Allow only converting to modifiers stricter than the parent entity

					var actualParentAccess = GetActualAccess(parentTypeDeclaration, accessor.GetParent<EntityDeclaration>());
					if (access != actualParentAccess && !IsStricterThan (access, actualParentAccess)) {
						continue;
					}
				}

				if (GetActualAccess(parentTypeDeclaration, node) != access) {
					yield return GetActionForLevel(context, accessName, access, node, selectedNode);
				}
			}
		}

		static bool IsStricterThan(Modifiers access1, Modifiers access2)
		{
			//First cover the basic cases
			if (access1 == access2) {
				return false;
			}

			if (access1 == Modifiers.Private) {
				return true;
			}
			if (access2 == Modifiers.Private) {
				return false;
			}

			if (access1 == Modifiers.Public) {
				return false;
			}
			if (access2 == Modifiers.Public) {
				return true;
			}

			return access2 == (Modifiers.Protected | Modifiers.Internal);
		}

		static Modifiers GetActualAccess(AstNode parentTypeDeclaration, EntityDeclaration node)
		{
			Modifiers nodeAccess = node.Modifiers & Modifiers.VisibilityMask;
			if (nodeAccess == Modifiers.None && node is Accessor) {
				EntityDeclaration parent = node.GetParent<EntityDeclaration>();

				nodeAccess = parent.Modifiers & Modifiers.VisibilityMask;
			}

			if (nodeAccess == Modifiers.None) {
				if (parentTypeDeclaration == null) {
					return Modifiers.Internal;
				}
				return Modifiers.Private;
			}

			return nodeAccess & Modifiers.VisibilityMask;
		}

		CodeAction GetActionForLevel(RefactoringContext context, string accessName, Modifiers access, EntityDeclaration node, AstNode selectedNode)
		{
			return new CodeAction(context.TranslateString("To " + accessName), script => {

				Modifiers newModifiers = node.Modifiers;
				newModifiers &= ~Modifiers.VisibilityMask;
				
				if (!(node is Accessor) || access != (node.GetParent<EntityDeclaration>().Modifiers & Modifiers.VisibilityMask)) {
					//Do not add access modifier for accessors if the new access level is the same as the parent
					//That is, in public int X { $private get; } if access == public, then the result should not have the modifier
					newModifiers |= access;
				}

				script.ChangeModifier(node, newModifiers);

			}, selectedNode);
		}
	}
}

