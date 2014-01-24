//
// RedundantUnsafeContextIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantUnsafeContextIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestUnsafeClass ()
		{
			Test<RedundantUnsafeContextIssue>(@"
unsafe class Foo
{
	public static void Main (string[] args)
	{
		System.Console.WriteLine (""Hello World!"");
	}
}
", @"
class Foo
{
	public static void Main (string[] args)
	{
		System.Console.WriteLine (""Hello World!"");
	}
}
");
		}

		[Test]
		public void TestUnsafeStatement ()
		{
			Test<RedundantUnsafeContextIssue>(@"
class Foo
{
	public static void Main (string[] args)
	{
		unsafe {
			System.Console.WriteLine (""Hello World1!"");
			System.Console.WriteLine (""Hello World2!"");
		}
	}
}
", @"
class Foo
{
	public static void Main (string[] args)
	{
		System.Console.WriteLine (""Hello World1!"");
		System.Console.WriteLine (""Hello World2!"");
	}
}
");
		}

		[Test]
		public void TestNestedUnsafeStatement ()
		{
			Test<RedundantUnsafeContextIssue>(@"
unsafe class Program
{
	static void Main(string str)
	{
		unsafe {
			fixed (char* charPtr = &str) {
				*charPtr = 'A';
			}
		}
	}
}
", @"
unsafe class Program
{
	static void Main(string str)
	{
		fixed (char* charPtr = &str) {
			*charPtr = 'A';
		}
	}
}
");
		}

		[Test]
		public void TestValidFixedPointer ()
		{
			TestWrongContext<RedundantUnsafeContextIssue>(@"
unsafe struct Foo {
	public fixed char fixedBuffer[128];
}
");
		}


		[Test]
		public void TestDisable ()
		{
			TestWrongContext<RedundantUnsafeContextIssue>(@"
unsafe class Foo
{
	public static void Main (string[] args)
	{
		// ReSharper disable once RedundantUnsafeContext
		System.Console.WriteLine (""Hello World!"");
	}
}
");
		}

		[Test]
		public void TestSizeOf ()
		{
			TestWrongContext<RedundantUnsafeContextIssue>(@"
public static class TestClass
{
	struct TestStruct {
	}
	public static void Main(String[] args)
	{
		unsafe {
			int a = sizeof(TestStruct);
		}
	}
}");
		}

		[Test]
		public void TestFixed ()
		{
			TestWrongContext<RedundantUnsafeContextIssue>(@"
class Foo
{
	unsafe struct TestStruct
	{
		public fixed byte TestVar[32];
	}
}
");
		}
	}
}

