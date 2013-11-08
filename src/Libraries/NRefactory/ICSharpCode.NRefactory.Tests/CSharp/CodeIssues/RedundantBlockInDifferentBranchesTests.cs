// 
// RedundantBlockInDifferentBranchesTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantBlockInDifferentBranchesTests : InspectionActionTestBase
	{
		[Test]
		public void TestConditionalExpression1()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int foo = 1;
		if (foo > 5)
		{
			foo = foo +  1;
			foo = foo + foo;
		}
		else
		{
			foo = foo + 1;
			foo = foo + foo;
		}
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 1);
		}
		
		[Test]
		public void TestConditionalExpression2()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int foo = 1;
		if (foo > 5)
			foo = foo + 1;
		else
			foo = foo + 1;
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 1);
		}
		
		[Test]
		public void TestConditionalExpression3()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int foo;
		if (foo > 5)
			foo = foo + 1;
		else
		{
			foo = foo + 1;
		}
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 1);
		}
		
		[Test]
		public void TestConditionalExpression4()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int foo = 1;
		if (foo > 5)
		{
		}
		else
		{}
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 1);
		}

		[Test]
		public void TestNoProblem1()
		{
			var input = @"
class TestClass
{
	void TestMethod (int foo)
	{
		if (foo > 5)
		{
			foo = foo + 1;
			return 2;
		}
		else
		{
			foo = foo + 1;
			return 2;
		}
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 0);
		}

		[Test]
		public void TestNoProblem2()
		{
			var input = @"
class TestClass
{
	void TestMethod (int foo)
	{
		if (foo > 5)
		{
			foo = foo + 1;
			return 2;
		}
		else
			return 5;
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 0);
		}
		
		[Test]
		public void TestResharperDisableRestore()
		{
			var input = @"
class TestClass
{
	void TestMethod (int foo)
	{
//Resharper disable RedundantBlockInDifferentBranches
		if (foo > 5)
			return 2;
		else
			return 5;
//Resharper restore RedundantBlockInDifferentBranches
	}
}";
			Test<RedundantBlockInDifferentBranchesIssue>(input, 0);
		}
	}
}