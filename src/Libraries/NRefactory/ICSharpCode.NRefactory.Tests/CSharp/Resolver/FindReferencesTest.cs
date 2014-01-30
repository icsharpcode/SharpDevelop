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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using ICSharpCode.NRefactory.Analysis;
using System.Text;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class FindReferencesTest
	{
		SyntaxTree syntaxTree;
		CSharpUnresolvedFile unresolvedFile;
		ICompilation compilation;
		FindReferences findReferences;
		
		void Init(string code)
		{
			syntaxTree = SyntaxTree.Parse(code, "test.cs");
			unresolvedFile = syntaxTree.ToTypeSystem();
			compilation = TypeSystemHelper.CreateCompilation(unresolvedFile);
			findReferences = new FindReferences();
		}
		
		AstNode[] FindReferences(ISymbol entity)
		{
			var result = new List<AstNode>();
			var searchScopes = findReferences.GetSearchScopes(entity);
			findReferences.FindReferencesInFile(searchScopes, unresolvedFile, syntaxTree, compilation,
			                                    (node, rr) => result.Add(node), CancellationToken.None);
			return result.OrderBy(n => n.StartLocation).ToArray();
		}

		AstNode[] FindReferences(INamespace ns)
		{
			var result = new List<AstNode>();
			var searchScopes = findReferences.GetSearchScopes(ns);
			findReferences.FindReferencesInFile(searchScopes, unresolvedFile, syntaxTree, compilation,
			                                    (node, rr) => result.Add(node), CancellationToken.None);
			return result.OrderBy(n => n.StartLocation).ToArray();
		}

		#region Parameters
		[Test]
		public void FindParameterReferences()
		{
			Init(@"using System;
class Test {
	void M(string par) {
		Console.WriteLine (par);
	}

	void Other()
	{
		M(par:null);
	}
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single();
			var method = test.Methods.Single(m => m.Name == "M");
			Assert.AreEqual(new int[] { 3, 4, 9 }, FindReferences(method.Parameters[0]).Select(n => n.StartLocation.Line).ToArray());
		}
		#endregion


		#region Method Group
		[Test]
		public void FindMethodGroupReference()
		{
			Init(@"using System;
class Test {
  Action<int> x = M;
  Action<string> y = M;
  static void M(int a) {}
  static void M(string a) {}
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single();
			var m_int = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "Int32");
			var m_string = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "String");
			Assert.AreEqual(new int[] { 3, 5 }, FindReferences(m_int).Select(n => n.StartLocation.Line).ToArray());
			Assert.AreEqual(new int[] { 4, 6 }, FindReferences(m_string).Select(n => n.StartLocation.Line).ToArray());
		}
		
		[Test]
		public void FindMethodGroupReferenceInOtherMethodCall()
		{
			Init(@"using System;
class Test {
 static void T(Action<int> a, Action<string> b) {
  this.T(M, M);
 }
 static void M(int a) {}
 static void M(string a) {}
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single();
			var m_int = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "Int32");
			var m_string = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "String");
			Assert.AreEqual(new [] { new TextLocation(4, 10), new TextLocation(6, 2) },
			                FindReferences(m_int).Select(n => n.StartLocation).ToArray());
			Assert.AreEqual(new [] { new TextLocation(4, 13), new TextLocation(7, 2) },
			                FindReferences(m_string).Select(n => n.StartLocation).ToArray());
		}
		
		[Test]
		public void FindMethodGroupReferenceInExplicitDelegateCreation()
		{
			Init(@"using System;
class Test {
 static void T(Action<int> a, Action<string> b) {
  this.T(new Action<int>(M), new Action<string>(M));
 }
 static void M(int a) {}
 static void M(string a) {}
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single();
			var m_int = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "Int32");
			var m_string = test.Methods.Single(m => m.Name == "M" && m.Parameters.Single().Type.Name == "String");
			Assert.AreEqual(new [] { new TextLocation(4, 26), new TextLocation(6, 2) },
			                FindReferences(m_int).Select(n => n.StartLocation).ToArray());
			Assert.AreEqual(new [] { new TextLocation(4, 49), new TextLocation(7, 2) },
			                FindReferences(m_string).Select(n => n.StartLocation).ToArray());
		}
		#endregion
		
		#region GetEnumerator
		[Test]
		public void FindReferenceToGetEnumeratorUsedImplicitlyInForeach()
		{
			Init(@"using System;
class MyEnumerable {
 public System.Collections.IEnumerator GetEnumerator();
}
class Test {
 static void T() {
  var x = new MyEnumerable();
  foreach (var y in x) {
  }
 }
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyEnumerable");
			var getEnumerator = test.Methods.Single(m => m.Name == "GetEnumerator");
			var actual = FindReferences(getEnumerator).ToList();
			Assert.AreEqual(2, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 3 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is ForeachStatement));
		}
		#endregion
		
		#region Op_Implicit
		[Test]
		public void FindReferencesForOpImplicitInLocalVariableInitialization()
		{
			Init(@"using System;
class Test {
 static void T() {
  int x = new Test();
 }
 public static implicit operator int(Test x) { return 0; }
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "Test");
			var opImplicit = test.Methods.Single(m => m.Name == "op_Implicit");
			var actual = FindReferences(opImplicit).ToList();
			Assert.AreEqual(2, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is ObjectCreateExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 6 && r is OperatorDeclaration));
		}
		
		[Test]
		public void FindReferencesForOpImplicitInLocalVariableInitialization_ExplicitCast()
		{
			Init(@"using System;
class Test {
 static void T() {
  int x = (int)new Test();
 }
 public static implicit operator int(Test x) { return 0; }
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "Test");
			var opImplicit = test.Methods.Single(m => m.Name == "op_Implicit");
			var actual = FindReferences(opImplicit).ToList();
			Assert.AreEqual(2, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is ObjectCreateExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 6 && r is OperatorDeclaration));
		}
		
		[Test]
		public void FindReferencesForOpImplicitInAssignment_ExplicitCast()
		{
			Init(@"using System;
class Test {
 static void T() {
  int x;
  x = (int)new Test();
 }
 public static implicit operator int(Test x) { return 0; }
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "Test");
			var opImplicit = test.Methods.Single(m => m.Name == "op_Implicit");
			var actual = FindReferences(opImplicit).ToList();
			Assert.AreEqual(2, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 5 && r is ObjectCreateExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 7 && r is OperatorDeclaration));
		}
		#endregion
		
		#region Inheritance
		const string inheritanceTest = @"using System;
class A { public virtual void M() {} }
class B : A { public override void M() {} }
class C : A { public override void M() {} }
class Calls {
	void Test(A a, B b, C c) {
		a.M();
		b.M();
		c.M();
	}
}";
		
		[Test]
		public void InheritanceTest1()
		{
			Init(inheritanceTest);
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "B");
			var BM = test.Methods.Single(m => m.Name == "M");
			var actual = FindReferences(BM).ToList();
			Assert.AreEqual(2, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 3 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is InvocationExpression));
		}
		
		[Test]
		public void InheritanceTest2()
		{
			Init(inheritanceTest);
			findReferences.FindCallsThroughVirtualBaseMethod = true;
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "B");
			var BM = test.Methods.Single(m => m.Name == "M");
			var actual = FindReferences(BM).ToList();
			Assert.AreEqual(3, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 3 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 7 && r is InvocationExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is InvocationExpression));
		}
		
		[Test]
		public void InheritanceTest3()
		{
			Init(inheritanceTest);
			findReferences.WholeVirtualSlot = true;
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "B");
			var BM = test.Methods.Single(m => m.Name == "M");
			var actual = FindReferences(BM).ToList();
			Assert.AreEqual(6, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 2 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 3 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 7 && r is InvocationExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is InvocationExpression));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 9 && r is InvocationExpression));
		}
		#endregion

		#region Await

		#if NET_4_5

		const string awaitTest = @"using System;
class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter() { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = await x;
	}
}";

		[Test]
		public void GetAwaiterReferenceInAwaitExpressionIsFound() {
			Init(awaitTest);
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyAwaitable");
			var method = test.Methods.Single(m => m.Name == "GetAwaiter");
			var actual = FindReferences(method).ToList();
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 13 && r is UnaryOperatorExpression));
		}

		[Test]
		public void GetResultReferenceInAwaitExpressionIsFound() {
			Init(awaitTest);
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyAwaiter");
			var method = test.Methods.Single(m => m.Name == "GetResult");
			var actual = FindReferences(method).ToList();
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 5 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 13 && r is UnaryOperatorExpression));
		}

		[Test]
		public void OnCompletedReferenceInAwaitExpressionIsFound() {
			Init(awaitTest);
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyAwaiter");
			var method = test.Methods.Single(m => m.Name == "OnCompleted");
			var actual = FindReferences(method).ToList();
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 13 && r is UnaryOperatorExpression));
		}

		[Test]
		public void IsCompletedReferenceInAwaitExpressionIsFound() {
			Init(awaitTest);
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyAwaiter");
			var property = test.Properties.Single(m => m.Name == "IsCompleted");
			var actual = FindReferences(property).ToList();
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 3 && r is PropertyDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 13 && r is UnaryOperatorExpression));
		}

		[Test]
		public void UnsafeOnCompletedReferenceInAwaitExpressionIsFound() {
			Init(@"using System;
class MyAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public void UnsafeOnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter() { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = await x;
	}
}");
			var test = compilation.MainAssembly.TopLevelTypeDefinitions.Single(t => t.Name == "MyAwaiter");
			var method = test.Methods.Single(m => m.Name == "UnsafeOnCompleted");
			var actual = FindReferences(method).ToList();
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 5 && r is MethodDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 14 && r is UnaryOperatorExpression));
		}

		#endif // NET_4_5

		#endregion
		
		#region Namespaces
		[Test]
		public void FindNamespaceTest()
		{
			Init(@"using System;
using Foo.Bar;

namespace Foo.Bar {
	class MyTest { }
}

namespace Other.Bar {
	class OtherTest {}
}

namespace Foo
{
	class Test
	{
		static void T()
		{
			Bar.MyTest test;
			Other.Bar.OtherTest test2;
		}
	}
}

namespace B
{
	using f = Foo.Bar;
	class Test2
	{
		Foo.Bar.MyTest a;
	}
}
");
			var test = compilation.MainAssembly.RootNamespace.GetChildNamespace("Foo").GetChildNamespace ("Bar");
			var actual = FindReferences(test).ToList();
			Assert.AreEqual(5, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 2 && r is MemberType));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is NamespaceDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 18 && r is SimpleType));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 26 && r is MemberType));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 29 && r is MemberType));
		}

		[Test]
		public void FindSub()
		{
			Init(@"using System;
using Foo.Bar;

namespace Foo.Bar {
	class MyTest { }
}

namespace Foo
{
	class Test
	{
		Foo.Bar.MyTest t;
	}
}
");
			var test = compilation.MainAssembly.RootNamespace.GetChildNamespace("Foo");
			var actual = FindReferences(test).ToList();
			Assert.AreEqual(4, actual.Count);
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 2 && r is SimpleType));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 4 && r is NamespaceDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 8 && r is NamespaceDeclaration));
			Assert.IsTrue(actual.Any(r => r.StartLocation.Line == 12 && r is SimpleType));
		}
		#endregion
		
		#region Rename

		internal static ISymbol GetSymbol (ICompilation compilation, string reflectionName)
		{
			Stack<ITypeDefinition> typeStack = new Stack<ITypeDefinition>(compilation.MainAssembly.TopLevelTypeDefinitions);
			while (typeStack.Count > 0) {
				var cur = typeStack.Pop();
				if (cur.ReflectionName == reflectionName)
					return cur;
				foreach (var member in cur.Members)
					if (member.ReflectionName == reflectionName)
						return member;
				foreach (var nested in cur.NestedTypes) {
					typeStack.Push(nested);
				}
			}
			return null;
		}

		IList<AstNode> Rename(string fullyQualifiedName, string newName, bool includeOverloads)
		{
			var sym = GetSymbol(compilation, fullyQualifiedName);
			Assert.NotNull(sym);
			var graph = new TypeGraph(compilation.Assemblies);
			var col = new SymbolCollector();
			col.IncludeOverloads = includeOverloads;
			col.GroupForRenaming = true;
			var scopes = findReferences.GetSearchScopes(col.GetRelatedSymbols(graph, sym));
			List<AstNode> result = new List<AstNode>();

			findReferences.RenameReferencesInFile(
				scopes,
				newName,
				new CSharpAstResolver(compilation, syntaxTree, unresolvedFile),
				delegate(RenameCallbackArguments obj) {
					result.Add (obj.NodeToReplace);
				},
				delegate(Error obj) {
					
				});
			return result;
		}

		void TestRename(string code, string symbolName)
		{
			StringBuilder sb = new StringBuilder();
			List<int> offsets = new List<int>();
			foreach (var ch in code) {
				if (ch == '$') {
					offsets.Add(sb.Length);
					continue;
				}
				sb.Append(ch);
			}
			Init(sb.ToString ());
			findReferences.WholeVirtualSlot = true;
			var doc = new ReadOnlyDocument(sb.ToString ());
			var result = Rename(symbolName, "x", false);
			Assert.AreEqual(offsets.Count, result.Count);

			result.Select(r => doc.GetOffset (r.StartLocation)).SequenceEqual(offsets);
		}

		[Test]
		public void TestSimpleRename ()
		{
			TestRename (@"using System;
class $Test {
	$Test test;
}", "Test");
		}


		[Test]
		public void TestOverride ()
		{
			TestRename(@"using System;
class Test {
	public virtual int $Foo { get; set; }
}

class Test2 : Test {
	public override int $Foo { get; set; }
}

class Test3 : Test {
	public override int $Foo { get; set; }
	public FindReferencesTest ()
	{
		$Foo = 4;
	}
}
", "Test.Foo");
		}

		#endregion
	}
}
