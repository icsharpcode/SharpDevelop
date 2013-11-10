// 
// GatherVisitorBase.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.CSharp;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp
{
	class GatherVisitorConstants
	{
		public const string DisableAllString = "disable All";
		public const string RestoreAllString = "restore All";

	}

	public interface IGatherVisitor
	{
		BaseRefactoringContext Ctx { get; }
		string SubIssue { get; set; }
		IEnumerable<CodeIssue> GetIssues();
	}

	/// <summary>
	/// The code issue provider gets a list of all code issues in a syntax tree.
	/// </summary>
	public abstract class GatherVisitorCodeIssueProvider : CodeIssueProvider
	{
		protected abstract IGatherVisitor CreateVisitor (BaseRefactoringContext context);

		public sealed override IEnumerable<CodeIssue> GetIssues (BaseRefactoringContext context, string subIssue = null)
		{
			var visitor = CreateVisitor(context);
			if (visitor == null)
				return Enumerable.Empty<CodeIssue> ();
			visitor.SubIssue = subIssue;
			return visitor.GetIssues();
		}
	}

	/// <summary>
	/// A base class for writing issue provider visitor implementations.
	/// </summary>
	class GatherVisitorBase<T> : DepthFirstAstVisitor, IGatherVisitor where T : CodeIssueProvider
	{
		/// <summary>
		/// The issue provider. May be <c>null</c> if none was specified.
		/// </summary>
		protected readonly T issueProvider;
		protected readonly BaseRefactoringContext ctx;

		public BaseRefactoringContext Ctx {
			get {
				return ctx;
			}
		}

		bool isAllDisabled;
		bool isDisabled;
		bool isDisabledOnce;
		bool isGloballySuppressed;
		bool isPragmaDisabled;
		List<DomRegion> suppressedRegions = new List<DomRegion>();
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string disableString;
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string disableOnceString;
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string restoreString;
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string suppressMessageCategory;
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string suppressMessageCheckId;
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static int pragmaWarning;

		static void SetDisableKeyword(string disableKeyword)
		{
			disableString = "disable " + disableKeyword;
			disableOnceString = "disable once " + disableKeyword;
			restoreString = "restore " + disableKeyword;
		}

		public readonly List<CodeIssue> FoundIssues = new List<CodeIssue>();

		public string SubIssue { get; set; }

		static GatherVisitorBase()
		{
			var attr = (IssueDescriptionAttribute)typeof(T).GetCustomAttributes(false).FirstOrDefault(a => a is IssueDescriptionAttribute);
			if (attr == null)
				return;
			if (attr.AnalysisDisableKeyword != null) 
				SetDisableKeyword(attr.AnalysisDisableKeyword);
			suppressMessageCheckId = attr.SuppressMessageCheckId;
			suppressMessageCategory = attr.SuppressMessageCategory;
			pragmaWarning = attr.PragmaWarning;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GatherVisitorBase{T}"/> class.
		/// </summary>
		/// <param name='ctx'>
		/// The refactoring context.
		/// </param>
		/// <param name='qualifierDirectiveEvidentIssueProvider'>
		/// The issue provider.
		/// </param>
		public GatherVisitorBase(BaseRefactoringContext ctx, T qualifierDirectiveEvidentIssueProvider = default(T))
		{
			this.ctx = ctx;
			this.issueProvider = qualifierDirectiveEvidentIssueProvider;
			if (suppressMessageCheckId != null) {
				foreach (var attr in this.ctx.Compilation.MainAssembly.AssemblyAttributes) {
					if (attr.AttributeType.Name == "SuppressMessageAttribute" && attr.AttributeType.Namespace == "System.Diagnostics.CodeAnalysis") {
						if (attr.PositionalArguments.Count < 2)
							return;
						var category = attr.PositionalArguments [0].ConstantValue;
						if (category == null || category.ToString() != suppressMessageCategory)
							continue;
						var checkId = attr.PositionalArguments [1].ConstantValue;
						if (checkId == null || checkId.ToString() != suppressMessageCheckId) 
							continue;
						isGloballySuppressed = true;
					}
				}
			}
		}

		/// <summary>
		/// Gets all the issues using the context root node as base.
		/// </summary>
		/// <returns>
		/// The issues.
		/// </returns>
		public IEnumerable<CodeIssue> GetIssues()
		{
			ctx.RootNode.AcceptVisitor(this);
			return FoundIssues;
		}

		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			if (ctx.CancellationToken.IsCancellationRequested)
				return;
			base.VisitTypeDeclaration(typeDeclaration);
		}

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			if (ctx.CancellationToken.IsCancellationRequested)
				return;
			base.VisitMethodDeclaration(methodDeclaration);
		}

		public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (ctx.CancellationToken.IsCancellationRequested)
				return;
			base.VisitPropertyDeclaration(propertyDeclaration);
		}

		public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			if (ctx.CancellationToken.IsCancellationRequested)
				return;
			base.VisitFieldDeclaration(fieldDeclaration);
		}

		public override void VisitComment(Comment comment)
		{
			if (comment.CommentType == CommentType.SingleLine) {
				var txt = comment.Content;
				if (string.IsNullOrEmpty(txt))
					return;
				if (isAllDisabled) {
					isAllDisabled &= txt.IndexOf(GatherVisitorConstants.RestoreAllString, StringComparison.InvariantCultureIgnoreCase) < 0;
				} else {
					isAllDisabled |= txt.IndexOf(GatherVisitorConstants.DisableAllString, StringComparison.InvariantCultureIgnoreCase) > 0;
				}

				if (restoreString != null) {
					if (isDisabled) {
						isDisabled &= txt.IndexOf(restoreString, StringComparison.InvariantCulture) < 0;
					} else {
						isDisabled |= txt.IndexOf(disableString, StringComparison.InvariantCulture) > 0;
						isDisabledOnce |= txt.IndexOf(disableOnceString, StringComparison.InvariantCulture) > 0;
					}
				}
			}
		}

		public override void VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
		{
			if (pragmaWarning == 0)
				return;

			var warning = preProcessorDirective as PragmaWarningPreprocessorDirective;
			if (warning == null)
				return;
			if (warning.IsDefined(pragmaWarning))
				isPragmaDisabled = warning.Disable;
		}

		public override void VisitAttribute(Attribute attribute)
		{
			base.VisitAttribute(attribute);
			if (suppressMessageCheckId == null)
				return;
			var resolveResult = ctx.Resolve(attribute);
			if (resolveResult.Type.Name == "SuppressMessageAttribute" && resolveResult.Type.Namespace == "System.Diagnostics.CodeAnalysis") {
				if (attribute.Arguments.Count < 2)
					return;
				var category = attribute.Arguments.First() as PrimitiveExpression;
				if (category == null || category.Value.ToString() != suppressMessageCategory)
					return;
				var checkId = attribute.Arguments.Skip(1).First() as PrimitiveExpression;
				if (checkId == null || checkId.Value.ToString() != suppressMessageCheckId) 
					return;
				suppressedRegions.Add(attribute.Parent.Parent.Region);
			}
		}

		protected bool IsSuppressed(TextLocation location)
		{
			if (isAllDisabled)
				return true;
			if (isDisabledOnce) {
				isDisabledOnce = false;
				return true;
			}
			return isDisabled || isGloballySuppressed || isPragmaDisabled || suppressedRegions.Any(r => r.IsInside(location));
		}

		protected void AddIssue(CodeIssue issue)
		{
			if (IsSuppressed(issue.Start))
				return;
			FoundIssues.Add(issue);
		}
	}
}
