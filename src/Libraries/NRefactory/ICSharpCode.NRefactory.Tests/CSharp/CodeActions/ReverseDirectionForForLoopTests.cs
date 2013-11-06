//
// ReverseDirectionForForLoopTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ReverseDirectionForForLoopTests : ContextActionTestBase
	{
		[Test]
		public void TestPrimitveExpression()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; i < 10; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 9; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestPrimitveExpressionCase2()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; 10 > i; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 9; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestPrimitveExpressionReverse()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 9; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 0; i < 10; i++)
			System.Console.WriteLine (i);
	}
}
");
		}
	
		[Test]
		public void TestLowerEqualPrimitveExpression()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; i <= 10; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 10; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestLowerEqualPrimitveExpressionCase2()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; 10 >= i; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 10; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}
	
		[Test]
		public void TestArbitraryBounds()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar (int from, int to)
	{
		$for (int i = from; i < to; i += 1)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar (int from, int to)
	{
		for (int i = to - 1; i >= from; i--)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestArbitraryBoundsCase2()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; i < a.T; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = a.T - 1; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}


		[Test]
		public void TestComplex()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = Foo ().Bar + Test; i < (to - from).Length; i += a + b)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = (to - from).Length - (a + b); i >= Foo ().Bar + Test; i -= a + b)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestComplexReverse()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = (to - from).Length - (a + b); i >= Foo ().Bar + Test; i -= a + b)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = Foo ().Bar + Test; i < (to - from).Length; i += a + b)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestArbitraryBoundsReverse()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar (int from, int to)
	{
		$for (int i = to - 1; i >= from; --i)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar (int from, int to)
	{
		for (int i = from; i < to; i++)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestPrimitiveExpressionArbitrarySteps()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; i < 100; i += 5)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 95; i >= 0; i -= 5)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestPrimitiveExpressionArbitraryStepsCase2()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0; i <= 100; i += 5)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 100; i >= 0; i -= 5)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestPrimitiveExpressionArbitraryStepsReverse()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 95; i >= 0; i -= 5)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 0; i < 100; i += 5)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestArbitrarySteps()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar (int from, int to, int step)
	{
		$for (int i = from; i < to; i += step)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar (int from, int to, int step)
	{
		for (int i = to - step; i >= from; i -= step)
			System.Console.WriteLine (i);
	}
}
");
		}

		[Test]
		public void TestArbitraryStepsReverse()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar (int from, int to, int step)
	{
		$for (int i = to - step; i >= from; i -= step)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar (int from, int to, int step)
	{
		for (int i = from; i < to; i += step)
			System.Console.WriteLine (i);
	}
}
");
		}
	
		[Ignore("Implement me")]
		[Test]
		public void TestOptimizedFor()
		{
			Test<ReverseDirectionForForLoopAction>(@"
class Foo
{
	public void Bar ()
	{
		$for (int i = 0, upper = 10; i < upper; i++)
			System.Console.WriteLine (i);
	}
}
", @"
class Foo
{
	public void Bar ()
	{
		for (int i = 9; i >= 0; i--)
			System.Console.WriteLine (i);
	}
}
");
		}


	}
}

