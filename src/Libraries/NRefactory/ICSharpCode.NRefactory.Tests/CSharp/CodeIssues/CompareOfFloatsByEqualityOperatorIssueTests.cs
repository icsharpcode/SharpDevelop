// 
// CompareFloatWithEqualityOperatorIssueTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CompareOfFloatsByEqualityOperatorIssueTests : InspectionActionTestBase
	{
		static void Test (string inputOp, string outputOp)
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		double x = 0.1;
		bool test = x " + inputOp + @" 0.1;
		bool test2 = x " + inputOp + @" 1ul;
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
		double x = 0.1;
		bool test = System.Math.Abs (x - 0.1) " + outputOp + @" EPSILON;
		bool test2 = System.Math.Abs (x - 1ul) " + outputOp + @" EPSILON;
	}
}";
			Test<CompareOfFloatsByEqualityOperatorIssue> (input, 2, output);
		}

		[Test]
		public void TestEquality ()
		{
			Test ("==", "<");
		}

		[Test]
		public void TestInequality ()
		{
			Test ("!=", ">");
		}

		[Test]
		public void TestZero ()
		{
			Test<CompareOfFloatsByEqualityOperatorIssue> (@"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = x == 0;
		bool test2 = 0.0e10 != x;
		bool test3 = 0L == y;
		bool test4 = y != 0.0000;
	}
}", 4, @"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = System.Math.Abs (x) < EPSILON;
		bool test2 = System.Math.Abs (x) > EPSILON;
		bool test3 = System.Math.Abs (y) < EPSILON;
		bool test4 = System.Math.Abs (y) > EPSILON;
	}
}");
		}

		[Test]
		public void TestNaN ()
		{
			Test<CompareOfFloatsByEqualityOperatorIssue> (@"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = x == System.Double.NaN;
		bool test2 = x != double.NaN;
		bool test3 = y == float.NaN;
		bool test4 = x != float.NaN;
	}
}", 4, @"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = double.IsNaN (x);
		bool test2 = !double.IsNaN (x);
		bool test3 = float.IsNaN (y);
		bool test4 = !double.IsNaN (x);
	}
}");
		}
		
		
		[Test]
		public void TestPositiveInfinity ()
		{

			Test<CompareOfFloatsByEqualityOperatorIssue> (@"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = x == System.Double.PositiveInfinity;
		bool test2 = x != double.PositiveInfinity;
		bool test3 = y == float.PositiveInfinity;
		bool test4 = x != float.PositiveInfinity;
	}
}", 4, @"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = double.IsPositiveInfinity (x);
		bool test2 = !double.IsPositiveInfinity (x);
		bool test3 = float.IsPositiveInfinity (y);
		bool test4 = !double.IsPositiveInfinity (x);
	}
}");
		}
		
		[Test]
		public void TestNegativeInfinity ()
		{
			Test<CompareOfFloatsByEqualityOperatorIssue> (@"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = x == System.Double.NegativeInfinity;
		bool test2 = x != double.NegativeInfinity;
		bool test3 = y == float.NegativeInfinity;
		bool test4 = x != float.NegativeInfinity;
	}
}", 4, @"
class TestClass
{
	void TestMethod (double x, float y)
	{
		bool test = double.IsNegativeInfinity (x);
		bool test2 = !double.IsNegativeInfinity (x);
		bool test3 = float.IsNegativeInfinity (y);
		bool test4 = !double.IsNegativeInfinity (x);
	}
}");
		}
	
		[Test]
		public void TestDisable()
		{
			TestWrongContext<CompareOfFloatsByEqualityOperatorIssue> (@"
class TestClass
{
	void TestMethod (double x, float y)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (x == y)
			System.Console.WriteLine (x);
	}
}");

		}
	}
}
