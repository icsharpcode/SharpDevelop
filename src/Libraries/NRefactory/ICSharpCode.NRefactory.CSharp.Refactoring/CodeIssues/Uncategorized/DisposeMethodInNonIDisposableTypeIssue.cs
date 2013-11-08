// DisposeMethodInNonIDisposableTypeIssue.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Type does not implement IDisposable despite having a Dispose method",
	                  Description="This type declares a method named Dispose, but it does not implement the System.IDisposable interface",
	                  Category=IssueCategories.CodeQualityIssues,
	                  Severity=Severity.Warning)]
	public class DisposeMethodInNonIDisposableTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		private class GatherVisitor : GatherVisitorBase<DisposeMethodInNonIDisposableTypeIssue>
		{
			public GatherVisitor(BaseRefactoringContext context)
				: base(context)
			{
			}

			static bool IsDisposeMethod(MethodDeclaration methodDeclaration)
			{
				if (!methodDeclaration.PrivateImplementationType.IsNull) {
					//Ignore explictly implemented methods
					return false;
				}
				if (methodDeclaration.Name != "Dispose") {
					return false;
				}
				if (methodDeclaration.Parameters.Count != 0) {
					return false;
				}

				if (methodDeclaration.HasModifier(Modifiers.Static)) {
					return false;
				}

				var primitiveType = methodDeclaration.ReturnType as PrimitiveType;
				if (primitiveType == null || primitiveType.KnownTypeCode != KnownTypeCode.Void) {
					return false;
				}

				return true;
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration.ClassType == ClassType.Enum) {
					//enums have no methods
					return;
				}

				var resolve = ctx.Resolve(typeDeclaration) as TypeResolveResult;
				if (resolve != null && Implements(resolve.Type, "System.IDisposable")) {
					return;
				}

				base.VisitTypeDeclaration(typeDeclaration);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (!IsDisposeMethod(methodDeclaration)) {
					return;
				}

				var type = methodDeclaration.GetParent<TypeDeclaration>();
				if (type == null) {
					return;
				}

				AddIssue(new CodeIssue(methodDeclaration.NameToken,
				         ctx.TranslateString("Type does not implement IDisposable despite having a Dispose method"),
				         ctx.TranslateString("Implement IDisposable"),
					script => Fix(script, methodDeclaration, type)));
			}

			static IEnumerable<MethodDeclaration> DisposeMethods(TypeDeclaration newTypeDeclaration)
			{
				return newTypeDeclaration.Members
					.OfType<MethodDeclaration>()
						.Where(IsDisposeMethod);
			}

			void Fix(Script script, MethodDeclaration methodDeclaration, TypeDeclaration typeDeclaration)
			{
				var newTypeDeclaration = (TypeDeclaration) typeDeclaration.Clone();

				var resolver = ctx.GetResolverStateAfter(typeDeclaration.LBraceToken);

				var typeResolve = resolver.ResolveSimpleName("IDisposable", new List<IType>()) as TypeResolveResult;
				bool canShortenIDisposable = typeResolve != null && typeResolve.Type.FullName == "System.IDisposable";

				string interfaceName = (canShortenIDisposable ? string.Empty : "System.") + "IDisposable";

				newTypeDeclaration.BaseTypes.Add(new SimpleType(interfaceName));

				foreach (var method in DisposeMethods(newTypeDeclaration).ToList()) {
					if (typeDeclaration.ClassType == ClassType.Interface) {
						method.Remove();
					}
					else {
						method.Modifiers &= ~Modifiers.Private;
						method.Modifiers &= ~Modifiers.Protected;
						method.Modifiers &= ~Modifiers.Internal;
						method.Modifiers |= Modifiers.Public;
					}
				}

				if (typeDeclaration.ClassType == ClassType.Interface) {
					var disposeMember = ((MemberResolveResult)ctx.Resolve(methodDeclaration)).Member;
					script.DoGlobalOperationOn(new List<IEntity>() { disposeMember }, (nCtx, nScript, nodes) => {
						List<Tuple<AstType, AstType>> pendingChanges = new List<Tuple<AstType, AstType>>();
						foreach (var node in nodes)
						{
							var method = node as MethodDeclaration;
							if (method != null && !method.PrivateImplementationType.IsNull) {
								var nResolver = ctx.GetResolverStateAfter(typeDeclaration.LBraceToken);

								var nTypeResolve = nResolver.ResolveSimpleName("IDisposable", new List<IType>()) as TypeResolveResult;
								bool nCanShortenIDisposable = nTypeResolve != null && nTypeResolve.Type.FullName == "System.IDisposable";

								string nInterfaceName = (nCanShortenIDisposable ? string.Empty : "System.") + "IDisposable";

								pendingChanges.Add(Tuple.Create(method.PrivateImplementationType, AstType.Create(nInterfaceName)));
							}
						}

						foreach (var change in pendingChanges) {
							nScript.Replace(change.Item1, change.Item2);
						}
					}, "Fix explicitly implemented members");
				}

				script.Replace(typeDeclaration, newTypeDeclaration);
			}

			static bool Implements(IType type, string fullName)
			{
				return type.GetAllBaseTypes ().Any (baseType => baseType.FullName == fullName);
			}

			//Ignore entities that are not methods -- don't visit children
			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
			}
		}
	}
}

