//
// XmlDocIssue.cs
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
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.NRefactory.CSharp.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Validate Xml documentation",
	                   Description = "Validate Xml docs",
	                   Category = IssueCategories.CompilerWarnings,
	                   Severity = Severity.Warning)]
	public class XmlDocIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<XmlDocIssue>
		{
			readonly List<Comment> storedXmlComment = new List<Comment>();

			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			void InvalideXmlComments()
			{
				if (storedXmlComment.Count == 0)
					return;
				var from = storedXmlComment.First().StartLocation;
				var to = storedXmlComment.Last().EndLocation;
				AddIssue(new CodeIssue(
					from,
					to,
					ctx.TranslateString("Xml comment is not placed before a valid language element"),
					ctx.TranslateString("Remove comment"),
					script => {
						var startOffset = script.GetCurrentOffset(from);
						var endOffset = script.GetCurrentOffset(to);
						endOffset += ctx.GetLineByOffset(endOffset).DelimiterLength;
						script.RemoveText(startOffset, endOffset - startOffset);
					}
				));
				storedXmlComment.Clear();
			}

			public override void VisitComment(Comment comment)
			{
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

			TextLocation TranslateOffset(int offset)
			{
				int line = storedXmlComment.First().StartLocation.Line;
				foreach (var cmt in storedXmlComment) {
					var next = offset - cmt.Content.Length - "\n".Length;
					if (next <= 0)
						return new TextLocation(line, cmt.StartLocation.Column + "///".Length + offset);
					offset = next;
					line++;
				}
				return TextLocation.Empty;
			}

			void AddXmlIssue(int offset, int length, string str)
			{
				var textLocation = TranslateOffset(offset);
				var textLocation2 = TranslateOffset(offset + length);
				if (textLocation.IsEmpty && textLocation2.IsEmpty) {
					AddIssue(new CodeIssue(storedXmlComment.Last(), str));
				} else if (textLocation.IsEmpty) {
					AddIssue(new CodeIssue(storedXmlComment.First().StartLocation, textLocation2, str));
				} else if (textLocation2.IsEmpty) {
					AddIssue(new CodeIssue(textLocation, storedXmlComment.Last().EndLocation, str));
				} else {
					AddIssue(new CodeIssue(textLocation, textLocation2, str));
				}
			}
			readonly StringTextSource emptySource = new StringTextSource("");
			void CheckXmlDoc(AstNode node)
			{
				ResolveResult resolveResult = ctx.Resolve(node);
				IEntity member = null;
				if (resolveResult is TypeResolveResult)
					member = resolveResult.Type.GetDefinition();
				if (resolveResult is MemberResolveResult)
					member = ((MemberResolveResult)resolveResult).Member;
				var xml = new StringBuilder();
				var firstline = "<root>\n";
				xml.Append(firstline);
				foreach (var cmt in storedXmlComment)
					xml.Append(cmt.Content + "\n");
				xml.Append("</root>\n");

				var doc = new AXmlParser().Parse(new StringTextSource(xml.ToString()));

				var stack = new Stack<AXmlObject>();
				stack.Push(doc);
				foreach (var err in doc.SyntaxErrors)
					AddXmlIssue(err.StartOffset - firstline.Length, err.EndOffset - err.StartOffset, err.Description);

				while (stack.Count > 0) {
					var cur = stack.Pop();
					var el = cur as AXmlElement;
					if (el != null) {
						switch (el.Name) {
							case "typeparam":
							case "typeparamref":
								var name = el.Attributes.FirstOrDefault(attr => attr.Name == "name");
								if (name == null)
									break;
								if (member != null && member.SymbolKind == SymbolKind.TypeDefinition) {
									var type = (ITypeDefinition)member;
									if (!type.TypeArguments.Any(arg => arg.Name == name.Value)) {
										AddXmlIssue(name.ValueSegment.Offset - firstline.Length + 1, name.ValueSegment.Length - 2, string.Format(ctx.TranslateString("Type parameter '{0}' not found"), name.Value));
									}
								}
								break;
							case "param":
							case "paramref":
								name = el.Attributes.FirstOrDefault(attr => attr.Name == "name");
								if (name == null)
									break;
								var m = member as IParameterizedMember;
								if (m != null && m.Parameters.Any(p => p.Name == name.Value))
									break;
								if (name.Value == "value" && member != null && (member.SymbolKind == SymbolKind.Property || member.SymbolKind == SymbolKind.Indexer || member.SymbolKind == SymbolKind.Event) && el.Name == "paramref")
									break;
								AddXmlIssue(name.ValueSegment.Offset - firstline.Length + 1, name.ValueSegment.Length - 2, string.Format(ctx.TranslateString("Parameter '{0}' not found"), name.Value));
								break;
							case "exception":
							case "seealso":
							case "see":
								var cref = el.Attributes.FirstOrDefault(attr => attr.Name == "cref");
								if (cref == null)
									break;
								try {
									var trctx = ctx.Resolver.TypeResolveContext;
									if (member is IMember)
										trctx = trctx.WithCurrentTypeDefinition(member.DeclaringTypeDefinition).WithCurrentMember((IMember)member);
									if (member is ITypeDefinition)
										trctx = trctx.WithCurrentTypeDefinition((ITypeDefinition)member);
									var state = ctx.Resolver.GetResolverStateBefore(node);
									if (state.CurrentUsingScope != null)
										trctx = trctx.WithUsingScope(state.CurrentUsingScope);
									var cdc = new CSharpDocumentationComment (emptySource, trctx);
									var entity = cdc.ResolveCref(cref.Value);

									if (entity == null) {
										AddXmlIssue(cref.ValueSegment.Offset - firstline.Length + 1, cref.ValueSegment.Length - 2, string.Format(ctx.TranslateString("Cannot find reference '{0}'"), cref.Value));
									}
								} catch (Exception e) {
									AddXmlIssue(cref.ValueSegment.Offset - firstline.Length + 1, cref.ValueSegment.Length - 2, string.Format(ctx.TranslateString("Reference parsing error '{0}'."), e.Message));
								}
								break;

						}
					}
					foreach (var child in cur.Children)
						stack.Push(child);
				}
				storedXmlComment.Clear();
			}

			protected virtual void VisitXmlChildren(AstNode node, Action checkDocumentationAction)
			{
				AstNode next;
				var child = node.FirstChild;
				while (child != null && (child is Comment || child.Role == Roles.NewLine)) {
					next = child.NextSibling;
					child.AcceptVisitor(this);
					child = next;
				}
				checkDocumentationAction();

				for (; child != null; child = next) {
					// Store next to allow the loop to continue
					// if the visitor removes/replaces child.
					next = child.NextSibling;
					child.AcceptVisitor(this);
				}
				InvalideXmlComments();
			}

			protected virtual void VisitXmlChildren(AstNode node)
			{
				VisitXmlChildren(node, () => CheckXmlDoc(node));
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
				VisitXmlChildren(eventDeclaration, () => {
					foreach (var e in eventDeclaration.Variables) {
						CheckXmlDoc(e);
					}
				});
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				VisitXmlChildren(fieldDeclaration, () => {
					foreach (var e in fieldDeclaration.Variables) {
						CheckXmlDoc(e);
					}
				});
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
