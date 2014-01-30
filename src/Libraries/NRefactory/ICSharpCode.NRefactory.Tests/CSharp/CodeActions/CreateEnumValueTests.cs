//
// CreateEnumValueTests.cs
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
	public class CreateEnumValueTests: ContextActionTestBase
	{
		[Test]
		public void TestSimpleEnum()
		{
			Test<CreateEnumValue>(
				@"enum Foo
{
    Bar
}

class Baz
{
    public Baz (Foo foo)
    {
    }
}

class Program
{
    public void Main ()
    {
        var b = new Baz (Foo.$Something);
    }
}
",@"enum Foo
{
	Something,
    Bar
}

class Baz
{
    public Baz (Foo foo)
    {
    }
}

class Program
{
    public void Main ()
    {
        var b = new Baz (Foo.Something);
    }
}
");
		}

		/// <summary>
		/// Bug 16663 - Enum constants with Flags are not recognized
		/// </summary>
		[Test]
		public void TestBug16663()
		{
			Test<CreateEnumValue>(@"
public enum MyEnum
{
}

class Test  
{
	static void Main ()
	{
		var e = MyEnum.$NotDefinedYetValue;
	}
}
",@"
public enum MyEnum
{
	NotDefinedYetValue
}

class Test  
{
	static void Main ()
	{
		var e = MyEnum.NotDefinedYetValue;
	}
}
");
		}


		[Test]
		public void TestBug16663_Case2()
		{
			Test<CreateEnumValue>(@"
public enum MyEnum
{
}

class Test  
{
	static void Main ()
	{
		var e = MyEnum.$NotDefinedYetValue | MyEnum.Foo;
	}
}
",@"
public enum MyEnum
{
	NotDefinedYetValue
}

class Test  
{
	static void Main ()
	{
		var e = MyEnum.NotDefinedYetValue | MyEnum.Foo;
	}
}
");
		}
	}
}

