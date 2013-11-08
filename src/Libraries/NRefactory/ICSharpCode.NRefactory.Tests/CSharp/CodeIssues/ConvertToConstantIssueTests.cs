//
// ConvertToConstantIssueTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertToConstantIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestBasicCase ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	public static void Main (string[] args)
	{
		const int fooBar = 12;
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestWrongLocalType ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		object fooBar = 12;
	}
}");
		}

		[Test]
		public void TestChangingVariableCase1 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		Console.WriteLine (fooBar++);
	}
}");
		}
		
		[Test]
		public void TestChangingVariableCase2 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		Something (out fooBar);
	}
}");
		}

		[Test]
		public void TestChangingVariableCase3 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		if (args.Length == 10)
			fooBar = 20;
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestChangingVariableCase4 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		switch (args[0]) {
			case ""Hello"":
				fooBar = 20;
				break;
		}
		Console.WriteLine (fooBar);
	}
}");
		}

		
		[Test]
		public void TestChangingVariableCase5 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		fooBar += 1;
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestChangingVariableCase6 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		int fooBar = 12;
		TestMe (ref fooBar);
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		// ReSharper disable once ConvertToConstant.Local
		int fooBar = 12;
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestField ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	const int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestReadonlyField ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	readonly int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestStaticField ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	static int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	const int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestChangingField ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public Test ()
	{
		fooBar = 2323;
	}
	int fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestWrongFieldType ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	object fooBar = 12;
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestChangingFieldCase2 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	int fooBar = 12;
	public Test ()
	{
		this.fooBar = 12;
	}
}");
		}

		[Test]
		public void TestChangingFieldCase3 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	int fooBar = 12;
	public Test ()
	{
		++this.fooBar;
	}
}");
		}

		[Test]
		public void TestShadowedField ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	int fooBar = 12;

	public Test (int fooBar)
	{
		fooBar = 2323;
	}

	public void Bar ()
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	const int fooBar = 12;

	public Test (int fooBar)
	{
		fooBar = 2323;
	}

	public void Bar ()
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestShadowedFieldCase2 ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	int fooBar = 12;

	public Test ()
	{
		int fooBar;
		fooBar = 2323;
	}

	public void Bar ()
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	const int fooBar = 12;

	public Test ()
	{
		int fooBar;
		fooBar = 2323;
	}

	public void Bar ()
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestNeverSuggestForControlVariable ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		for (int i = 0; i < 10;) {
			Console.WriteLine(i);
		}
	}
}");
		}


		[Test]
		public void TestVarCase ()
		{
			Test<ConvertToConstantIssue>(@"class Test
{
	public static void Main (string[] args)
	{
		var fooBar = 12;
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	public static void Main (string[] args)
	{
		const int fooBar = 12;
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestArbitraryStructCase ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"
struct Bar {
	public int A;
}

class Test
{
	public static void Main (string[] args)
	{
		var fooBar = default(Bar);
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestArbitraryStructCase2 ()
		{
			TestWrongContext<ConvertToConstantIssue>(@"
struct Bar {
	public int A;
}

class Test
{
	const Bar foo = new Bar();
	public static void Main (string[] args)
	{
		var fooBar = foo;
	}
}");
		}


		[Test]
		public void TestComplexCase ()
		{
			Test<ConvertToConstantIssue>(@"
class Test
{
	public static void Main (string[] args)
	{
		var pi2 = System.Math.PI * 2;
		Console.WriteLine (pi2);
	}
}", @"
class Test
{
	public static void Main (string[] args)
	{
		const double pi2 = System.Math.PI * 2;
		Console.WriteLine (pi2);
	}
}");
		}

	}
}

