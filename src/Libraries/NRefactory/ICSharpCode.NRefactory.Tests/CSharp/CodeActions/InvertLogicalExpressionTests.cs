// 
// InvertLogicalExpressionTests.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class InvertLogicalExpressionTests : ContextActionTestBase
	{
		[Test]
		public void ConditionlAnd()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a $&& b){}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(!a || !b)) {
		}
	}
}");
		}

		[Test]
		public void ConditionlAndReverse()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(!a $|| !b)) {
		}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a && b) {
		}
	}
}");
		}

		[Test]
		public void ConditionlOr()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a $|| b){}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(!a && !b)) {
		}
	}
}");
		}

		[Test]
		public void ConditionlOrReverse()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(!a $&& !b)){}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a || b) {
		}
	}
}");
		}


		[Test]
		public void ConditionlAnd2()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		int a = 1;
		bool b = true;
		if ((a > 1) $&& b){}
	}
}", @"
class TestClass
{
	public void F()
	{
		int a = 1;
		bool b = true;
		if (!((a <= 1) || !b)) {
		}
	}
}");
		}

		[Test]
		public void ConditionlOr2()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		int a = 1;
		bool b = true;
		if (!((a > 1) $|| b)){}
	}
}", @"
class TestClass
{
	public void F()
	{
		int a = 1;
		bool b = true;
		if ((a <= 1) && !b) {
		}
	}
}");
		}
	
		[Test]
		public void Equals()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a $== b){}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(a != b)) {
		}
	}
}");
		}

		[Test]
		public void EqualsReverse()
		{
			Test<InvertLogicalExpressionAction>(@"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (!(a $!= b)){}
	}
}", @"
class TestClass
{
	public void F()
	{
		bool a = true;
		bool b = false;
		if (a == b) {
		}
	}
}");
		}

		
		[Test]
		public void TestNullCoalescing()
		{
			TestWrongContext<InvertLogicalExpressionAction>(@"
class Foo
{
	void Bar (object i, object j)
	{
		Console.WriteLine (i $?? j);
	}
}
");
		}


		[Test]
		public void TestUnaryExpression()
		{
			Test<InvertLogicalExpressionAction>(@"
class Foo
{
	void Bar (bool a, bool b)
	{
		Console.WriteLine ($!(a && b));
	}
}
", @"
class Foo
{
	void Bar (bool a, bool b)
	{
		Console.WriteLine (!a || !b);
	}
}
");
		}
	}
}
