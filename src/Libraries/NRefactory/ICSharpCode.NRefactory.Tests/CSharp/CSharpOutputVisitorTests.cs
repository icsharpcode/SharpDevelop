// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp
{
	[TestFixture]
	public class CSharpOutputVisitorTests
	{
		void AssertOutput(string expected, AstNode node, CSharpFormattingOptions policy = null)
		{
			if (policy == null)
				policy = FormattingOptionsFactory.CreateMono();
			StringWriter w = new StringWriter();
			w.NewLine = "\n";
			node.AcceptVisitor(new CSharpOutputVisitor(new TextWriterTokenWriter(w) { IndentationString = "$" }, policy));
			Assert.AreEqual(expected.Replace("\r", ""), w.ToString());
		}
		
		[Test]
		public void AnonymousLocalVariableDeclaration()
		{
			var code = @"class Test
{
	void Foo ()
	{
		Action<int> act = delegate (int testMe) {
		};
	}
}
";
			var unit = SyntaxTree.Parse(code);
			AssertOutput("class Test\n{\n$void Foo ()\n${\n$$Action<int> act = delegate (int testMe) {\n$$};\n$}\n}\n", unit);
		}
		
		[Test]
		public void AssignmentInCollectionInitializer()
		{
			Expression expr = new ObjectCreateExpression {
				Type = new SimpleType("List"),
				Initializer = new ArrayInitializerExpression(
					new ArrayInitializerExpression(
						new AssignmentExpression(new IdentifierExpression("a"), new PrimitiveExpression(1))
					)
				)
			};
			
			AssertOutput("new List {\n${\n$$a = 1\n$}\n}", expr);
		}
		
		[Test]
		public void EnumDeclarationWithInitializers ()
		{
			TypeDeclaration type = new TypeDeclaration {
				ClassType = ClassType.Enum,
				Name = "DisplayFlags",
				Members = {
					new EnumMemberDeclaration {
						Name = "D",
						Initializer = new PrimitiveExpression(4)
					}
				}};
			
			AssertOutput("enum DisplayFlags\n{\n$D = 4\n}\n", type);
		}
		
		[Test]
		public void InlineCommentAtEndOfCondition()
		{
			IfElseStatement condition = new IfElseStatement();
			condition.AddChild(new CSharpTokenNode(new TextLocation(1, 1), IfElseStatement.IfKeywordRole), IfElseStatement.IfKeywordRole);
			condition.AddChild(new CSharpTokenNode(new TextLocation(1, 4), Roles.LPar), Roles.LPar);
			condition.AddChild(new IdentifierExpression("cond", new TextLocation(1, 5)), IfElseStatement.ConditionRole);
			condition.AddChild(new Comment(CommentType.MultiLine, new TextLocation(1, 9), new TextLocation(1, 14)) { Content = "a" }, Roles.Comment);
			condition.AddChild(new CSharpTokenNode(new TextLocation(1, 14), Roles.RPar), Roles.RPar);
			condition.AddChild(new ReturnStatement(), IfElseStatement.TrueRole);
			
			AssertOutput("if (cond/*a*/)\n$return;\n", condition);
		}
		
		[Test]
		public void SwitchStatement()
		{
			SwitchStatement type = new SwitchStatement {
				Expression = new IdentifierExpression("i"),
				SwitchSections = {
					new SwitchSection {
						CaseLabels = {
							new CaseLabel(new PrimitiveExpression(1)),
							new CaseLabel(new PrimitiveExpression(2))
						},
						Statements = {
							new ExpressionStatement(new IdentifierExpression("A").Invoke()),
							new ExpressionStatement(new IdentifierExpression("B").Invoke()),
							new BreakStatement()
						}
					},
					new SwitchSection {
						CaseLabels = {
							new CaseLabel(new PrimitiveExpression(3))
						},
						Statements = {
							new BlockStatement {
								new VariableDeclarationStatement(new PrimitiveType("int"), "a", new PrimitiveExpression(3)),
								new ReturnStatement(new IdentifierExpression("a")),
							}
						}
					},
					new SwitchSection {
						CaseLabels = {
							new CaseLabel()
						},
						Statements = {
							new BreakStatement()
						}
					}
				}};
			
			AssertOutput("switch (i) {\ncase 1:\ncase 2:\n$A ();\n$B ();\n$break;\n" +
			             "case 3: {\n$int a = 3;\n$return a;\n}\n" +
			             "default:\n$break;\n}\n", type);
		}
		
		[Test]
		public void ZeroLiterals()
		{
			AssertOutput("0.0", new PrimitiveExpression(0.0));
			AssertOutput("-0.0", new PrimitiveExpression(-0.0));
			AssertOutput("0f", new PrimitiveExpression(0f));
			AssertOutput("-0f", new PrimitiveExpression(-0f));
		}

		[Test]
		public void BlankLinesBetweenMembers()
		{
			var code = @"class Test
{
	Test() {}
	void Foo() {}
	int P { get; set; }
}
";
			var unit = SyntaxTree.Parse(code);
			var options = FormattingOptionsFactory.CreateMono();
			options.MinimumBlankLinesBetweenMembers = 2;
			AssertOutput("class Test\n{\n$Test ()\n${\n$}\n\n\n$void Foo ()\n${\n$}\n\n\n$int P {\n$$get;\n$$set;\n$}\n}\n", unit, options);
		}

		[Test]
		public void BlankLinesAfterUsings() {
			var code = @"using System;
using System.Collections;
using List = System.Collections.List;
namespace NS
{
	using System.Collections.Generic;
	using Collection = System.Collections.Collection;
	using System.Xml;
	class C {}
}
";
			var unit = SyntaxTree.Parse(code);
			var options = FormattingOptionsFactory.CreateMono();
			options.MinimumBlankLinesAfterUsings = 2;
			AssertOutput("using System;\nusing System.Collections;\nusing List = System.Collections.List;\n\n\nnamespace NS\n{\n$using System.Collections.Generic;\n$using Collection = System.Collections.Collection;\n$using System.Xml;\n\n\n$class C\n${\n$}\n}\n", unit, options);
		}
		
		[Test, Ignore("#pragma warning not implemented in output visitor - issue #188")]
		public void PragmaWarning()
		{
			var code = @"#pragma warning disable 414";
			var unit = SyntaxTree.Parse(code);
			var options = FormattingOptionsFactory.CreateMono();
			AssertOutput("#pragma warning disable 414\n", unit, options);
		}
	}
}
