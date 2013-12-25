// 
// ImplementInterfaceAction.cs
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
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Implement interface", Description = "Creates an interface implementation.")]
	public class ImplementInterfaceAction : CodeActionProvider
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
			if (resolveResult.Type.Kind != TypeKind.Interface)
				yield break;
			
			bool interfaceMissing;
			var toImplement = CollectMembersToImplement(state.CurrentTypeDefinition, resolveResult.Type, false, out interfaceMissing);
			if (toImplement.Count == 0)
				yield break;
			
			yield return new CodeAction(context.TranslateString("Implement interface"), script =>
				script.InsertWithCursor(
					context.TranslateString("Implement Interface"),
					state.CurrentTypeDefinition,
					(s, c) => GenerateImplementation(c, toImplement, interfaceMissing).ToList()
				)
			, type);
		}

		static void RemoveConstraints(EntityDeclaration decl)
		{
			foreach (var child in decl.GetChildrenByRole(Roles.Constraint).ToArray()) {
				child.Remove();
			}
		}
		
		public static IEnumerable<AstNode> GenerateImplementation(RefactoringContext context, IEnumerable<Tuple<IMember, bool>> toImplement, bool generateRegion)
		{
			var service = (CodeGenerationService)context.GetService(typeof(CodeGenerationService)); 
			if (service == null)
				yield break;
			var nodes = new Dictionary<IType, List<EntityDeclaration>>();
			
			foreach (var member in toImplement) {
				if (!nodes.ContainsKey(member.Item1.DeclaringType)) 
					nodes [member.Item1.DeclaringType] = new List<EntityDeclaration>();
				var decl = service.GenerateMemberImplementation(context, member.Item1, member.Item2);
				if (member.Item2 || member.Item1.DeclaringType.Kind != TypeKind.Interface)
					RemoveConstraints(decl);

				nodes[member.Item1.DeclaringType].Add(decl);
			}
			
			foreach (var kv in nodes) {
				if (generateRegion) {
					if (kv.Key.Kind == TypeKind.Interface) {
						yield return new PreProcessorDirective(
							PreProcessorDirectiveType.Region,
							string.Format("{0} implementation", kv.Key.Name));
					} else {
						yield return new PreProcessorDirective(
							PreProcessorDirectiveType.Region,
							string.Format("implemented abstract members of {0}", kv.Key.Name));
					}
				}
				foreach (var member in kv.Value)
					yield return member;
				if (generateRegion) {
					yield return new PreProcessorDirective(
						PreProcessorDirectiveType.Endregion
					);
				}
			}
		}

		static bool IsImplementation(IMember m, IMember method)
		{
			return m.UnresolvedMember == method.UnresolvedMember;
		}

		public static List<Tuple<IMember, bool>> CollectMembersToImplement(ITypeDefinition implementingType, IType interfaceType, bool explicitly, out bool interfaceMissing)
		{
			//var def = interfaceType.GetDefinition();
			List<Tuple<IMember, bool>> toImplement = new List<Tuple<IMember, bool>>();
			bool alreadyImplemented;
			interfaceMissing = true;
			// Stub out non-implemented events defined by @iface
			foreach (var evGroup in interfaceType.GetEvents (e => !e.IsSynthetic).GroupBy (m => m.DeclaringType).Reverse ())
				foreach (var ev in evGroup) {
					if (ev.DeclaringType.Kind != TypeKind.Interface)
						continue;

					bool needsExplicitly = explicitly;
					alreadyImplemented = implementingType.GetMembers().Any(m => m.ImplementedInterfaceMembers.Any(im => IsImplementation (im, ev)));
				
					if (!alreadyImplemented) {
						toImplement.Add(new Tuple<IMember, bool>(ev, needsExplicitly));
					} else {
						interfaceMissing = false;
					}
				}
			
			// Stub out non-implemented methods defined by @iface
			foreach (var methodGroup in interfaceType.GetMethods (d => !d.IsSynthetic).GroupBy (m => m.DeclaringType).Reverse ())
				foreach (var method in methodGroup) {
					if (method.DeclaringType.Kind != TypeKind.Interface)
						continue;
					bool needsExplicitly = explicitly;
					alreadyImplemented = false;

					foreach (var cmet in implementingType.GetMethods ()) {
						alreadyImplemented |= cmet.ImplementedInterfaceMembers.Any(m => IsImplementation (m, method));

						if (CompareMembers(method, cmet)) {
							if (!needsExplicitly && !cmet.ReturnType.Equals(method.ReturnType))
								needsExplicitly = true;
							else
								alreadyImplemented |= !needsExplicitly /*|| cmet.InterfaceImplementations.Any (impl => impl.InterfaceType.Equals (interfaceType))*/;
						}
					}
					if (toImplement.Where(t => t.Item1 is IMethod).Any(t => CompareMembers(method, (IMethod)t.Item1)))
						needsExplicitly = true;
					if (!alreadyImplemented) {
						toImplement.Add(new Tuple<IMember, bool>(method, needsExplicitly));
					} else {
						interfaceMissing = false;
					}
				}
			
			// Stub out non-implemented properties defined by @iface
			foreach (var propGroup in interfaceType.GetProperties (p => !p.IsSynthetic).GroupBy (m => m.DeclaringType).Reverse ())
				foreach (var prop in propGroup) {
					if (prop.DeclaringType.Kind != TypeKind.Interface)
						continue;

					bool needsExplicitly = explicitly;
					alreadyImplemented = implementingType.GetMembers().Any(m => m.ImplementedInterfaceMembers.Any(im => IsImplementation (im, prop)));

					foreach (var t in implementingType.GetAllBaseTypeDefinitions ()) {
						if (t.Kind == TypeKind.Interface) {
							foreach (var cprop in t.Properties) {
								if (cprop.Name == prop.Name && cprop.IsShadowing) {
									if (!needsExplicitly && !cprop.ReturnType.Equals(prop.ReturnType))
										needsExplicitly = true;
								}
							}
							continue;
						}
						foreach (var cprop in t.Properties) {
							if (cprop.Name == prop.Name) {
								if (!needsExplicitly && !cprop.ReturnType.Equals(prop.ReturnType))
									needsExplicitly = true;
								else
									alreadyImplemented |= !needsExplicitly/* || cprop.InterfaceImplementations.Any (impl => impl.InterfaceType.Resolve (ctx).Equals (interfaceType))*/;
							}
						}
					}
					if (!alreadyImplemented) {
						toImplement.Add(new Tuple<IMember, bool>(prop, needsExplicitly));
					} else {
						interfaceMissing = false;
					}
				}
			return toImplement;
		}
		
		internal static bool CompareMembers(IMember interfaceMethod, IMember typeMethod)
		{
			if (typeMethod.IsExplicitInterfaceImplementation)
				return typeMethod.ImplementedInterfaceMembers.Any(m => m.Equals(interfaceMethod));
			return SignatureComparer.Ordinal.Equals(interfaceMethod, typeMethod);
		}
	}
}

