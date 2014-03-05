//
// ConvertToLambdaExpressionIssueTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertToLambdaExpressionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestReturn ()
		{
			Test<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		System.Func<int, int> f = i => {
			return i + 1;
		};
	}
}", @"
class TestClass
{
	void TestMethod ()
	{
		System.Func<int, int> f = i => i + 1;
	}
}");
		}

		[Test]
		public void TestExpressionStatement ()
		{
			Test<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		System.Action<int> f = i => {
			System.Console.Write (i);
		};
	}
}", @"
class TestClass
{
	void TestMethod ()
	{
		System.Action<int> f = i => System.Console.Write (i);
	}
}");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		// ReSharper disable once ConvertToLambdaExpression
		System.Func<int, int> f = i => {
			return i + 1;
		};
	}
}");
		}


		[Test]
		public void TestAssignmentExpression ()
		{
			TestWrongContext<ConvertToLambdaExpressionIssue>(@"
using System;
public class Test
{
	void Foo ()
	{
		int i;
		Action<int> foo = x => {
			i = x + x;
		};
		foo(5);
	}
}
");
		}


		/// <summary>
		/// Bug 14840 - Incorrect "can be converted to expression" suggestion
		/// </summary>
		[Test]
		public void TestBug14840 ()
		{
			TestWrongContext<ConvertToLambdaExpressionIssue>(@"using System;
using System.Collections.Generic;

class C
{
	void Foo (Action<int> a) {}
	void Foo (Func<int,int> a) {}

	void Test ()
	{
		int t = 0;
		Foo (c => { t = c; });
	}
}");
		}

		[Test]
		public void TestAnonymousMethod ()
		{
			Test<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		System.Action a = delegate () {
			System.Console.WriteLine ();
		};
	}
}", @"
class TestClass
{
	void TestMethod ()
	{
		System.Action a = () => System.Console.WriteLine ();
	}
}");
		}


		[Test]
		public void TestAnonymousFunction ()
		{
			Test<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		System.Func<int, int> f = delegate (int i) {
			return i + 1;
		};
	}
}", @"
class TestClass
{
	void TestMethod ()
	{
		System.Func<int, int> f = (int i) => i + 1;
	}
}");
		}

		[Test]
		public void TestAnonymousMethodWithoutParameterList ()
		{
			TestWrongContext<ConvertToLambdaExpressionIssue> (@"
class TestClass
{
	void TestMethod ()
	{
		System.Func<int, int> f = delegate {
			return 123;
		};
	}
}");
		}


	}
}

