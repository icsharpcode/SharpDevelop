//
// CS1573ParameterHasNoMatchingParamTagIssue.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ICSharpCode.NRefactory.Documentation;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Parameter has no matching param tag in the XML comment",
	                  Description = "Parameter has no matching param tag in the XML comment",
	                  Category = IssueCategories.CompilerWarnings,
	                  Severity = Severity.Warning,
	                  PragmaWarning = 1573,
	                  AnalysisDisableKeyword = "CSharpWarnings::CS1573")]
	public class CS1573ParameterHasNoMatchingParamTagIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS1573ParameterHasNoMatchingParamTagIssue>
		{
			readonly List<Comment> storedXmlComment = new List<Comment>();

			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			void InvalideXmlComments()
			{
				storedXmlComment.Clear();
			}

			public override void VisitComment(Comment comment)
			{
				base.VisitComment(comment);
				if (comment.CommentType == CommentType.Documentation)
					storedXmlComment.Add(comment);
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				InvalideXmlComments();
				base.VisitNamespaceDeclaration(namespaceDeclaration);
			}

			public override void VisitUsingDeclaration(UsingDeclaration usingDeclaration)
			{
				InvalideXmlComments();
				base.VisitUsingDeclaration(usingDeclaration);
			}

			public override void VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration)
			{
				InvalideXmlComments();
				base.VisitUsingAliasDeclaration(usingDeclaration);
			}

			public override void VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration)
			{
				InvalideXmlComments();
				base.VisitExternAliasDeclaration(externAliasDeclaration);
			}

			void AddXmlIssue(int line, int col, int length, string str)
			{
				var cmt = storedXmlComment [Math.Max(0, Math.Min(storedXmlComment.Count - 1, line))];

				AddIssue(new CodeIssue(new TextLocation(cmt.StartLocation.Line, cmt.StartLocation.Column + 3 + col),
				         new TextLocation(cmt.StartLocation.Line, cmt.StartLocation.Column + 3 + col + length),
					str));
			}

			int SearchAttributeColumn(int x, int line)
			{
				var comment = storedXmlComment [Math.Max(0, Math.Min(storedXmlComment.Count - 1, line))];
				var idx = comment.Content.IndexOfAny(new char[] { '"', '\'' }, x);
				return idx < 0 ? x : idx + 1;
			}

			void CheckXmlDoc(AstNode node)
			{
				ResolveResult resolveResult = ctx.Resolve(node);
				IEntity member = null;
				if (resolveResult is TypeResolveResult)
					member = resolveResult.Type.GetDefinition();
				if (resolveResult is MemberResolveResult)
					member = ((MemberResolveResult)resolveResult).Member;
				var xml = new StringBuilder();
				xml.AppendLine("<root>");
				foreach (var cmt in storedXmlComment)
					xml.AppendLine(cmt.Content);
				xml.AppendLine("</root>");

				List<Tuple<string, int>> parameters = new List<Tuple<string, int>>();

				using (var reader = new XmlTextReader(new StringReader(xml.ToString()))) {
					reader.XmlResolver = null;
					try {
						while (reader.Read()) {
							if (member == null)
								continue;
							if (reader.NodeType == XmlNodeType.Element) {
								switch (reader.Name) {
									case "param":
										reader.MoveToFirstAttribute();
										var line = reader.LineNumber;
										var name = reader.GetAttribute("name");
										if (name == null)
											break;
										parameters.Add(Tuple.Create(name, line));
										break;

								}
							}
						}
					} catch (XmlException) {
					}

					if (storedXmlComment.Count > 0 && parameters.Count > 0) {
						var pm = member as IParameterizedMember;
						if (pm != null) {
							for (int i = 0; i < pm.Parameters.Count; i++) {
								var p = pm.Parameters [i];
								if (!parameters.Any(tp => tp.Item1 == p.Name)) {
									AstNode before = i < parameters.Count ? storedXmlComment [parameters [i].Item2 - 2] : null;
									AstNode afterNode = before == null ? storedXmlComment [storedXmlComment.Count - 1] : null;
									AddIssue(new CodeIssue(
										GetParameterHighlightNode(node, i),
										string.Format(ctx.TranslateString("Missing xml documentation for Parameter '{0}'"), p.Name),
										string.Format(ctx.TranslateString("Create xml documentation for Parameter '{0}'"), p.Name),
										script => {
										if (before != null) {
											script.InsertBefore(
												before, 
												new Comment(string.Format(" <param name = \"{0}\"></param>", p.Name), CommentType.Documentation)
												);
										} else {
											script.InsertAfter(
												afterNode, 
												new Comment(string.Format(" <param name = \"{0}\"></param>", p.Name), CommentType.Documentation)
												);
										}
										}));
								}
							}

						}
					}
					storedXmlComment.Clear();
				}
			}

			AstNode GetParameterHighlightNode(AstNode node, int i)
			{
				if (node is MethodDeclaration)
					return ((MethodDeclaration)node).Parameters.ElementAt(i).NameToken;
				if (node is ConstructorDeclaration)
					return ((ConstructorDeclaration)node).Parameters.ElementAt(i).NameToken;
				if (node is OperatorDeclaration)
					return ((OperatorDeclaration)node).Parameters.ElementAt(i).NameToken;
				if (node is IndexerDeclaration)
					return ((IndexerDeclaration)node).Parameters.ElementAt(i).NameToken;
				throw new InvalidOperationException("invalid parameterized node:" + node);
			}

			protected virtual void VisitXmlChildren(AstNode node)
			{
				AstNode next;
				var child = node.FirstChild;
				while (child != null && (child is Comment || child is PreProcessorDirective || child.Role == Roles.NewLine)) {
					next = child.NextSibling;
					child.AcceptVisitor(this);
					child = next;
				}

				CheckXmlDoc(node);

				for (; child != null; child = next) {
					// Store next to allow the loop to continue
					// if the visitor removes/replaces child.
					next = child.NextSibling;
					child.AcceptVisitor(this);
				}
				InvalideXmlComments();
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				VisitXmlChildren(typeDeclaration);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				VisitXmlChildren(methodDeclaration);
			}

			public override void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
			{
				VisitXmlChildren(delegateDeclaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				VisitXmlChildren(constructorDeclaration);
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				VisitXmlChildren(eventDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				VisitXmlChildren(destructorDeclaration);
			}

			public override void VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
			{
				VisitXmlChildren(enumMemberDeclaration);
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
				VisitXmlChildren(eventDeclaration);
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				VisitXmlChildren(fieldDeclaration);
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				VisitXmlChildren(indexerDeclaration);
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				VisitXmlChildren(propertyDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				VisitXmlChildren(operatorDeclaration);
			}
		}
	}
}

