// AutoLinqTests.cs
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

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class AutoLinqSumActionTests : ContextActionTestBase
	{
		[Test]
		public void TestSimpleIntegerLoop() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestMergedIntegerLoop() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result = 0;
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result = list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestNonZeroMergedIntegerLoop() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result = 1;
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result = 1 + list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestMergedAssignmentIntegerLoop() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result;
		result = 1;
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		int result;
		result = 1 + list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestMergedDecimal() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		decimal result = 0.0m;
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		var list = new int[] { 1, 2, 3 };
		decimal result = list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestIntegerLoopInBlock() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result += x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestExpression() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result += x * 2;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum (x => x * 2);
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestDisabledForStrings() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		string result = string.Empty;
		var list = new string[] { ""a"", ""b"" };
		$foreach (var x in list) {
			result += x;
		}
	}
}";
			TestWrongContext<AutoLinqSumAction>(source);
		}

		[Test]
		public void TestShort() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		short result = 0;
		var list = new short[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		short result = 0;
		var list = new short[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestLong() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		long result = 0;
		var list = new long[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		long result = 0;
		var list = new long[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestUnsignedLong() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		ulong result = 0;
		var list = new ulong[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		ulong result = 0;
		var list = new ulong[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestFloat() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		float result = 0;
		var list = new float[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		float result = 0;
		var list = new float[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestDouble() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		double result = 0;
		var list = new double[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		double result = 0;
		var list = new double[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestDecimal() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		decimal result = 0;
		var list = new decimal[] { 1, 2, 3 };
		$foreach (var x in list)
			result += x;
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		decimal result = 0;
		var list = new decimal[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestMinus() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result -= x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum (x => -x);
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestCombined() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result += x;
			result += 2 * x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum (x => x + 2 * x);
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestCombinedPrecedence() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result += x;
			result += x << 1;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum (x => x + (x << 1));
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestEmptyStatements() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result += x;
			;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestSimpleConditional() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			if (x > 0)
				result += x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Where (x => x > 0).Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestInvertedConditional() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			if (x > 0)
				;
			else
				result += x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Where (x => x <= 0).Sum ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestIncrement() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			result++;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Count ();
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestCompleteConditional() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		$foreach (var x in list) {
			if (x > 0)
				result += x * 2;
			else
				result += x;
		}
	}
}";

			string result = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2, 3 };
		result += list.Sum (x => x > 0 ? x * 2 : x);
	}
}";

			Assert.AreEqual(result, RunContextAction(new AutoLinqSumAction(), source));
		}

		[Test]
		public void TestDisabledForSideEffects() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		string result = string.Empty;
		var list = new string[] { ""a"", ""b"" };
		$foreach (var x in list) {
			TestMethod();
			result += x;
		}
	}
}";
			TestWrongContext<AutoLinqSumAction>(source);
		}

		[Test]
		public void TestDisabledForInnerAssignments() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2 };
		int p = 0;
		$foreach (var x in list) {
			result += (p = x);
		}
	}
}";
			TestWrongContext<AutoLinqSumAction>(source);
		}

		[Test]
		public void TestDisabledForInnerIncrements() {
			string source = @"
using System.Linq;

class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2 };
		int p = 0;
		$foreach (var x in list) {
			result += (p++);
		}
	}
}";
			TestWrongContext<AutoLinqSumAction>(source);
		}

		[Test]
		public void TestDisabledForNoLinq() {
			string source = @"
class TestClass
{
	void TestMethod() {
		int result = 0;
		var list = new int[] { 1, 2 };
		$foreach (var x in list) {
			result += x;
		}
	}
}";
			TestWrongContext<AutoLinqSumAction>(source);
		}
	}
}

