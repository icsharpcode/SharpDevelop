// 
// AddOptionalParameterToInvocationTests.cs
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

using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class AddOptionalParameterToInvocationTests : ContextActionTestBase
	{
		[Test]
		public void TestSimple ()
		{
			Test<AddOptionalParameterToInvocationAction> (@"
class TestClass
{
	public void Foo(string msg = ""Hello"") {}
	public void Bar() {
		$Foo ();
	}
}", @"
class TestClass
{
	public void Foo(string msg = ""Hello"") {}
	public void Bar() {
		Foo (""Hello"");
	}
}");
		}

		[Test]
		public void TestMultiple1 ()
		{
			Test<AddOptionalParameterToInvocationAction> (@"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		$Foo ();
	}
}", @"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		Foo (""Hello"");
	}
}");
		}

		[Test]
		public void TestExtensionMethod ()
		{
			Test<AddOptionalParameterToInvocationAction> (@"
static class Extensions
{
	public static void Foo(this string self, string msg = ""Hello"") {}
}
class TestClass
{
	public void Bar() {
		string.$Foo ();
	}
}", @"
static class Extensions
{
	public static void Foo(this string self, string msg = ""Hello"") {}
}
class TestClass
{
	public void Bar() {
		string.Foo (""Hello"");
	}
}");
		}

		[Test]
		public void TestMultiple2 ()
		{
			Test<AddOptionalParameterToInvocationAction> (@"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		$Foo ();
	}
}", @"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		Foo (msg2: ""Bar"");
	}
}", 1);
		}

		[Test]
		public void TestMultiple3 ()
		{
			Test<AddOptionalParameterToInvocationAction> (@"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		$Foo ();
	}
}", @"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		Foo (""Hello"", ""Bar"");
	}
}", 2);
		}

		[Test]
		public void TestNoMoreParameters ()
		{
			TestWrongContext<AddOptionalParameterToInvocationAction>(@"
class TestClass
{
	public void Foo(string msg = ""Hello"", string msg2 = ""Bar"") {}
	public void Bar() {
		$Foo (string.Empty, string.Empty);
	}
}
");
		}

		[Test]
		public void TestParams ()
		{
			TestWrongContext<AddOptionalParameterToInvocationAction>(@"
class TestClass
{
	public void Foo(params string[] p) {}
	public void Bar() {
		$Foo ();
	}
}
");
		}
	}
}
