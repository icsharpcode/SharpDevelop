//
// RedundantParamsIssueTests.cs
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
	public class RedundantParamsIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestBasicCase ()
		{
			Test<RedundantParamsIssue>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, params object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}", @"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
		}

		[Test]
		public void TestValidCase ()
		{
			TestWrongContext<RedundantParamsIssue>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	public override void Foo(string fmt, object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<RedundantParamsIssue>(@"class FooBar
{
	public virtual void Foo(string fmt, object[] args)
	{
	}
}

class FooBar2 : FooBar
{
	// ReSharper disable once RedundantParams
	public override void Foo(string fmt, params object[] args)
	{
		System.Console.WriteLine(fmt, args);
	}
}");
		}
	}
}

