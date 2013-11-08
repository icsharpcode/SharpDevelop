//
// RedundantStringToCharArrayCallIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantStringToCharArrayCallIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleForeachCase ()
		{
			Test<RedundantStringToCharArrayCallIssue>(@"
using System;
class FooBar
{
	public void Test (string str)
	{
		foreach (char c in str.ToCharArray ()) {
			Console.WriteLine (c);
		}
	}
}
", @"
using System;
class FooBar
{
	public void Test (string str)
	{
		foreach (char c in str) {
			Console.WriteLine (c);
		}
	}
}
");
		}

		[Test]
		public void TestVarForeachCase ()
		{
			Test<RedundantStringToCharArrayCallIssue>(@"
using System;
class FooBar
{
	public void Test (string str)
	{
		foreach (var c in str.ToCharArray ()) {
			Console.WriteLine (c);
		}
	}
}
", @"
using System;
class FooBar
{
	public void Test (string str)
	{
		foreach (var c in str) {
			Console.WriteLine (c);
		}
	}
}
");
		}

		[Test]
		public void TestIndexerCase ()
		{
			Test<RedundantStringToCharArrayCallIssue>(@"
using System;
class FooBar
{
	public void Test (string str)
	{
		Console.WriteLine ((str.ToCharArray ())[5]);
	}
}
", @"
using System;
class FooBar
{
	public void Test (string str)
	{
		Console.WriteLine (str [5]);
	}
}
");
		}


		[Test]
		public void TestDisable ()
		{
			TestWrongContext<RedundantStringToCharArrayCallIssue>(@"
using System;
class FooBar
{
	public void Test (string str)
	{
		// ReSharper disable once RedundantStringToCharArrayCall
		foreach (char c in str.ToCharArray ()) {
			Console.WriteLine (c);
		}
	}
}
");
		}


	}
}

