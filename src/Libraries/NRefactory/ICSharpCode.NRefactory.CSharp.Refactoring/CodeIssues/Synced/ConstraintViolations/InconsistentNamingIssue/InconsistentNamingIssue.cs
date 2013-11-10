// 
// InconsistentNamingIssue.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Inconsistent Naming",
	       Description = "Name doesn't match the defined style for this entity.",
           Category = IssueCategories.ConstraintViolations,
	       Severity = Severity.Warning,
           AnalysisDisableKeyword = "InconsistentNaming")]
	public class InconsistentNamingIssue : CodeIssueProvider
	{
		public override IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context, string subIssue)
		{
			var visitor = new GatherVisitor(context);
			context.RootNode.AcceptVisitor(visitor);
			return visitor.FoundIssues;
		}

		class GatherVisitor : GatherVisitorBase<InconsistentNamingIssue>
		{
			readonly NamingConventionService service;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
				service = (NamingConventionService)ctx.GetService (typeof (NamingConventionService));
			}

			void CheckName(TypeDeclaration node, AffectedEntity entity, Identifier identifier, Modifiers accessibilty)
			{
				TypeResolveResult resolveResult = ctx.Resolve(node) as TypeResolveResult;
				if (resolveResult == null)
					return;
				var type = resolveResult.Type;
				if (type.DirectBaseTypes.Any(t => t.FullName == "System.Attribute")) {
					if (CheckNamedResolveResult(resolveResult, node, AffectedEntity.CustomAttributes, identifier, accessibilty)) {
						return;
					}
				} else if (type.DirectBaseTypes.Any(t => t.FullName == "System.EventArgs")) {
					if (CheckNamedResolveResult(resolveResult, node, AffectedEntity.CustomEventArgs, identifier, accessibilty)) {
						return;
					}
				} else if (type.DirectBaseTypes.Any(t => t.FullName == "System.Exception")) {
					if (CheckNamedResolveResult(resolveResult, node, AffectedEntity.CustomExceptions, identifier, accessibilty)) {
						return;
					}
				}

				var typeDef = type.GetDefinition();
				if (typeDef != null && typeDef.Attributes.Any(attr => attr.AttributeType.FullName == "NUnit.Framework.TestFixtureAttribute")) {
					if (CheckNamedResolveResult(resolveResult, node, AffectedEntity.TestType, identifier, accessibilty)) {
						return;
					}
				}

				CheckNamedResolveResult(resolveResult, node, entity, identifier, accessibilty);
			}

			void CheckName(MethodDeclaration node, AffectedEntity entity, Identifier identifier, Modifiers accessibilty)
			{
				ResolveResult resolveResult = null;
				if (node != null && node.Attributes.Any ())
					resolveResult = ctx.Resolve(node);
				
				if (resolveResult is MemberResolveResult) {
					var member = ((MemberResolveResult)resolveResult).Member;
					if (member.SymbolKind == SymbolKind.Method && member.Attributes.Any(attr => attr.AttributeType.FullName == "NUnit.Framework.TestAttribute")) {
						if (CheckNamedResolveResult(resolveResult, node, AffectedEntity.TestMethod, identifier, accessibilty)) {
							return;
						}
					}
				}

				CheckNamedResolveResult(resolveResult, node, entity, identifier, accessibilty);
			}

			void CheckName(AstNode node, AffectedEntity entity, Identifier identifier, Modifiers accessibilty)
			{
				CheckNamedResolveResult(null, node, entity, identifier, accessibilty);
			}

			bool CheckNamedResolveResult(ResolveResult resolveResult, AstNode node, AffectedEntity entity, Identifier identifier, Modifiers accessibilty)
			{
				bool wasHandled = false;
				foreach (var rule in service.Rules) {
					if (!rule.AffectedEntity.HasFlag(entity)) {
						continue;
					}
					if (!rule.VisibilityMask.HasFlag(accessibilty)) {
						continue;
					}
					if (!rule.IncludeInstanceMembers || !rule.IncludeStaticEntities) {
						EntityDeclaration typeSystemEntity;
						if (node is VariableInitializer) {
							typeSystemEntity = node.Parent as EntityDeclaration;
						} else {
							typeSystemEntity = node as EntityDeclaration;
						}
						if (!rule.IncludeInstanceMembers) {
							if (typeSystemEntity == null || !typeSystemEntity.HasModifier (Modifiers.Static) || typeSystemEntity.HasModifier (Modifiers.Sealed)) {
								continue;
							}
						}
						if (!rule.IncludeStaticEntities) {
							if (typeSystemEntity == null || typeSystemEntity.HasModifier (Modifiers.Static) || typeSystemEntity.HasModifier (Modifiers.Sealed)) {
								continue;
							}
						}
					}

					wasHandled = true;
					if (!rule.IsValid(identifier.Name)) {
						IList<string> suggestedNames;
						var msg = rule.GetErrorMessage(ctx, identifier.Name, out suggestedNames);
						var actions = new List<CodeAction>(suggestedNames.Select(n => new CodeAction(string.Format(ctx.TranslateString("Rename to '{0}'"), n), (Script script) => {
							if (resolveResult == null)
								resolveResult = ctx.Resolve (node);
							if (resolveResult is MemberResolveResult) {
								script.Rename(((MemberResolveResult)resolveResult).Member, n);
							} else if (resolveResult is TypeResolveResult) {
								var def = resolveResult.Type.GetDefinition();
								if (def != null) {
									script.Rename(def, n);
								} else if (resolveResult.Type.Kind == TypeKind.TypeParameter) {
									script.Rename((ITypeParameter)resolveResult.Type, n);
								}
							} else if (resolveResult is LocalResolveResult) {
								script.Rename(((LocalResolveResult)resolveResult).Variable, n);
							} else if (resolveResult is NamespaceResolveResult) {
								script.Rename(((NamespaceResolveResult)resolveResult).Namespace, n);
							} else { 
								script.Replace(identifier, Identifier.Create(n));
							}
						}, identifier)));

						if (entity != AffectedEntity.Label) {
							actions.Add(new CodeAction(string.Format(ctx.TranslateString("Rename '{0}'..."), identifier.Name), (Script script) => {
								if (resolveResult == null)
									resolveResult = ctx.Resolve (node);
								if (resolveResult is MemberResolveResult) {
									script.Rename(((MemberResolveResult)resolveResult).Member);
								} else if (resolveResult is TypeResolveResult) {
									var def = resolveResult.Type.GetDefinition();
									if (def != null) {
										script.Rename(def);
									} else if (resolveResult.Type.Kind == TypeKind.TypeParameter) {
										script.Rename((ITypeParameter)resolveResult.Type);
									}
								} else if (resolveResult is LocalResolveResult) {
									script.Rename(((LocalResolveResult)resolveResult).Variable);
								} else if (resolveResult is NamespaceResolveResult) {
									script.Rename(((NamespaceResolveResult)resolveResult).Namespace);
								}
							}, identifier));
						}

						AddIssue(new CodeIssue(identifier, msg, actions));
					}
				}
				return wasHandled;
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				base.VisitNamespaceDeclaration(namespaceDeclaration);
				var type = namespaceDeclaration.NamespaceName;
				while (type is MemberType) {
					var mt = (MemberType)type;
					CheckNamedResolveResult(null, namespaceDeclaration, AffectedEntity.Namespace, mt.MemberNameToken, Modifiers.None);
					type = mt.Target;
				}
				if (type is SimpleType)
					CheckNamedResolveResult(null, namespaceDeclaration, AffectedEntity.Namespace, ((SimpleType)type).IdentifierToken, Modifiers.None);
			}

			Modifiers GetAccessibiltiy(EntityDeclaration decl, Modifiers defaultModifier)
			{
				var accessibility = (decl.Modifiers & Modifiers.VisibilityMask);
				if (accessibility == Modifiers.None) {
					return defaultModifier;
				}
				return accessibility;
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				base.VisitTypeDeclaration(typeDeclaration);
				AffectedEntity entity;
				switch (typeDeclaration.ClassType) {
					case ClassType.Class:
						entity = AffectedEntity.Class;
						break;
					case ClassType.Struct:
						entity = AffectedEntity.Struct;
						break;
					case ClassType.Interface:
						entity = AffectedEntity.Interface;
						break;
					case ClassType.Enum:
						entity = AffectedEntity.Enum;
						break;
					default:
						throw new System.ArgumentOutOfRangeException();
				}
				CheckName(typeDeclaration, entity, typeDeclaration.NameToken, GetAccessibiltiy(typeDeclaration, typeDeclaration.Parent is TypeDeclaration ? Modifiers.Private : Modifiers.Internal));
			}

			public override void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
			{
				base.VisitDelegateDeclaration(delegateDeclaration);
				CheckName(delegateDeclaration, AffectedEntity.Delegate, delegateDeclaration.NameToken, GetAccessibiltiy(delegateDeclaration, delegateDeclaration.Parent is TypeDeclaration ? Modifiers.Private : Modifiers.Internal));
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				if (propertyDeclaration.Modifiers.HasFlag (Modifiers.Override))
					return;
				base.VisitPropertyDeclaration(propertyDeclaration);
				CheckName(propertyDeclaration, AffectedEntity.Property, propertyDeclaration.NameToken, GetAccessibiltiy(propertyDeclaration, Modifiers.Private));
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				if (indexerDeclaration.Modifiers.HasFlag(Modifiers.Override)) {
					var rr = ctx.Resolve (indexerDeclaration) as MemberResolveResult;
					if (rr == null)
						return;
					var baseType = rr.Member.DeclaringType.DirectBaseTypes.FirstOrDefault (t => t.Kind != TypeKind.Interface);
					var method = baseType != null ? baseType.GetProperties (m => m.IsIndexer && m.IsOverridable && m.Parameters.Count == indexerDeclaration.Parameters.Count).FirstOrDefault () : null;
					if (method == null)
						return;
					int i = 0;
					foreach (var par in indexerDeclaration.Parameters) {
						if (method.Parameters[i++].Name != par.Name) {
							par.AcceptVisitor (this);
						}
					}
					return;
				}
				base.VisitIndexerDeclaration(indexerDeclaration);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (methodDeclaration.Modifiers.HasFlag(Modifiers.Override)) {
					var rr = ctx.Resolve (methodDeclaration) as MemberResolveResult;
					if (rr == null)
						return;
					var baseType = rr.Member.DeclaringType.DirectBaseTypes.FirstOrDefault (t => t.Kind != TypeKind.Interface);
					var method = baseType != null ? baseType.GetMethods (m => m.Name == rr.Member.Name && m.IsOverridable && m.Parameters.Count == methodDeclaration.Parameters.Count).FirstOrDefault () : null;
					if (method == null)
						return;
					int i = 0;
					foreach (var par in methodDeclaration.Parameters) {
						if (method.Parameters[i++].Name != par.Name) {
							par.AcceptVisitor (this);
						}
					}

					return;
				}
				base.VisitMethodDeclaration(methodDeclaration);

				CheckName(methodDeclaration, methodDeclaration.Modifiers.HasFlag(Modifiers.Async) ? AffectedEntity.AsyncMethod : AffectedEntity.Method, methodDeclaration.NameToken, GetAccessibiltiy(methodDeclaration, Modifiers.Private));
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				base.VisitFieldDeclaration(fieldDeclaration);
				var entity = AffectedEntity.Field;
				if (fieldDeclaration.Modifiers.HasFlag(Modifiers.Const)) {
					entity = AffectedEntity.ConstantField;
				} else if (fieldDeclaration.Modifiers.HasFlag(Modifiers.Readonly)) {
					entity = AffectedEntity.ReadonlyField;
				}
				foreach (var init in fieldDeclaration.Variables) {
					CheckName(init, entity, init.NameToken, GetAccessibiltiy(fieldDeclaration, Modifiers.Private));
				}
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
				base.VisitFixedFieldDeclaration(fixedFieldDeclaration);
				var entity = AffectedEntity.Field;
				if (fixedFieldDeclaration.Modifiers.HasFlag(Modifiers.Const)) {
					entity = AffectedEntity.ConstantField;
				} else if (fixedFieldDeclaration.Modifiers.HasFlag(Modifiers.Readonly)) {
					entity = AffectedEntity.ReadonlyField;
				}
				CheckName(fixedFieldDeclaration, entity, fixedFieldDeclaration.NameToken, GetAccessibiltiy(fixedFieldDeclaration, Modifiers.Private));
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
				base.VisitEventDeclaration(eventDeclaration);
				foreach (var init in eventDeclaration.Variables) {
					CheckName(init, AffectedEntity.Event, init.NameToken, GetAccessibiltiy(eventDeclaration, Modifiers.Private));
				}
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				if (eventDeclaration.Modifiers.HasFlag (Modifiers.Override))
					return;
				base.VisitCustomEventDeclaration(eventDeclaration);
				CheckName(eventDeclaration, AffectedEntity.Event, eventDeclaration.NameToken, GetAccessibiltiy(eventDeclaration, Modifiers.Private));
			}

			public override void VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
			{
				base.VisitEnumMemberDeclaration(enumMemberDeclaration);
				CheckName(enumMemberDeclaration, AffectedEntity.EnumMember, enumMemberDeclaration.NameToken, GetAccessibiltiy(enumMemberDeclaration, Modifiers.Private));
			}

			public override void VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
			{
				base.VisitParameterDeclaration(parameterDeclaration);
				CheckNamedResolveResult(null, parameterDeclaration, parameterDeclaration.Parent is LambdaExpression ? AffectedEntity.LambdaParameter : AffectedEntity.Parameter, parameterDeclaration.NameToken, Modifiers.None);
			}

			public override void VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration)
			{
				base.VisitTypeParameterDeclaration(typeParameterDeclaration);
				CheckNamedResolveResult(null, typeParameterDeclaration, AffectedEntity.TypeParameter, typeParameterDeclaration.NameToken, Modifiers.None);
			}

			public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
			{
				base.VisitVariableDeclarationStatement(variableDeclarationStatement);
				var entity = variableDeclarationStatement.Modifiers.HasFlag(Modifiers.Const) ? AffectedEntity.LocalConstant : AffectedEntity.LocalVariable;
				foreach (var init in variableDeclarationStatement.Variables) {
					CheckNamedResolveResult(null, init, entity, init.NameToken, Modifiers.None);
				}
			}

			public override void VisitLabelStatement(LabelStatement labelStatement)
			{
				base.VisitLabelStatement(labelStatement);
				CheckNamedResolveResult(null, labelStatement, AffectedEntity.Label, labelStatement.LabelToken, Modifiers.None);
			}
		}

	}
}

