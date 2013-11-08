//
// FlipEqualsTargetAndArgumentActionTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class FlipEqualsTargetAndArgumentActionTests : ContextActionTestBase
	{
		[Test]
		public void TestSimpleCase ()
		{
			Test<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x.$Equals (y))
			Console.WriteLine (x);
	}
}", @"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (y.Equals (x))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestComplexCase ()
		{
			Test<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x.$Equals (1 + 2))
			Console.WriteLine (x);
	}
}", @"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if ((1 + 2).Equals (x))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestRemoveParens ()
		{
			Test<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if ((1 + 2).Equals $(x))
			Console.WriteLine (x);
	}
}", @"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x.Equals (1 + 2))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestUnaryOperatorCase ()
		{
			Test<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, bool y)
	{
		if (x.$Equals (!y))
			Console.WriteLine (x);
	}
}", @"
class Foo
{
	public void FooFoo (object x, bool y)
	{
		if ((!y).Equals (x))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestNullCase ()
		{
			TestWrongContext<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x.$Equals (null))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestStaticCase ()
		{
			TestWrongContext<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public static bool Equals (object a) { return false; }

	public void FooFoo (object x, object y)
	{
		if (Foo.$Equals (x))
			Console.WriteLine (x);
	}
}");
		}

		[Test]
		public void TestCaretLocation ()
		{
			TestWrongContext<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x$.Equals (y))
			Console.WriteLine (x);
	}
}");
			TestWrongContext<FlipEqualsTargetAndArgumentAction>(@"
class Foo
{
	public void FooFoo (object x, object y)
	{
		if (x.Equals ($y))
			Console.WriteLine (x);
	}
}");
		}
	}
}

