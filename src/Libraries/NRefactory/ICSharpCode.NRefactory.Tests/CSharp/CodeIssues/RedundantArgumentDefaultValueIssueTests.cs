//
// RedundantArgumentDefaultValueIssueTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantArgumentDefaultValueIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleCase()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		Bar (s);
	}
}
", @"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		Bar ();
	}
}
");
		}


		[Test]
		public void TestNamedArgument()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar(foo: 22);
	}
}
", @"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar ();
	}
}
");
		}

		[Test]
		public void TestMultipleRemoveFirst()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s, 3);
	}
}
", 2, @"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar ();
	}
}
", 0);
		}

		[Test]
		public void TestMultipleRemoveSecond()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s, 3);
	}
}
", 2, @"
class Test
{ 
	public void Bar (int foo = 22, int test = 3)
	{
		const int s = 22;
		Bar (s);
	}
}
", 1);
		}
	
		[Test]
		public void TestInvalid()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		Bar (21);
	}
}");
		}

		[Test]
		public void TestInvalidCase2()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22, int bar = 3)
	{
		Bar (22, 4);
	}
}");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public void Bar (int foo = 22)
	{
		const int s = 22;
		// ReSharper disable once RedundantArgumentDefaultValue
		Bar (s);
	}
}
");
		}
	
		[Test]
		public void TestConstructor()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public Test (int a, int b, int c = 4711)
	{
		new Test (1, 2, 4711);
	}
}
", @"
class Test
{ 
	public Test (int a, int b, int c = 4711)
	{
		new Test (1, 2);
	}
}
");
		}

		[Test]
		public void TestIndexer()
		{
			Test<RedundantArgumentDefaultValueIssue>(@"
class Test
{ 
	public int this [int a, int b = 2] {
		get {
			return this [a, 2];
		}
	}
}
", @"
class Test
{ 
	public int this [int a, int b = 2] {
		get {
			return this [a];
		}
	}
}
");
		}
	}
}

