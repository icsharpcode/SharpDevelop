//
// NullValueAnalysisTests.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.CSharp.Analysis
{
	[TestFixture]
	public class NullValueAnalysisTests
	{
		class StubbedRefactoringContext : BaseRefactoringContext
		{
			bool supportsVersion5;

			public override string DefaultNamespace {
				get {
					return string.Empty;
				}
			}

			StubbedRefactoringContext(CSharpAstResolver resolver, bool supportsVersion5) :
				base(resolver, CancellationToken.None) {
				this.supportsVersion5 = supportsVersion5;
			}

			internal static StubbedRefactoringContext Create(SyntaxTree tree, bool supportsVersion5 = true)
			{
				IProjectContent pc = new CSharpProjectContent();
				pc = pc.AddAssemblyReferences(CecilLoaderTests.Mscorlib);
				pc = pc.AddOrUpdateFiles(new[] {
					tree.ToTypeSystem()
				});
				var compilation = pc.CreateCompilation();
				var resolver = new CSharpAstResolver(compilation, tree);

				return new StubbedRefactoringContext(resolver, supportsVersion5);
			}

			#region implemented abstract members of BaseRefactoringContext
			public override int GetOffset(TextLocation location)
			{
				throw new NotImplementedException();
			}
			public override ICSharpCode.NRefactory.Editor.IDocumentLine GetLineByOffset(int offset)
			{
				throw new NotImplementedException();
			}
			public override TextLocation GetLocation(int offset)
			{
				throw new NotImplementedException();
			}
			public override string GetText(int offset, int length)
			{
				throw new NotImplementedException();
			}
			public override string GetText(ICSharpCode.NRefactory.Editor.ISegment segment)
			{
				throw new NotImplementedException();
			}
			#endregion


			public override bool Supports(Version version)
			{
				if (supportsVersion5)
					return version.Major <= 5;
				return version.Major <= 4;
			}
		}

		static NullValueAnalysis CreateNullValueAnalysis(SyntaxTree tree, MethodDeclaration methodDeclaration, bool supportsCSharp5 = true)
		{
			var ctx = StubbedRefactoringContext.Create(tree, supportsCSharp5);
			var analysis =  new NullValueAnalysis(ctx, methodDeclaration, CancellationToken.None) {
				IsParametersAreUninitialized = true
			};
			analysis.Analyze();
			return analysis;
		}

		static NullValueAnalysis CreateNullValueAnalysis(MethodDeclaration methodDeclaration)
		{
			var type = new TypeDeclaration {
				Name = "DummyClass",
				ClassType = ClassType.Class
			};
			type.Members.Add(methodDeclaration);
			var tree = new SyntaxTree { FileName = "test.cs" };
			tree.Members.Add(type);

			return CreateNullValueAnalysis(tree, methodDeclaration);
		}

		static ParameterDeclaration CreatePrimitiveParameter(string typeKeyword, string parameterName)
		{
			return new ParameterDeclaration(new PrimitiveType(typeKeyword), parameterName);
		}

		static ParameterDeclaration CreateStringParameter(string parameterName = "p")
		{
			return CreatePrimitiveParameter("string", parameterName);
		}

		[Test]
		public void TestSimple()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new ExpressionStatement(new AssignmentExpression(
						new IdentifierExpression("p"), new NullReferenceExpression())),
					new ExpressionStatement(new AssignmentExpression(
						new IdentifierExpression("p"), new PrimitiveExpression("Hello"))),
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter());

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = method.Body.Statements.First();
			var stmt2 = method.Body.Statements.ElementAt(1);
			
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p"));
		}

		[Test]
		public void TestIfStatement()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new IfElseStatement {
						Condition = new BinaryOperatorExpression(new IdentifierExpression("p"),
						                                         BinaryOperatorType.Equality,
						                                         new NullReferenceExpression()),
						TrueStatement = new ExpressionStatement(new AssignmentExpression(
							new IdentifierExpression("p"),
							new PrimitiveExpression("Hello")))
					},
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter());

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = (IfElseStatement)method.Body.Statements.First();
			var stmt2 = (ExpressionStatement)stmt1.TrueStatement;
			var stmt3 = (ReturnStatement)method.Body.Statements.ElementAt(1);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p"));
		}

		[Test]
		public void TestEndlessLoop()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new VariableDeclarationStatement(new PrimitiveType("string"),
					                                 "p2", new NullReferenceExpression()),
					new WhileStatement {
						Condition = new BinaryOperatorExpression(new IdentifierExpression("p1"),
						                                 BinaryOperatorType.Equality,
						                                 new NullReferenceExpression()),
						EmbeddedStatement = new ExpressionStatement(
							new AssignmentExpression(new IdentifierExpression("p2"),
						                         AssignmentOperatorType.Assign,
						                         new PrimitiveExpression("")))
					},
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter("p1"));

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = (WhileStatement)method.Body.Statements.ElementAt(1);
			var stmt2 = (ExpressionStatement)stmt1.EmbeddedStatement;
			var stmt3 = (ReturnStatement)method.Body.Statements.ElementAt(2);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p2"));
		}

		[Test]
		public void TestLoop()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new VariableDeclarationStatement(new PrimitiveType("string"),
					                                 "p2", new NullReferenceExpression()),
					new WhileStatement {
						Condition = new BinaryOperatorExpression(new IdentifierExpression("p1"),
						                                         BinaryOperatorType.Equality,
						                                         new NullReferenceExpression()),
						EmbeddedStatement = new BlockStatement {
							new ExpressionStatement(
								new AssignmentExpression(new IdentifierExpression("p2"),
						                         AssignmentOperatorType.Assign,
						                         new PrimitiveExpression(""))),
							new ExpressionStatement(
								new AssignmentExpression(new IdentifierExpression("p1"),
							                         AssignmentOperatorType.Assign,
							                         new PrimitiveExpression("")))
						}
					},
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter("p1"));

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = (WhileStatement)method.Body.Statements.ElementAt(1);
			var stmt2 = (ExpressionStatement)((BlockStatement)stmt1.EmbeddedStatement).Statements.Last();
			var stmt3 = (ReturnStatement)method.Body.Statements.ElementAt(2);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p2"));
		}

		ExpressionStatement MakeStatement(Expression expr)
		{
			return new ExpressionStatement(expr);
		}

		[Test]
		public void TestForLoop()
		{
			var forStatement = new ForStatement();
			forStatement.Initializers.Add(MakeStatement(new AssignmentExpression(new IdentifierExpression("p2"),
			                                                                     new PrimitiveExpression(""))));
			forStatement.Condition = new BinaryOperatorExpression(new IdentifierExpression("p1"),
			                                                      BinaryOperatorType.Equality,
			                                                      new NullReferenceExpression());
			forStatement.Iterators.Add(MakeStatement(new AssignmentExpression(new IdentifierExpression("p2"),
			                                                                  AssignmentOperatorType.Assign,
			                                                                  new NullReferenceExpression())));
			forStatement.EmbeddedStatement = MakeStatement(new AssignmentExpression(new IdentifierExpression("p1"),
			                                                                        AssignmentOperatorType.Assign,
			                                                                        new PrimitiveExpression("")));
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					forStatement,
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter("p1"));
			method.Parameters.Add(CreateStringParameter("p2"));

			var returnStatement = (ReturnStatement)method.Body.Statements.Last();
			var content = forStatement.EmbeddedStatement;

			var analysis = CreateNullValueAnalysis(method);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(forStatement, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(forStatement, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(content, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(content, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(content, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(returnStatement, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(returnStatement, "p2"));
		}

		[Test]
		public void TestNullCoallescing()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new ExpressionStatement(new AssignmentExpression(new IdentifierExpression("p1"),
					                                                 new BinaryOperatorExpression(new IdentifierExpression("p1"),
					                             BinaryOperatorType.NullCoalescing,
					                             new PrimitiveExpression(""))))
				}
			};

			method.Parameters.Add(CreateStringParameter("p1"));

			var analysis = CreateNullValueAnalysis(method);
			var stmt = method.Body.Statements.Single();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt, "p1"));
		}

		[Test]
		public void TestCapturedLambdaVariables()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new VariableDeclarationStatement(AstType.Create("System.Action"),
					                                 "action",
					                                 new LambdaExpression {
						Body = new BlockStatement {
							MakeStatement(new AssignmentExpression(new IdentifierExpression("p1"),
							                                       new NullReferenceExpression()))
						}
					}),
					MakeStatement(new AssignmentExpression(new IdentifierExpression("p1"),
					                                       new NullReferenceExpression())),
					new ExpressionStatement(new InvocationExpression(new IdentifierExpression("action"))),
					MakeStatement(new AssignmentExpression(new IdentifierExpression("p3"),
					                                       new IdentifierExpression("p1")))
				}
			};

			method.Parameters.Add(CreateStringParameter("p1"));
			method.Parameters.Add(CreateStringParameter("p2"));
			method.Parameters.Add(CreateStringParameter("p3"));

			var analysis = CreateNullValueAnalysis(method);
			var declareLambda = (VariableDeclarationStatement)method.Body.Statements.First();
			var lastStatement = (ExpressionStatement)method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(declareLambda, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(declareLambda, "p2"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusBeforeStatement(lastStatement, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(lastStatement, "p2"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(lastStatement, "p3"));
		}

		[Test]
		public void TestVariableVisitsCount()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		string s = null;
		while (s == null) {
			string s2 = null;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			Assert.AreEqual(8, analysis.NodeVisits);
		}


		[Test]
		public void TestInvocation()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
delegate void MyDelegate(string p1, out string p2);
class TestClass
{
	void TestMethod()
	{
		string p1 = null;
		string p2 = null;
		MyDelegate del = (string a, out string b) => { b = a; };
		del(p1 = """", out p2);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ExpressionStatement)method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(lastStatement, "p2"));
		}

		[Test]
		public void TestLock()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		object o = null;
		lock (o) {
			//Impossible
			int x1 = 1;
		}
		//Impossible
		int x2 = 1;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lockStatement = (LockStatement)method.Body.Statements.ElementAt(1);
			var lockBlock = (BlockStatement)lockStatement.EmbeddedStatement;
			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lockBlock.Statements.Single(), "x1"));
			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x2"));
		}

		[Test]
		public void TestComparisonWithNonNull()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	string SomeValue () { return null; }
	bool PathMatches ()
	{
		string handlerPath = SomeValue();
		if (handlerPath == ""*"")
			return false;
		return true;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var end = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusBeforeStatement(end, "handlerPath"));
		}

		[Test]
		public void TestLock2()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	object MaybeNull() { return null; }
	void TestMethod()
	{
		object o = MaybeNull();
		try {
			lock (o) {
			}
		} catch (NullReferenceException e) {

		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var tryStatement = (TryCatchStatement) method.Body.Statements.Last();
			var tryBlock = tryStatement.TryBlock;
			var catchBlock = tryStatement.CatchClauses.Single().Body;

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(tryBlock, "o"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(catchBlock, "o"));
		}

		[Test]
		public void TestMemberAccess()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		object o = null;
		string s = o.ToString();
		int x2 = 1;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x2"));
		}

		[Test]
		public void TestMemberAccess2()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	object MaybeNull()
	{
		return null;
	}
	void TestMethod()
	{
		object o = MaybeNull();
		string s = o.ToString();
		int x2 = 1;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestNullableHasValue()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		int? x = null;
		bool y = x.HasValue;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestMemberAccessExtensionMethod()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
static class ObjectExtensions {
	static internal string Extension(this object obj) {
		return """";
	}
}
class TestClass
{
	void TestMethod()
	{
		object o = null;
		string s = o.Extension();
		int x2 = 1;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestAs()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
delegate void MyDelegate(string p1, out string p2);
class TestClass
{
	void TestMethod(object o)
	{
		string p1 = o as string;
		o = new object();
		string p2 = o as string;
		string p3 = typeof(string) as string;
		o = null;
		string p4 = o as string;
		string p5 = """" as string;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var p5Statement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(p5Statement, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(p5Statement, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(p5Statement, "p3"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(p5Statement, "p4"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(p5Statement, "p5"));
		}

		[Test]
		public void TestUncaptureVariable()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		while (true) {
			string p1 = null;
			Action action = () => { p1 = """"; };
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var whileStatement = (WhileStatement)method.Body.Statements.Single();
			var whileBlock = (BlockStatement)whileStatement.EmbeddedStatement;
			var actionStatement = whileBlock.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(actionStatement, "p1"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusAfterStatement(actionStatement, "p1"));
		}

		[Test]
		public void TestSimpleForeach()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? accum = 0;
		foreach (var x in new int[] { 1, 2, 3}) {
			accum += x;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ForeachStatement)method.Body.Statements.Last();
			var content = (BlockStatement)lastStatement.EmbeddedStatement;

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(content, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(lastStatement, "accum"));
		}

		[Test]
		public void TestNonNullEnumeratedValue()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? accum = 0;
		foreach (var x in new int?[] { 1, 2, 3}) {
			accum += x;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ForeachStatement)method.Body.Statements.Last();
			var content = (BlockStatement)lastStatement.EmbeddedStatement;

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(content, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(lastStatement, "accum"));
		}

		[Test]
		public void TestCapturedForeachInCSharp5()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? accum = 0;
		foreach (var x in new int?[] { 1, 2, 3 }) {
			Action action = () => { x = null; };
			accum += x;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method, true);

			var foreachStatement = (ForeachStatement)method.Body.Statements.ElementAt(1);
			var foreachBody = (BlockStatement)foreachStatement.EmbeddedStatement;
			var action = foreachBody.Statements.First();
			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(action, "x"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusAfterStatement(action, "x"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(lastStatement, "accum"));
		}

		[Test]
		public void TestCapturedForeachInCSharp4()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? accum = 0;
		foreach (var x in new int?[] { 1, 2, 3}) {
			Action action = () => { x = null; };
			accum += x;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method, false);

			var foreachStatement = (ForeachStatement)method.Body.Statements.ElementAt(1);
			var foreachBody = (BlockStatement)foreachStatement.EmbeddedStatement;
			var action = foreachBody.Statements.First();
			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusBeforeStatement(action, "x"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusAfterStatement(action, "x"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(lastStatement, "accum"));
		}

		[Test]
		public void TestForCapture()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	string TestMethod(string p)
	{
		for (int? i = 0; i < 10; ++i) {
			int? inI = i;
			Action action = () => { i = null; inI = null; };
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);
			var forStatement = (ForStatement)method.Body.Statements.Single();
			var forBody = (BlockStatement)forStatement.EmbeddedStatement;
			var actionStatement = forBody.Statements.Last();

			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusBeforeStatement(actionStatement, "i"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusAfterStatement(actionStatement, "i"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusBeforeStatement(actionStatement, "inI"));
			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusAfterStatement(actionStatement, "inI"));
		}

		[Test]
		public void TestExpressionState()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
delegate void MyDelegate(string p1, out string p2);
class TestClass
{
	string TestMethod(string p)
	{
		if (p != null) p = """";
		return p;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ReturnStatement)method.Body.Statements.Last();
			var expr = lastStatement.Expression;

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetExpressionResult(expr));
		}

		[Test]
		public void TestField()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	static object o;
	string TestMethod()
	{
		o = null;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestNullableCast()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	int? maybeNull;
	string TestMethod()
	{
		int? x = maybeNull;
		int y = (int)x;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "y"));
		}

		[Test]
		public void TestNonNullableCapture()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	string TestMethod()
	{
		int a = 0;
		Action action = () => { a = 1; };
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "a"));
		}

		[Test]
		public void TestAssignmentFromNonNullableTypes()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	int this[int idx] { get { return 0; } }
	int i;
	int M() { return 0; }
	string TestMethod()
	{
		object o = TestClass.i;
		object p = i;
		object q = M();
		object m = this[0];
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "q"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "m"));
		}

		[Test]
		public void TestNonNullReference()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void M(ref int i) {}

	string TestMethod()
	{
		int i = 1;
		M(ref i);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "i"));
		}

		[Test]
		public void TestCompileConstants()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	const int? value1 = null;
	const bool value2 = true;
	void TestMethod()
	{
		int? p1 = value2 ? value1 : 0;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (VariableDeclarationStatement)method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestConditionalAnd()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new IfElseStatement {
						Condition = new BinaryOperatorExpression(
								new BinaryOperatorExpression(new IdentifierExpression("p1"),
						                                         BinaryOperatorType.Equality,
						                                         new NullReferenceExpression()),
								BinaryOperatorType.ConditionalAnd,
								new BinaryOperatorExpression(new IdentifierExpression("p2"),
						                                     BinaryOperatorType.Equality,
						                                     new NullReferenceExpression())),
						TrueStatement = new ExpressionStatement(new AssignmentExpression(
							new IdentifierExpression("p1"),
							new PrimitiveExpression("Hello")))
					},
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter("p1"));
			method.Parameters.Add(CreateStringParameter("p2"));

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = (IfElseStatement)method.Body.Statements.First();
			var stmt2 = (ExpressionStatement)stmt1.TrueStatement;
			var stmt3 = (ReturnStatement)method.Body.Statements.ElementAt(1);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p2"));
		}

		[Test]
		public void TestConditionalOr()
		{
			var method = new MethodDeclaration {
				Body = new BlockStatement {
					new IfElseStatement {
						Condition = new UnaryOperatorExpression(UnaryOperatorType.Not,
						                                        new BinaryOperatorExpression(
							new BinaryOperatorExpression(new IdentifierExpression("p1"),
						                             BinaryOperatorType.Equality,
						                             new NullReferenceExpression()),
							BinaryOperatorType.ConditionalOr,
							new BinaryOperatorExpression(new IdentifierExpression("p2"),
						                             BinaryOperatorType.Equality,
						                             new NullReferenceExpression()))),
						TrueStatement = new ExpressionStatement(new AssignmentExpression(
							new IdentifierExpression("p1"),
							new NullReferenceExpression()))
					},
					new ReturnStatement()
				}
			};
			method.Parameters.Add(CreateStringParameter("p1"));
			method.Parameters.Add(CreateStringParameter("p2"));

			var analysis = CreateNullValueAnalysis(method);
			var stmt1 = (IfElseStatement)method.Body.Statements.First();
			var stmt2 = (ExpressionStatement)stmt1.TrueStatement;
			var stmt3 = (ReturnStatement)method.Body.Statements.ElementAt(1);

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt1, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(stmt2, "p2"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(stmt2, "p1"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(stmt3, "p2"));
		}

		[Test]
		public void TestFinally()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		int? x = 1;
		int? y = 1;
		try {
			x = 2;
			x = null;
		} finally {
			y = null;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var tryFinally = (TryCatchStatement) method.Body.Statements.Last();
			var finallyStatement = tryFinally.FinallyBlock.Statements.Single();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusBeforeStatement(finallyStatement, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(finallyStatement, "y"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(finallyStatement, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(finallyStatement, "y"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(tryFinally, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(tryFinally, "y"));
		}

		[Test]
		public void TestFinallyCapture()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? x = 1;
		try {
			Action a = () => { x = null; };
		} finally {
			Console.WriteLine(x);
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var tryFinally = (TryCatchStatement) method.Body.Statements.Last();
			var finallyStatement = tryFinally.FinallyBlock.Statements.Single();

			Assert.AreEqual(NullValueStatus.CapturedUnknown, analysis.GetVariableStatusBeforeStatement(finallyStatement, "x"));
		}


		[Test]
		public void TestReturnInFinally()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
class TestClass
{
	void TestMethod()
	{
		int? x = 1;
		int? y = 1;
		try {
			x = null;
			return;
		} finally {
			y = null;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var tryFinally = (TryCatchStatement) method.Body.Statements.Last();
			var finallyStatement = tryFinally.FinallyBlock.Statements.Single();

			//Make sure it's not unreachable
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(finallyStatement, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(finallyStatement, "y"));
		}

		[Test]
		public void TestTryCatch()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int? x = 1;
		int? y = 2;
		int? z = 3;
		try {
			x = null;
		} catch (Exception e) {
			x = null;
			y = 3;
			z = null;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var tryCatch = (TryCatchStatement) method.Body.Statements.Last();
			var catchStatement = tryCatch.CatchClauses.First().Body.Statements.First();

			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusBeforeStatement(catchStatement, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusBeforeStatement(catchStatement, "e"));
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(tryCatch, "x"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(tryCatch, "y"));
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(tryCatch, "z"));
		}

		[Test]
		public void TestIndexer()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	int[] MaybeNull() { return null; }
	void TestMethod()
	{
		int[] x = MaybeNull();
		x[(x = null) != null ? 0 : 1] = 2;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ExpressionStatement) method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestIndexer2()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	int[] MaybeNull() { return null; }
	void TestMethod()
	{
		int[] x = MaybeNull();
		object o = null;
		x[(x == null ? o = null : o = 1) != null ? 1 : 2] = 2;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = (ExpressionStatement) method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestLocalInvocation()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		Action<int> x = i => {};
		x((x = null) != null ? 1 : 0);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestLocalInvocation2()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	Action<int> MaybeNull()
	{
		Action<int> x = i => {};
		return x;
	}
	void TestMethod()
	{
		Action<int> x = MaybeNull();
		object o = null;
		x((x == null ? o = null : o = 1) == null ? 1 : 2);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestLinq()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int?[] collection = new int?[10];
		var x = from item in collection
				where item != null
				select item
				into item2
				let item3 = item2
				select item3;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var linqStatement = (VariableDeclarationStatement) method.Body.Statements.Last();
			var linqExpression = (QueryExpression)linqStatement.Variables.Single().Initializer;
			var continuation = (QueryContinuationClause)linqExpression.Clauses.First();
			var itemInWhere = ((BinaryOperatorExpression)((QueryWhereClause)continuation.PrecedingQuery.Clauses.ElementAt(1)).Condition).Left;
			var itemInSelect = ((QuerySelectClause)continuation.PrecedingQuery.Clauses.ElementAt(2)).Expression;
			var item2InLet = ((QueryLetClause)linqExpression.Clauses.ElementAt(1)).Expression;
			var item3InSelect = ((QuerySelectClause)linqExpression.Clauses.Last()).Expression;
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(linqStatement, "x"));
			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetExpressionResult(itemInWhere));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetExpressionResult(itemInSelect));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetExpressionResult(item2InLet));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetExpressionResult(item3InSelect));
		}

		[Test]
		public void TestLinqMember()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int?[] collection = new int?[10];
		foreach (var x in from item in collection
                 where item.Value > 1
				 select item) {
			/* x is not null, or else item.Value would trigger an exception */
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var foreachStatement = (ForeachStatement) method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(foreachStatement.EmbeddedStatement, "x"));
		}

		[Test]
		public void TestLinqGroupBy()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int?[] collection = new int?[10];
		foreach (var x in from item in collection
		                  group new { x = item } by item != null) {
		}
	}
}
", "test.cs");

			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var foreachStatement = (ForeachStatement)method.Body.Statements.Last();
			var foreachBody = foreachStatement.EmbeddedStatement;
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(foreachBody, "x"));
		}

		[Test]
		public void TestNoExecutionLinq()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	void TestMethod()
	{
		int?[] collection = new int?[0];
		var x = from item in collection
				where (collection = null) != null
				select item;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var linqStatement = (VariableDeclarationStatement) method.Body.Statements.Last();

			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(linqStatement, "collection"));
		}

		[Test]
		public void TestLinqMemberNoExecution()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
abstract class TestClass
{
	abstract int?[] Collection { get; }
	void TestMethod()
	{
		object o = null;
		int?[] collection = Collection;
		var x = from item in collection
                where o.ToString() == """"
				select item;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Single();
			var analysis = CreateNullValueAnalysis(tree, method);

			var lastStatement = method.Body.Statements.Last();

			//o.ToString always throws an exception, but the query might not be executed at all
			Assert.AreEqual(NullValueStatus.DefinitelyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "o"));
		}

		[Test]
		public void TestUsing()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	System.IDisposable Foo() {
		throw new NotImplementedException();
	}

	void TestMethod()
	{
		using (var x = Foo()) {
			;
		}
		using (Foo()) {
			//Ensure the analysis doesn't throw an exception for a non-declaration using
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var usingStatement = (UsingStatement)method.Body.Statements.First();
			var content = usingStatement.EmbeddedStatement;

			Assert.AreEqual(NullValueStatus.Unknown, analysis.GetVariableStatusAfterStatement(content, "x"));
		}

		[Test]
		public void TestFixed()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
using System;
class TestClass
{
	unsafe void TestMethod()
	{
		int y = 0;
		fixed (int* x = &y)
		{
			*x = 1;
		}
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);

			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);

			var fixedStatement = (FixedStatement)method.Body.Statements.Last();
			var content = fixedStatement.EmbeddedStatement;

			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(content, "x"));
		}

		[Test]
		public void TestNotNullInvocation()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	object NotNull() {
		return 1;
	}

	void TestMethod()
	{
		string p1 = NotNull();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestNotNullDelegateInvocation()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Delegate)]
	class NotNullAttribute : System.Attribute
	{
	}
}
[JetBrains.Annotations.NotNull]
delegate object NotNullDelegate();
class TestClass
{
	NotNullDelegate myDelegate;
	void TestMethod()
	{
		string p1 = myDelegate();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestNotNullField()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	string x = """";
	void TestMethod()
	{
		string p1 = this.x;
		string p2 = x;
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p2"));
		}

		[Test]
		public void TestConstructorOut()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class CanBeNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	public TestClass([JetBrains.Annotations.CanBeNull] out string o) {
		o = null;
	}
	
	void TestMethod()
	{
		string p1;
		new TestClass(out p1);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestConstructorNamedOut()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class CanBeNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	public TestClass([JetBrains.Annotations.CanBeNull] out string o) {
		o = null;
	}
	
	void TestMethod()
	{
		string p1;
		new TestClass(o: out p1);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestMethodOut()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class CanBeNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	void TestMethod([JetBrains.Annotations.CanBeNull] out string o) {
		o = null;
	}
	
	void TestMethod()
	{
		string p1;
		TestMethod(out p1);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.PotentiallyNull, analysis.GetVariableStatusAfterStatement(lastStatement, "p1"));
		}

		[Test]
		public void TestAssertion()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_TRUE)] bool condition, string message) {
	}
	
	void TestMethod(string x)
	{
		Assert(x != null, ""x is null"");
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion2()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_TRUE)] bool condition, string message) {
	}
	
	void TestMethod(string x)
	{
		Assert(message: ""x is null"", condition: x != null);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion3()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_TRUE)] bool condition = false) {
	}
	
	void TestMethod(string x)
	{
		Assert();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion4()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_FALSE)] bool condition = true) {
	}
	
	void TestMethod(string x)
	{
		Assert();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion5()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_FALSE)] bool condition = false) {
	}
	
	void TestMethod(string x)
	{
		Assert();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreNotEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion6()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void AssertNotNull([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_NOT_NULL)] object condition) {
	}
	
	void TestMethod(string x)
	{
		AssertNotNull(x);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion7()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void Assert([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_FALSE)] bool condition) {
	}
	
	void TestMethod(string x)
	{
		Assert(x == null);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.DefinitelyNotNull, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion8()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void AssertNotNull([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_NOT_NULL)] string condition = ""x"") {
	}
	
	void TestMethod(string x)
	{
		AssertNotNull();
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreNotEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}

		[Test]
		public void TestAssertion9()
		{
			var parser = new CSharpParser();
			var tree = parser.Parse(@"
namespace JetBrains.Annotations
{
	enum AssertionConditionType
	{
		IS_FALSE,
		IS_TRUE,
		IS_NULL,
		IS_NOT_NULL
	}

	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class AssertionConditionAttribute : System.Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType) {}
	}
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class AssertionMethodAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.AssertionMethod]
	void AssertNotNull([JetBrains.Annotations.AssertionCondition(JetBrains.Annotations.AssertionConditionType.IS_NOT_NULL)] object condition) {
	}
	
	void TestMethod(string x)
	{
		AssertNotNull(null);
	}
}
", "test.cs");
			Assert.AreEqual(0, tree.Errors.Count);
			
			var method = tree.Descendants.OfType<MethodDeclaration>().Last();
			var analysis = CreateNullValueAnalysis(tree, method);
			
			var lastStatement = method.Body.Statements.Last();
			
			Assert.AreEqual(NullValueStatus.UnreachableOrInexistent, analysis.GetVariableStatusAfterStatement(lastStatement, "x"));
		}
	}
}

