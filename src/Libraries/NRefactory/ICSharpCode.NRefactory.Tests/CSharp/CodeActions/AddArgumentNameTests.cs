// 
// AddArgumentNameTests.cs
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
	public class AddArgumentNameTests : ContextActionTestBase
	{
		[Test]
		public void MethodInvocation1()
		{
			Test<AddArgumentNameAction>(@"
class TestClass
{
	public void Foo(int a, int b, float c = 0.1){}
	public void F()
	{
		Foo($1,b: 2);
	}
}", @"
class TestClass
{
	public void Foo(int a, int b, float c = 0.1){}
	public void F()
	{
		Foo (a: 1, b: 2);
	}
}");
		}

		[Test]
		public void MethodInvocation2()
		{
			Test<AddArgumentNameAction>(@"
class TestClass
{
	public void Foo(int a, int b, float c = 0.1){}
	public void F()
	{
		Foo($1, 2);
	}
}", @"
class TestClass
{
	public void Foo(int a, int b, float c = 0.1){}
	public void F()
	{
		Foo (a: 1, b: 2);
	}
}");
		}

		[Test]
		public void AttributeUsage()
		{
			Test<AddArgumentNameAction>(@"
using System;
public class AnyClass
{
	[Obsolete($"" "", error: true)]
	static void Old() { }
}", @"
using System;
public class AnyClass
{
	[Obsolete(message: "" "", error: true)]
	static void Old() { }
}");
		}

		[Test]
		public void AttributeNamedArgument()
		{
			Test<AddArgumentNameAction>(@"
class MyAttribute : System.Attribute
{
	public string Name1 { get; set; }
	public string Name2 { get; set; }
	public string Name3 { get; set; }
	private int foo;

	public MyAttribute(int foo)
	{
		this.foo = foo;
	}
}


[My($1, Name1 = """", Name2 = """")]
public class Test
{
}
", @"
class MyAttribute : System.Attribute
{
	public string Name1 { get; set; }
	public string Name2 { get; set; }
	public string Name3 { get; set; }
	private int foo;

	public MyAttribute(int foo)
	{
		this.foo = foo;
	}
}


[My(foo: 1, Name1 = """", Name2 = """")]
public class Test
{
}
");
		}

		[Test]
		public void AttributeNamedArgumentInvalidCase()
		{
			TestWrongContext<AddArgumentNameAction>(@"
class MyAttribute : System.Attribute
{
	public string Name1 { get; set; }
	public string Name2 { get; set; }
	public string Name3 { get; set; }
	private int foo;

	public MyAttribute(int foo)
	{
		this.foo = foo;
	}
}


[My(1, $Name1 = """", Name2 = """")]
public class Test
{
}
");
		}

		[Test]
		public void IndexerInvocation()
		{
			Test<AddArgumentNameAction>(@"
public class TestClass
{
	public int this[int i, int j]
	{
		set { }
		get { return 0; }
	}
}
internal class Test
{
	private void Foo()
	{
		var TestBases = new TestClass();
		int a = TestBases[ $1, 2];
	}
}", @"
public class TestClass
{
	public int this[int i, int j]
	{
		set { }
		get { return 0; }
	}
}
internal class Test
{
	private void Foo()
	{
		var TestBases = new TestClass();
		int a = TestBases [i: 1, j: 2];
	}
}");
		}
	
		[Test]
		public void TestParamsInvalidContext()
		{

			TestWrongContext<AddArgumentNameAction>(@"
class TestClass
{
	public void F()
	{
		System.Console.WriteLine (""foo"", 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, $12);
	}
}");
		}

	}
}