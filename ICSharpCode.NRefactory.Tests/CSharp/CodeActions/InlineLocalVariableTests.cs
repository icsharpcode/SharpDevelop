// 
// InlineLocalVariableTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
	public class InlineLocalVariableTests : ContextActionTestBase
	{
		[Test]
		public void TestSimpleInline ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		int $tmp = 5 + 6;
		Console.WriteLine (tmp);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine (5 + 6);
	}
}");
		}

		[Test]
		public void TestAddNoParensForSimpleReplacement ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test (int foo)
	{
		int $tmp = foo;
		Console.WriteLine (tmp.ToString ());
	}
}", @"class TestClass
{
	void Test (int foo)
	{
		Console.WriteLine (foo.ToString ());
	}
}");
		}

		[Test]
		public void TestAddNoParensForMemberReferenceReplacement ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	static int Foo;
	void Test ()
	{
		int $tmp = TestClass.Foo;
		Console.WriteLine (tmp.ToString ());
	}
}", @"class TestClass
{
	static int Foo;
	void Test ()
	{
		Console.WriteLine (TestClass.Foo.ToString ());
	}
}");
		}

		[Test]
		public void TestAddParens ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		int $tmp = 5 + 6;
		Console.WriteLine (tmp.ToString ());
		Console.WriteLine (tmp * 3);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine ((5 + 6).ToString ());
		Console.WriteLine ((5 + 6) * 3);
	}
}");
		}

		[Test]
		public void TestAddParensConditionalOperatorCase ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		bool $tmp = 3 == 5;
		Console.WriteLine (tmp ? tmp : 2);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine ((3 == 5) ? 3 == 5 : 2);
	}
}");
		}

		[Test]
		public void TestAddParensAsOperatorCase ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		object $tmp = 3 == 5;
		Console.WriteLine (tmp as bool);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine ((3 == 5) as bool);
	}
}");
		}


		[Test]
		public void TestAddParensIsOperatorCase ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		object $tmp = 3 == 5;
		Console.WriteLine (tmp is bool);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine ((3 == 5) is bool);
	}
}");
		}

		[Test]
		public void TestAddParensCastOperatorCase ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		object $tmp = 3 == 5;
		Console.WriteLine ((bool)tmp);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine ((bool)(3 == 5));
	}
}");
		}



		[Test]
		public void TestPointerReferenceExpression ()
		{
			Test<InlineLocalVariableAction> (@"unsafe class TestClass
{
	public void Test (int* foo)
	{
		int* $ptr = foo + 5;
		Console.WriteLine (ptr->ToString ());
	}
}", @"unsafe class TestClass
{
	public void Test (int* foo)
	{
		Console.WriteLine ((foo + 5)->ToString ());
	}
}");
		}


		[Test]
		public void TestUnaryOperatorCase ()
		{
			Test<InlineLocalVariableAction> (@"class TestClass
{
	void Test ()
	{
		bool $tmp = 3 == 5;
		Console.WriteLine (!tmp);
	}
}", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine (!(3 == 5));
	}
}");
		}


	}
}