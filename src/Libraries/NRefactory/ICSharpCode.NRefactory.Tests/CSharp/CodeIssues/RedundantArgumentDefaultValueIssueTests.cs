//
// RedundantArgumentDefaultValueIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantArgumentDefaultValueIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleCase()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		Bar (s);
	}
}
", @"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		Bar ();
	}
}
");
		}


		[Test]
		public void TestNamedArgument()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar(foo: 22);
	}
}
", @"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar ();
	}
}
");
		}

		[Test]
		public void TestMultipleRemoveFirst()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s, 3);
	}
}
", 2, @"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar ();
	}
}
", 0);
		}

		[Test]
		public void TestMultipleRemoveSecond()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s, 3);
	}
}
", 2, @"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s);
	}
}
", 1);
		}
	
		[Test]
		public void TestInvalid()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar (21);
	}
}");
		}

		[Test]
		public void TestInvalidCase2()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int bar = 3)
	{
		Bar (22, 4);
	}
}");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		// ReSharper disable once RedundantArgumentDefaultValue
		Bar (s);
	}
}
");
		}
	
		[Test]
		public void TestConstructor()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public Test (int a, int b, int c = 4711)
	{
		new Test (1, 2, 4711);
	}
}
", @"
class Test
{ 
	public Test (int a, int b, int c = 4711)
	{
		new Test (1, 2);
	}
}
");
		}

		[Test]
		public void TestIndexer()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public int this [int a, int b = 2] {
		get {
			return this [a, 2];
		}
	}
}
", @"
class Test
{ 
	public int this [int a, int b = 2] {
		get {
			return this [a];
		}
	}
}
");
		}

		[Test]
		public void SimpleCase()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"")
	{
	}

	void Bar()
	{
		Foo (""a1"");
	}
}", @"
class TestClass
{
	void Foo(string a1 = ""a1"")
	{
	}

	void Bar()
	{
		Foo ();
	}
}");
		}

		[Test]
		public void ChecksConstructors()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	public TestClass(string a1 = ""a1"")
	{
	}

	void Bar()
	{
		var foo = new TestClass (""a1"");
	}
}", @"
class TestClass
{
	public TestClass(string a1 = ""a1"")
	{
	}

	void Bar()
	{
		var foo = new TestClass ();
	}
}");
		}

		[Test]
		public void IgnoresAllParametersPreceedingANeededOne()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"")
	{
	}

	void Bar()
	{
		Foo (""a1"", ""Another string"", ""a3"");
	}
}", @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"")
	{
	}

	void Bar()
	{
		Foo (""a1"", ""Another string"");
	}
}");
		}

		[Test]
		public void ChecksParametersIfParamsAreUnused()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"", params string[] extraStrings)
	{
	}

	void Bar()
	{
		Foo (""a1"", ""a2"", ""a3"");
	}
}", 3, @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"", params string[] extraStrings)
	{
	}

	void Bar()
	{
		Foo ();
	}
}", 0);
		}

		[Test]
		public void IgnoresIfParamsAreUsed()
		{
			var input = @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"", params string[] extraStrings)
	{
	}

	void Bar()
	{
		Foo (""a1"", ""a2"", ""a3"", ""extraString"");
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantArgumentDefaultValueIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Ignore("Fixme")]
		[Test]
		public void NamedArgument()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"")
	{
	}

	void Bar()
	{
		Foo (a2: ""a2"");
	}
}", @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"")
	{
	}

	void Bar()
	{
		Foo ();
	}
}");
		}

		[Ignore("Fixme")]
		[Test]
		public void DoesNotStopAtDifferingNamedParameters()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"")
	{
	}

	void Bar()
	{
		Foo (""a1"", ""a2"", a3: ""non-default"");
	}
}", @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", string a3 = ""a3"")
	{
	}

	void Bar()
	{
		Foo (a3: ""non-default"");
	}
}");
		}

		[Ignore("Fixme")]
		[Test]
		public void DoesNotStopAtNamedParamsArray()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", params string[] extras)
	{
	}

	void Bar()
	{
		Foo (""a1"", ""a2"", extras: new [] { ""extra1"" });
	}
}", @"
class TestClass
{
	void Foo(string a1 = ""a1"", string a2 = ""a2"", params string[] extras)
	{
	}

	void Bar()
	{
		Foo (extras: new[] {
	""extra1""
});
	}
}");
		}

	}
}

