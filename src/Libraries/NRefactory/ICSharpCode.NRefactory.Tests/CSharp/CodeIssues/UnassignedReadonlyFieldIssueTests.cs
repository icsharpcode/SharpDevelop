//
// UnassignedReadonlyFieldIssueTests.cs
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
	public class UnassignedReadonlyFieldIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestField ()
		{
			Test<UnassignedReadonlyFieldIssue>(@"class Test
{
	readonly object fooBar;
}", @"class Test
{
	public Test (object fooBar)
	{
		this.fooBar = fooBar;
	}
	readonly object fooBar;
}");
		}

		[Test]
		public void TestValueTypeField ()
		{
			Test<UnassignedReadonlyFieldIssue>(@"class Test
{
	readonly int fooBar;
}", @"class Test
{
	public Test (int fooBar)
	{
		this.fooBar = fooBar;
	}
	readonly int fooBar;
}");
		}

		
		[Test]
		public void TestDisable ()
		{
			TestWrongContext<UnassignedReadonlyFieldIssue>(@"class Test
{
	// ReSharper disable once UnassignedReadonlyField.Compiler
	readonly object fooBar;
}");
		}

		
		[Test]
		public void TestPragmaDisable ()
		{
			TestWrongContext<UnassignedReadonlyFieldIssue>(@"class Test
{
	#pragma warning disable 649
	readonly int test;
	#pragma warning restore 649
}");
		}

		[Test]
		public void TestAlreadyInitalized ()
		{
			TestWrongContext<UnassignedReadonlyFieldIssue>(@"class Test
{
	public Test (object fooBar)
	{
		this.fooBar = fooBar;
	}
	readonly object fooBar;
}");
		}

		[Test]
		public void TestAlreadyInitalizedCase2 ()
		{
			TestWrongContext<UnassignedReadonlyFieldIssue>(@"
using System;
public class FooBar
{
	sealed class Bar
	{
		public int foo;
	}

	readonly string foo;
	
	public FooBar()
	{
		this.foo = """";
	}
}
");
		}
	}
}

