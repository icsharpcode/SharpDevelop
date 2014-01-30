//
// ConvertMethodGroupToAnonymousMethodActionTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ConvertMethodGroupToAnonymousMethodActionTests : ContextActionTestBase
	{
		[Test]
		public void TestVoidMethod ()
		{
			Test<ConvertMethodGroupToAnonymousMethodAction>(@"
using System;
public class Test
{
	void Foo ()
	{
		Action act = $Foo;
	}
}
", @"
using System;
public class Test
{
	void Foo ()
	{
		Action act = delegate {
	Foo ();
};
	}
}
");
		}

		[Test]
		public void TestParameter ()
		{
			Test<ConvertMethodGroupToAnonymousMethodAction>(@"
using System;
public class Test
{
	void Foo (int x, int y)
	{
		Action<int,int> act = $Foo;
	}
}
", @"
using System;
public class Test
{
	void Foo (int x, int y)
	{
		Action<int,int> act = delegate (int arg1, int arg2) {
	Foo (arg1, arg2);
};
	}
}
");
		}
	
		[Test]
		public void TestFunction ()
		{
			Test<ConvertMethodGroupToAnonymousMethodAction>(@"
using System;
public class Test
{
	bool Foo (int x, int y)
	{
		Func<int,int,bool> act = $Foo;
	}
}
", @"
using System;
public class Test
{
	bool Foo (int x, int y)
	{
		Func<int,int,bool> act = delegate (int arg1, int arg2) {
	return Foo (arg1, arg2);
};
	}
}
");
		}
	
		[Test]
		public void TestOverloads ()
		{
			Test<ConvertMethodGroupToAnonymousMethodAction>(@"
using System;
public class Test
{
	static void Foo (int x) { }
	static void Foo (int x, int y) { }
	static void Foo () { }

	void Bar ()
	{
		Action<int, int> act = Test.$Foo;
	}
}", @"
using System;
public class Test
{
	static void Foo (int x) { }
	static void Foo (int x, int y) { }
	static void Foo () { }

	void Bar ()
	{
		Action<int, int> act = delegate (int arg1, int arg2) {
			Test.Foo (arg1, arg2);
		};
	}
}");
		}
	
	
	}
}

