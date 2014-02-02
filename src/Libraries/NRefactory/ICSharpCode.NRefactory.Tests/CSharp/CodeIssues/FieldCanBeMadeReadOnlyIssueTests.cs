//
// FieldCanBeMadeReadOnlyIssueTests.cs
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
	public class FieldCanBeMadeReadOnlyIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestInitializedField ()
		{
			Test<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	object fooBar = new object ();
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	readonly object fooBar = new object ();
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestFieldAssignedInConstructor ()
		{
			Test<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	object fooBar;
	public Test ()
	{
		fooBar = new object ();
	}
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}", @"class Test
{
	readonly object fooBar;
	public Test ()
	{
		fooBar = new object ();
	}
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}
	
		[Test]
		public void TestDisable ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	// ReSharper disable once FieldCanBeMadeReadOnly.Local
	object fooBar = new object ();
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestFactoryMethod ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	object fooBar;
	
	public static Test Create ()
	{
		var result = new Test ();
		result.fooBar = new object ();
		return result;
	}
}");
		}

		[Test]
		public void TestFactoryMethodCase2 ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	object fooBar;
	
	public static Test Create ()
	{
		var result = new Test {fooBar = new object () };
		return result;
	}
}");
		}


		[Test]
		public void TestUninitalizedValueTypeField ()
		{
			Test<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	int fooBar;
	public Test ()
	{
		fooBar = 5;
	}
}", @"class Test
{
	readonly int fooBar;
	public Test ()
	{
		fooBar = 5;
	}
}");
		}

		[Test]
		public void TestInitalizedValueTypeField ()
		{
			// Is handled by the 'to const' issue.
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	int fooBar = 12;
	public void FooBar ()
	{
		System.Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestSpecializedFieldBug ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"
using System;
class Test<T> where T : IDisposable
{
	object fooBar = new object ();
	public void Foo ()
	{
		fooBar = null;
	}
}");
		}


		[Test]
		public void TestFieldAssignedInConstructorLambda ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"
using System;

class Test
{
	object fooBar;
	public Action<object> act;
	public Test ()
	{
		act = o => { fooBar = o; };
	}
}");
		}

		[Test]
		public void MutableStruct ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	MutableStruct m;
	public static void Main (string[] args)
	{
		m.Increment();
	}
}
struct MutableStruct {
	int val;
	public void Increment() {
		val++;
	}
}
");
		}

		[Test]
		public void TestUnassignedField ()
		{
			TestWrongContext<FieldCanBeMadeReadOnlyIssue>(@"class Test
{
	object fooBar;
}");
		}

	}
}

