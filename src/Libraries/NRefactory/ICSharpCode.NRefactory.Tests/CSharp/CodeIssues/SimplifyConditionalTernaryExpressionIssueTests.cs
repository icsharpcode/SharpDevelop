//
// SimplifyConditionalTernaryExpressionIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class SimplifyConditionalTernaryExpressionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestFalseTrueCase ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void Bar ()
	{
		var a = 1 < 2 ? false : true;
	}
}
", @"
class Foo
{
	void Bar ()
	{
		var a = 1 >= 2;
	}
}
");
		}

		[Test]
		public void TestFalseTrueCase2 ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void Bar ()
	{
		var a = obj is foo ? false : true;
	}
}
", @"
class Foo
{
	void Bar ()
	{
		var a = !(obj is foo);
	}
}
");
		}
	
		[Test]
		public void TestFalseExprCase ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b ? false : c);
	}
}
", @"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a >= b && c);
	}
}
");
		}

		[Test]
		public void TestTrueExprCase ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b ? true : c);
	}
}
", @"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b || c);
	}
}
");
		}

		[Test]
		public void TestExprFalseCase ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b ? c : false);
	}
}
", @"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b && c);
	}
}
");
		}

		[Test]
		public void TestExprTrueCase ()
		{
			Test<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a < b ? c : true);
	}
}
", @"
class Foo
{
	void FooBar (int a, int b, bool c)
	{
		Console.WriteLine (a >= b || c);
	}
}
");
		}

		[Test]
		public void TestInvalidCase ()
		{
			TestWrongContext<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void FooBar (int a, int b, bool c, boold d)
	{
		Console.WriteLine (a < b ? c : d);
	}
}");
		}
	

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void Bar ()
	{
		// ReSharper disable once SimplifyConditionalTernaryExpression
		var a = 1 < 2 ? false : true;
	}
}
");
		}
	

		[Test]
		public void TestSkipRedundantCase ()
		{
			TestWrongContext<SimplifyConditionalTernaryExpressionIssue>(@"
class Foo
{
	void Bar ()
	{
		var a = 1 < 2 ? true : false;
	}
}
");
		}
	
	}
}

