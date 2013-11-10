//
// ConvertIfStatementToSwitchStatementIssueTests.cs
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
	public class ConvertIfStatementToSwitchStatementIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestBreak ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		} else if (a == 2 || a == 3) {
			b = 2;
		} else {
			b = 3;
		}
	}
}");
		}

		[Test]
		public void TestReturn ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	int TestMethod (int a)
	{
		$if (a == 0) {
			int b = 1;
			return b + 1;
		} else if (a == 2 || a == 3) {
			return 2;
		} else if (a == 13) {
			return 12;
		} else {
			return -1;
		}
	}
}");
		}
		
		[Test]
		public void TestConstantExpression ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	int TestMethod (int? a)
	{
		if (a == (1 == 1 ? 11 : 12)) {
			return 1;
		} else if (a == (2 * 3) + 1 || a == 6 / 2) {
			return 2;
		} else if (a == null || a == (int)(10L + 2) || a == default(int) || a == sizeof(int)) {
			return 3;		
		} else {
			return -1;
		}
	}
}");


			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	const int b = 0;
	int TestMethod (int a)
	{
		const int c = 1;
		$if (a == b) {
			return 1;
		} else if (a == b + c) {
			return 0;
		} else if (a ==  c) {
			return 2;
		} else {
			return -1;
		}
	}
}");
		}

		[Test]
		public void TestNestedOr ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	int TestMethod (int a)
	{
		$if (a == 0) {
			return 1;
		} else if ((a == 2 || a == 4) || (a == 3 || a == 5)) {
			return 2;
		} else if (a == 12) {
			return 23;
		} else {
			return -1;
		}
	}
}");
		}

		[Test]
		public void TestComplexSwitchExpression ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	int TestMethod (int a, int b)
	{
		$if (a + b == 0) {
			return 1;
		} else if (1 == a + b) {
			return 0;
		} else if (2 == a + b) {
			return 0;
		} else {
			return -1;
		}
	}
}");
		}

		[Test]
		public void TestNonConstantExpression ()
		{
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a, int c)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == c) {
			b = 1;
		} else if (a == 2 || a == 3) {
			b = 2;
		} else {
			b = 3;
		}
	}
}");
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a, int c)
	{
		int b;
		$if (a == c) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		} else if (a == 2 || a == 3) {
			b = 2;
		} else {
			b = 3;
		}
	}
}");
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a, int c)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		} else if (a == 2 || a == c) {
			b = 2;
		} else {
			b = 3;
		}
	}
}");
		}
		
		[Test]
		public void TestNonEqualityComparison ()
		{
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a > 4) {
			b = 1;
		} else if (a == 2 || a == 3) {
			b = 2;
		} else {
			b = 3;
		}
	}
}");
		}

		[Test]
		public void TestValidType ()
		{
			// enum
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
enum TestEnum
{
	First,
	Second,
	Third
}
class TestClass
{
	int TestMethod (TestEnum a)
	{
		$if (a == TestEnum.First) {
			return 1;
		} else if (a == TestEnum.Second) {
			return -1;
		} else if (a == TestEnum.Third) {
			return 3;
		} else {
			return 0;
		}
	}
}");

			// string, bool, char, integral, nullable
			TestValidType ("string", "\"test\"");
			TestValidType ("bool", "true");
			TestValidType ("char", "'a'");
			TestValidType ("byte", "0");
			TestValidType ("sbyte", "0");
			TestValidType ("short", "0");
			TestValidType ("long", "0");
			TestValidType ("ushort", "0");
			TestValidType ("uint", "0");
			TestValidType ("ulong", "0");
			TestValidType ("bool?", "null");
		}

		void TestValidType (string type, string caseValue)
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	int TestMethod (" + type + @" a)
	{
		$if (a == " + caseValue + @") {
			return 1;
		} else if (a == default("  + type +  @")) {
			return -1;
		} else if (a == null) {
			return -1;
		}
	}
}");
		}

		[Test]
		public void TestInvalidType ()
		{
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (double a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else {
			b = 3;
		}
	}
}");
		}

		[Test]
		public void TestNoElse()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		} else if (a == 2) {
			b = 1;
		}
	}
}");
		}
	
		[Test]
		public void TestNestedIf ()
		{
			TestIssue<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			if (b == 0)
				return;
		} else if (a == 2 || a == 3) {
			b = 2;
		} else if (a == 12) {
			b = 3;
		}
	}
}");
		}

		
		[Test]
		public void TestτooSimpleCase1()
		{
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		}
	}
}");
		}
		[Test]
		public void TestτooSimpleCase2()
		{
			TestWrongContext<ConvertIfStatementToSwitchStatementIssue> (@"
class TestClass
{
	void TestMethod (int a)
	{
		int b;
		$if (a == 0) {
			b = 0;
		} else if (a == 1) {
			b = 1;
		} else {
			b = 1;
		}
	}
}");
		}
	}
}

