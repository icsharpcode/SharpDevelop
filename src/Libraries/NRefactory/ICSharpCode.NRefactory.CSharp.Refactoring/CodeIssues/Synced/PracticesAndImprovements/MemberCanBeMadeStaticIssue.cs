//
// MemberCanBeMadeStaticIssue.cs
//
// Author:
//       Ciprian Khlud <ciprian.mustiata@yahoo.com>
//       Mike krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Ciprian Khlud
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Member can be made static",
	                  Description = "A member doesn't use 'this' object neither explicit nor implicit. It can be made static.",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "MemberCanBeMadeStatic.Local"
	                  )]
	[SubIssueAttribute(CommonSubIssues.PrivateMember)]
	[SubIssueAttribute(CommonSubIssues.NonPrivateMember, false)]
	public class MemberCanBeMadeStaticIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		private class GatherVisitor : GatherVisitorBase<MemberCanBeMadeStaticIssue>
		{
			bool CheckPrivateMember {
				get {
					return SubIssue == CommonSubIssues.PrivateMember;
				}
			}

			public GatherVisitor(BaseRefactoringContext context)
				: base(context)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var rr = ctx.Resolve(typeDeclaration);
				if (rr.Type.GetNonInterfaceBaseTypes().Any(t => t.Name == "MarshalByRefObject" && t.Namespace == "System"))
					return; // ignore MarshalByRefObject, as only instance method calls get marshaled
				
				base.VisitTypeDeclaration(typeDeclaration);
			}

			bool SkipMember(IMember resolved)
			{
				return resolved.Accessibility == Accessibility.Private && !CheckPrivateMember ||
					resolved.Accessibility != Accessibility.Private && CheckPrivateMember;
			}
			
			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var context = ctx;
				if (methodDeclaration.HasModifier(Modifiers.Static) ||
				    methodDeclaration.HasModifier(Modifiers.Virtual) ||
				    methodDeclaration.HasModifier(Modifiers.Override) ||
				    methodDeclaration.HasModifier(Modifiers.New) ||
				    methodDeclaration.Attributes.Any())
					return;

				var body = methodDeclaration.Body;
				// skip empty methods
				if (!body.Statements.Any())
					return;

				if (body.Statements.Count == 1) {
					if (body.Statements.First () is ThrowStatement)
						return;
				}
					
				var resolved = context.Resolve(methodDeclaration) as MemberResolveResult;
				if (resolved == null || SkipMember(resolved.Member))
					return;
				var isImplementingInterface = resolved.Member.ImplementedInterfaceMembers.Any();
				if (isImplementingInterface)
					return;

				if (StaticVisitor.UsesNotStaticMember(context, body))
					return;
				
				AddIssue(new CodeIssue(
					methodDeclaration.NameToken.StartLocation, methodDeclaration.NameToken.EndLocation,
					string.Format(context.TranslateString("Method '{0}' can be made static."), methodDeclaration.Name),
					string.Format(context.TranslateString("Make '{0}' static"), methodDeclaration.Name),
					script => script.ChangeModifier(methodDeclaration, methodDeclaration.Modifiers | Modifiers.Static)) { IssueMarker = IssueMarker.DottedLine });
			}

			static bool IsEmpty(Accessor setter)
			{
				return setter.IsNull || 
					!setter.Body.Statements.Any() ||
					setter.Body.Statements.Count == 1 && setter.Body.Statements.First () is ThrowStatement;
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				if (propertyDeclaration.HasModifier(Modifiers.Static) ||
				    propertyDeclaration.HasModifier(Modifiers.Virtual) ||
				    propertyDeclaration.HasModifier(Modifiers.Override) ||
				    propertyDeclaration.HasModifier(Modifiers.New) ||
				    propertyDeclaration.Attributes.Any())
					return;

				if (IsEmpty(propertyDeclaration.Setter) && IsEmpty(propertyDeclaration.Getter))
					return;


				var resolved = ctx.Resolve(propertyDeclaration) as MemberResolveResult;
				if (resolved == null || SkipMember(resolved.Member))
					return;
				var isImplementingInterface = resolved.Member.ImplementedInterfaceMembers.Any();
				if (isImplementingInterface)
					return;

				if (!propertyDeclaration.Getter.IsNull && StaticVisitor.UsesNotStaticMember(ctx, propertyDeclaration.Getter.Body) ||
				    !propertyDeclaration.Setter.IsNull && StaticVisitor.UsesNotStaticMember(ctx, propertyDeclaration.Setter.Body))
					return;

				AddIssue(new CodeIssue(propertyDeclaration.NameToken.StartLocation, propertyDeclaration.NameToken.EndLocation,
				         string.Format(ctx.TranslateString("Property '{0}' can be made static."), propertyDeclaration.Name),
				         string.Format(ctx.TranslateString("Make '{0}' static"), propertyDeclaration.Name),
					script => script.ChangeModifier(propertyDeclaration, propertyDeclaration.Modifiers | Modifiers.Static)) { IssueMarker = IssueMarker.DottedLine });
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				if (eventDeclaration.HasModifier(Modifiers.Static) ||
				    eventDeclaration.HasModifier(Modifiers.Virtual) ||
				    eventDeclaration.HasModifier(Modifiers.Override) ||
				    eventDeclaration.HasModifier(Modifiers.New) ||
				    eventDeclaration.Attributes.Any())
					return;

				if (IsEmpty(eventDeclaration.RemoveAccessor) && IsEmpty(eventDeclaration.AddAccessor))
					return;


				var resolved = ctx.Resolve(eventDeclaration) as MemberResolveResult;
				if (resolved == null || SkipMember(resolved.Member))
					return;
				var isImplementingInterface = resolved.Member.ImplementedInterfaceMembers.Any();
				if (isImplementingInterface)
					return;

				if (!eventDeclaration.AddAccessor.IsNull && StaticVisitor.UsesNotStaticMember(ctx, eventDeclaration.AddAccessor.Body) ||
				    !eventDeclaration.RemoveAccessor.IsNull && StaticVisitor.UsesNotStaticMember(ctx, eventDeclaration.RemoveAccessor.Body))
					return;

				AddIssue(new CodeIssue(eventDeclaration.NameToken.StartLocation, eventDeclaration.NameToken.EndLocation,
				         string.Format(ctx.TranslateString("Event '{0}' can be made static."), eventDeclaration.Name),
				         string.Format(ctx.TranslateString("Make '{0}' static"), eventDeclaration.Name),
					script => script.ChangeModifier(eventDeclaration, eventDeclaration.Modifiers | Modifiers.Static)) { IssueMarker = IssueMarker.DottedLine });
			}


			static void DoStaticMethodGlobalOperation(AstNode fnode, RefactoringContext fctx, MemberResolveResult rr,
			                                                           Script fscript)
			{
				if (fnode is MemberReferenceExpression) {
					var memberReference = fctx.CreateShortType(rr.Member.DeclaringType).Member(rr.Member.Name);
					fscript.Replace(fnode, memberReference);
				} else {
					var invoke = fnode as InvocationExpression;
					if (invoke == null)
						return;
					if ((invoke.Target is MemberReferenceExpression))
						return;
					var memberReference = fctx.CreateShortType(rr.Member.DeclaringType).Member(rr.Member.Name);
					fscript.Replace(invoke.Target, memberReference);
				}
			}
		}
	}
}

