// 
// RedundantIfElseBlockIssueTests.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantIfElseBlockIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestReturn()
		{
			var input = @"
class TestClass
{
	int TestMethod (int i)
	{
		if (i > 0)
			return 1;
		else
			return 0;
	}
}";
			var output = @"
class TestClass
{
	int TestMethod (int i)
	{
		if (i > 0)
			return 1;
		return 0;
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

		[Test]
		public void TestDisable()
		{
			var input = @"
class TestClass
{
	int TestMethod (int i)
	{
		if (i > 0)
			return 1;
// ReSharper disable once RedundantIfElseBlock
		else
			return 0;
	}
}";
			TestWrongContext<RedundantIfElseBlockIssue>(input);
		}

		[Test]
		public void TestBreakLoop()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int k = 0;
		for (int i = 0; i < 10; i++) {
			if (i > 5)
				break;
			else
				k++;
		}
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
		int k = 0;
		for (int i = 0; i < 10; i++) {
			if (i > 5)
				break;
			k++;
		}
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

		[Test]
		public void TestContinueLoop()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int k = 0;
		for (int i = 0; i < 10; i++) {
			if (i > 5)
				continue;
			else
				k++;
		}
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
		int k = 0;
		for (int i = 0; i < 10; i++) {
			if (i > 5)
				continue;
			k++;
		}
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

		[Test]
		public void TestBlockStatement()
		{
			var input = @"
class TestClass
{
	int TestMethod (int i)
	{
		if (i > 0) {
			return 1;
		} else {
			return 0;
		}
	}
}";
			var output = @"
class TestClass
{
	int TestMethod (int i)
	{
		if (i > 0) {
			return 1;
		}
		return 0;
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

		[Test]
		public void TestEmptyFalseBlock()
		{
			var input = @"
class TestClass
{
	void TestMethod (int i)
	{
		int a;
		if (i > 0)
			a = 1;
		else { }
	}
}";
			var output = @"
class TestClass
{
	void TestMethod (int i)
	{
		int a;
		if (i > 0)
			a = 1;
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

		[Test]
		public void TestNecessaryElse()
		{

			var input = @"
class TestClass
{
	void TestMethod (int i)
	{
		int a;
		if (i > 0)
			a = 1;
		else
			a = 0;
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 0);
		}

		[Test]
		public void TestNecessaryElseCase2()
		{

			var input = @"
class TestClass
{
	void TestMethod (int i)
	{
		int a;
		while (true) {
			if (i > 0) {
				a = 1;
			} else if (i < 0) {
				a = 0;
				break;
			} else {
				break;
			}
		}
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 0);
		}

		[Test]

		public void TestNecessaryElseBecauseOfVarDeclaration()
		{

			var input = @"
class TestClass
{
	void TestMethod (int i)
	{
		if (i > 0) {
			int a = 1;
			return;
		} else {
			int a = 2;
			return;
		}
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 0);
		}

		[Test]
		public void TestNecessaryElseBecauseOfVarDeclarationInDifferentStatement()
		{
			var input = @"
class TestClass
{
	void TestMethod (int i)
	{
		{
			int a = 1;
		}
		if (i > 0) {
			return;
		} else {
			int a = 2;
			return;
		}
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 0);
		}

		[Test]
		public void TestReturn2()
		{
			var input = @"
class TestClass
{
	bool? EvaluateCondition (Expression expr)
	{
		ResolveResult rr = EvaluateConstant (expr);
		if (rr != null && rr.IsCompileTimeConstant)
			return rr.ConstantValue as bool?;
		else
			return null;
	}
}";
			var output = @"
class TestClass
{
	bool? EvaluateCondition (Expression expr)
	{
		ResolveResult rr = EvaluateConstant (expr);
		if (rr != null && rr.IsCompileTimeConstant)
			return rr.ConstantValue as bool?;
		return null;
	}
}";
			Test<RedundantIfElseBlockIssue>(input, 1, output);
		}

	}
}
