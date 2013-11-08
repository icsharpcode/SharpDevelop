//
// RewriteIfReturnToReturnIssueTests.cs
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
	public class RewriteIfReturnToReturnIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestIfElse ()
		{
			TestIssue<RewriteIfReturnToReturnIssue>(@"class Foo
{
	bool Bar (string str)
	{
		if (str.Length > 10)
			return true;
		else
			return false;
	}
}");
		}

		[Test]
		public void TestIfReturn ()
		{
			TestIssue<RewriteIfReturnToReturnIssue>(@"class Foo
{
	bool Bar (string str)
	{
		if (str.Length > 10)
			return true;
		return false;
	}
}");
		}
	
		[Test]
		public void TestSkipIfElseIf ()
		{
			TestWrongContext<ConvertIfStatementToConditionalTernaryExpressionIssue>(@"class Foo
{
	static int Bar (int x)
	{
		if (x < 10)
			return -10;
		else if (x > 10)
			return 10;
		else
			return 20;
		return result;
	}
}");
		}

		[Test]
		public void TestSkipComplexTrueExpression ()
		{
			TestWrongContext<ConvertIfStatementToConditionalTernaryExpressionIssue>(@"class Foo
{
	static int Bar (int x)
	{
		if (x > 10)
			return 10 +
					 12;
		else
			return 20;
		return result;
	}
}");
		}

		[Test]
		public void TestSkipComplexFalseExpression ()
		{
			TestWrongContext<ConvertIfStatementToConditionalTernaryExpressionIssue>(@"class Foo
{
	static int Bar (int x)
	{
		if (x > 10)
			return 10;
		else
			return 20 +

12;
	}
}");
		}


		[Test]
		public void TestSkipAnnoyingSuggestionCase1 ()
		{
			TestWrongContext<ConvertIfStatementToConditionalTernaryExpressionIssue>(@"class Foo
{
	int Bar (int a, int b)
	{
		if (a < 1 && a > 2 && b < a) {
			return a;
		}
		return b;
	}
}");
		}
	}
}

