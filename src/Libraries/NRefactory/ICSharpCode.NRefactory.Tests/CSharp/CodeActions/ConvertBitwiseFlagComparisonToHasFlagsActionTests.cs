//
// ConvertBitwiseFlagComparisonToHasFlagsActionTests.cs
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
	public class ConvertBitwiseFlagComparisonToHasFlagsActionTests : ContextActionTestBase
	{
		[Test]
		public void TestComparisonWithNullInEqual ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & Foo.A) $!= 0);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (f.HasFlag (Foo.A));
	}
}
");
		}

		[Test]
		public void TestComparisonWithNullEqual ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & Foo.A) $== 0);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (!f.HasFlag (Foo.A));
	}
}
");
		}

		[Test]
		public void TestComparisonWithFlagInEqual ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & Foo.A) $!= Foo.A);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (!f.HasFlag (Foo.A));
	}
}
");
		}

		[Test]
		public void TestComparisonWithFlagEqual ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & Foo.A) $== Foo.A);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (f.HasFlag (Foo.A));
	}
}
");
		}
	
		[Test]
		public void TestMultipleFlags ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & (Foo.A | Foo.B)) $!= 0);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (f.HasFlag (Foo.A) | f.HasFlag (Foo.B));
	}
}
");
		}

		[Test]
		public void TestMultipleFlagsCase2 ()
		{
			Test<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & (Foo.A & Foo.B)) $!= 0);
	}
}
", @"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine (f.HasFlag (Foo.A | Foo.B));
	}
}
");
		}

		[Test]
		public void TestInvalid ()
		{
			TestWrongContext<ConvertBitwiseFlagComparisonToHasFlagsAction>(@"
[Flags]
enum Foo
{
	A, B
}

class FooBar
{
	public void Bar (Foo f)
	{
		Console.WriteLine ((f & (Foo.A | Foo.B & 1)) $!= 0);
	}
}
");
		}
	}
}

