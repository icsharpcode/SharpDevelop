//
// SimplifyAnonymousMethodToDelegateIssueTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class SimplifyAnonymousMethodToDelegateIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleLambda ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		var action = $(foo, bar) => MyMethod (foo, bar);
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	void Bar (string str)
	{
		var action = MyMethod;
	}
}");
		}

		[Test]
		public void TestLambdaWithBody ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		var action = $(foo, bar) => { return MyMethod (foo, bar); };
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	void Bar (string str)
	{
		var action = MyMethod;
	}
}");
		}

		[Test]
		public void TestSimpleInvalidLambda ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		var action = $(foo, bar) => MyMethod (bar, foo);
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestSimpleAnonymousMethod ()
		{
			var input = @"class Foo
{
	int MyMethod (int x, int y) { return x * y; }

	void Bar (string str)
	{
		var action = $delegate(int foo, int bar) { return MyMethod (foo, bar); };
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	int MyMethod (int x, int y) { return x * y; }

	void Bar (string str)
	{
		var action = MyMethod;
	}
}");
		}

		[Test]
		public void TestSkipComplexCase ()
		{
			var input = @"using System;
using System.Linq;

class Foo
{
	int MyMethod (int x, int y) { return x * y; }

	void Bar (string str)
	{
		var action = $() => str.Where (c => c != 'a').ToArray ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestComplexCase ()
		{
			var input = @"class Foo
{
	int MyMethod (int x, int y) { return x * y; }

	void Bar (string str)
	{
		var action = $delegate(int foo, int bar) { return MyMethod (bar, foo); };
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new SimplifyAnonymousMethodToDelegateIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
	}
}

