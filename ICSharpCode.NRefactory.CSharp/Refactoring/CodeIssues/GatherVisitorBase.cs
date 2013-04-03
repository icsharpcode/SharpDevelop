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

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// A base class for writing issue provider visitor implementations.
	/// </summary>
	class GatherVisitorBase<T> : DepthFirstAstVisitor where T : ICodeIssueProvider
	{
		/// <summary>
		/// The issue provider. May be <c>null</c> if none was specified.
		/// </summary>
		protected readonly T IssueProvider;

		protected readonly BaseRefactoringContext ctx;
		bool isDisabled;
		bool isGloballySuppressed;
		List<DomRegion> suppressedRegions =new List<DomRegion> ();

		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string disableString;

		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string restoreString;

		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string suppressMessageCategory;

		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static string suppressMessageCheckId;

		static void SetDisableKeyword(string disableKeyword)
		{
			disableString = "disable " + disableKeyword;
			restoreString = "restore " + disableKeyword;
		}

		public readonly List<CodeIssue> FoundIssues = new List<CodeIssue> ();

		static GatherVisitorBase()
		{
			var attr = (IssueDescriptionAttribute)typeof(T).GetCustomAttributes(false).FirstOrDefault(a => a is IssueDescriptionAttribute);
			if (attr == null)
				return;
			if (attr.ResharperDisableKeyword != null) 
				SetDisableKeyword(attr.ResharperDisableKeyword);
			suppressMessageCheckId  = attr.SuppressMessageCheckId;
			suppressMessageCategory = attr.SuppressMessageCategory;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCode.NRefactory.CSharp.GatherVisitorBase"/> class.
		/// </summary>
		/// <param name='ctx'>
		/// The refactoring context.
		/// </param>
		/// <param name='issueProvider'>
		/// The issue provider.
		/// </param>
		public GatherVisitorBase (BaseRefactoringContext ctx, T issueProvider = default(T))
		{
			this.ctx = ctx;
			this.IssueProvider = issueProvider;
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

		protected override void VisitChildren (AstNode node)
		{
			if (ctx.CancellationToken.IsCancellationRequested || isGloballySuppressed)
				return;
			base.VisitChildren (node);
		}

		public override void VisitComment(Comment comment)
		{
			if (comment.CommentType == CommentType.SingleLine && restoreString != null) {
				var txt = comment.Content;
				if (string.IsNullOrEmpty(txt))
					return;
				if (isDisabled) {
					isDisabled &= txt.IndexOf(restoreString, StringComparison.InvariantCulture) < 0;
				} else {
					isDisabled |= txt.IndexOf(disableString, StringComparison.InvariantCulture) > 0;
				}
			}
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
				var category = attribute.Arguments.First () as PrimitiveExpression;
				if (category == null || category.Value.ToString () != suppressMessageCategory)
					return;
				var checkId = attribute.Arguments.Skip (1).First () as PrimitiveExpression;
				if (checkId == null || checkId.Value.ToString() != suppressMessageCheckId) 
					return;
				suppressedRegions.Add (attribute.Parent.Parent.Region);
			}
		}

		protected bool IsSuppressed(TextLocation location)
		{
			return isDisabled || isGloballySuppressed || suppressedRegions.Any(r => r.IsInside(location));
		}

		protected void AddIssue(AstNode node, string title, System.Action<Script> fix = null)
		{
			if (IsSuppressed(node.StartLocation))
				return;
			FoundIssues.Add(new CodeIssue (title, node.StartLocation, node.EndLocation, fix != null ? new CodeAction (title, fix, node) : null));
		}

		protected void AddIssue(TextLocation start, TextLocation end, string title, System.Action<Script> fix = null)
		{
			if (IsSuppressed(start))
				return;
			FoundIssues.Add(new CodeIssue(title, start, end, fix != null ? new CodeAction(title, fix, start, end) : null));
		}

		protected void AddIssue(AstNode node, string title, CodeAction fix)
		{
			if (IsSuppressed(node.StartLocation))
				return;
			FoundIssues.Add(new CodeIssue(title, node.StartLocation, node.EndLocation, fix));
		}

		protected void AddIssue(TextLocation start, TextLocation end, string title, CodeAction fix)
		{
			if (IsSuppressed(start))
				return;
			FoundIssues.Add(new CodeIssue (title, start, end, fix));
		}
		
		protected void AddIssue(AstNode node, string title, IEnumerable<CodeAction> fixes)
		{
			if (IsSuppressed(node.StartLocation))
				return;
			FoundIssues.Add(new CodeIssue(title, node.StartLocation, node.EndLocation, fixes));
		}

		protected void AddIssue(TextLocation start, TextLocation end, string title, IEnumerable<CodeAction> fixes)
		{
			if (IsSuppressed(start))
				return;
			FoundIssues.Add(new CodeIssue (title, start, end, fixes));
		}
	}
}
