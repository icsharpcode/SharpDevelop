// 
// ConvertIfStatementToNullCoalescingExpressionActionTests.cs
//  
// Author:
//       Luís Reis <luiscubal@gmail.com>
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
	public class ConvertIfStatementToNullCoalescingExpressionActionTests : ContextActionTestBase
	{
		[Test]
		public void TestDeclaration ()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo ();
		$if (o == null)
			o = new object ();
	}
}
";

			string output = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo () ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestYodaConditionals ()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo ();
		$if (null == o)
			o = new object ();
	}
}
";

			string output = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo () ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestAssignment ()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o;
		o = Foo ();
		$if (o == null)
			o = new object ();
	}
}
";

			string output = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o;
		o = Foo () ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestIsolated ()
		{
			string input = @"
class TestClass
{
	object o;
	void TestMethod()
	{
		$if (o == null)
			o = new object ();
	}
}
";

			string output = @"
class TestClass
{
	object o;
	void TestMethod()
	{
		o = o ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestBlock ()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo ();
		$if (o == null)
		{
			o = new object ();
		}
	}
}
";

			string output = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo () ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestInvertedCondition ()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo ();
		$if (o != null)
		{
		} else {
			o = new object ();
		}
	}
}
";

			string output = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo () ?? new object ();
	}
}
";

			Assert.AreEqual(output, RunContextAction(new ConvertIfStatementToNullCoalescingExpressionAction(), input));
		}

		[Test]
		public void TestDisabledForImproperCondition()
		{
			string input = @"
class TestClass
{
	void Foo()
	{
		return null;
	}
	void TestMethod()
	{
		object o = Foo ();
		$if (o != null)
		{
			o = new object ();
		}
	}
}
";

			TestWrongContext<ConvertIfStatementToNullCoalescingExpressionAction>(input);
		}
	}
}
