// 
// UseOfMemberOfNullReferenceTests.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
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

using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class UseOfMemberOfNullReferenceTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimple ()
		{
			string input = @"
class TestClass
{
	int TestMethod() {
		string x = null;
		return x.Length;
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestDisabledForNonNull ()
		{
			string input = @"
class TestClass
{
	int TestMethod() {
		string x = """";
		return x.Length;
	}
}";
			TestWrongContext<UseOfMemberOfNullReference> (input);
		}

		[Test]
		public void TestDisabledForHasValue ()
		{
			string input = @"
class TestClass
{
	bool TestMethod() {
		int? x = null;
		return x.HasValue;
	}
}";
			TestWrongContext<UseOfMemberOfNullReference> (input);
		}

		[Test]
		public void TestMethodGroup ()
		{
			string input = @"
static class X {
	static void Clone(this string x, int i) {}
}
class TestClass
{
	object TestMethod() {
		string x = null;
		return x.Clone();
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestDisabledForExtensionMethods ()
		{
			string input = @"
static class X
{
	static void Foo(this string x) {}
}
class TestClass
{
	void TestMethod() {
		string x = """";
		x.Foo();
	}
}";
			TestWrongContext<UseOfMemberOfNullReference> (input);
		}

		[Test]
		public void TestAs ()
		{
			string input = @"
class TestClass
{
	int TestMethod(object x) {
		return (x as string).Length;
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestAsCase2 ()
		{
			string input = @"
class TestClass
{
	int TestMethod(object x) {
		return (this as TestClass).TestMethod(null);
	}
}";
			Test<UseOfMemberOfNullReference> (input, 0);
		}


		[Test]
		public void TestIfBranch ()
		{
			string input = @"
class TestClass
{
	int TestMethod(object x, object y) 
	{
		if (y != null)
			x = null;
		return x.GetHashCode ();
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestSwitchBranch ()
		{
			string input = @"
class TestClass
{
	int TestMethod(object x, int y) 
	{
		switch (y) {
		case 1:
			break;
		case 2:
			x = null;
			break;
		default:
			x = new object ();
			break;
		}
		return x.GetHashCode ();
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestWhile ()
		{
			string input = @"
class TestClass
{
	int TestMethod(object x, int y) 
	{
		while (y++ < 10) {
			x = null;
		}
		return x.GetHashCode ();
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestNullExpression ()
		{
			string input = @"
class TestClass
{
	int TestMethod() {
		return default(string).Length;
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestNullExpressionCase2 ()
		{
			string input = @"
class TestClass
{
	int TestMethod() {
		return (default(string) ?? default(string)).Length;
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}

		[Test]
		public void TestNullExpressionCase3 ()
		{
			string input = @"
class TestClass
{
	int TestMethod() {
		return (1 == 1 ? default(string) : """").Length;
	}
}";
			Test<UseOfMemberOfNullReference> (input, 1);
		}


		
		[Test]
		public void TestParameter ()
		{
			TestWrongContext<UseOfMemberOfNullReference> (@"
class TestClass
{
	void TestMethod(TestClass test) 
	{
		test.TestMethod(this);
	}
}");
		}

		[Test]
		public void TestDoWhile()
		{
			TestWrongContext<UseOfMemberOfNullReference> (@"
public class TestClass {
	static string GetElement ()
	{
		string x = """";
		do {
			break;
		} while ((x = null) == null);
		x.ToString();
	}
}");
		}

		[Test]
		public void TestDefaultStruct ()
		{
			TestWrongContext<UseOfMemberOfNullReference> (@"
struct Foo { public int Length { get { return 0; } } } 
class TestClass
{
	int TestMethod() 
	{
		var foo = default(Foo);
		return foo.Length;
	}
}");
		}

	}
}