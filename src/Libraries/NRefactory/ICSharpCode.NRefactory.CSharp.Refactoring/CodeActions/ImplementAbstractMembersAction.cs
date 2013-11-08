// 
// ImplementAbstractMembersAction.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Implement abstract members", Description = "Implements abstract members from an abstract class.")]
	public class ImplementAbstractMembersAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var service = (CodeGenerationService)context.GetService(typeof(CodeGenerationService)); 
			if (service == null)
				yield break;

			var type = context.GetNode<AstType>();
			if (type == null || type.Role != Roles.BaseType)
				yield break;
			var state = context.GetResolverStateBefore(type);
			if (state.CurrentTypeDefinition == null)
				yield break;

			var resolveResult = context.Resolve(type);
			if (resolveResult.Type.Kind != TypeKind.Class || resolveResult.Type.GetDefinition() == null || !resolveResult.Type.GetDefinition().IsAbstract)
				yield break;

			var toImplement = CollectMembersToImplement(state.CurrentTypeDefinition, resolveResult.Type);
			if (toImplement.Count == 0)
				yield break;

			yield return new CodeAction(
				context.TranslateString("Implement abstract members"), 
				script => script.InsertWithCursor(
					context.TranslateString("Implement abstract members"), 
					state.CurrentTypeDefinition, (s, c) => ImplementInterfaceAction.GenerateImplementation(c, toImplement.Select(m => Tuple.Create(m, false)), true)
				.Select(entity => {
					var decl = entity as EntityDeclaration;
					if (decl != null)
						decl.Modifiers |= Modifiers.Override;
					return entity;
				}).ToList()), type);
		}

		public static List<IMember> CollectMembersToImplement(ITypeDefinition implementingType, IType abstractType)
		{
			var toImplement = new List<IMember>();
			bool alreadyImplemented;
			foreach (var member in abstractType.GetMembers (d => !d.IsSynthetic  && d.IsAbstract)) {
				alreadyImplemented = false;
				var allBaseTypes = member.DeclaringType.GetAllBaseTypes().ToList ();
				foreach (var cmet in implementingType.GetMembers (d => d.SymbolKind == member.SymbolKind && d.Name == member.Name)) {
					if (allBaseTypes.Contains(cmet.DeclaringType))
						continue;
					if (ImplementInterfaceAction.CompareMembers(member, cmet)) {
						alreadyImplemented = true;
						break;
					}
				}
				if (!alreadyImplemented) 
					toImplement.Add(member);
			}
			return toImplement;
		}

	}
}
