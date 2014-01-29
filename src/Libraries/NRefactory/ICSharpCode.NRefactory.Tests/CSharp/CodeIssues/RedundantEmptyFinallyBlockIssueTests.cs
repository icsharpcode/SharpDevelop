//
// RedundantEmptyFinallyBlockIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantEmptyFinallyBlockIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestRedundantTry()
		{
			Test<RedundantEmptyFinallyBlockIssue>(@"
using System;
class Test
{
	static void Main (string[] args)
	{
		try {
			Console.WriteLine (""1"");
			Console.WriteLine (""2"");
		} finally {
		}
	}
}
", @"
using System;
class Test
{
	static void Main (string[] args)
	{
		Console.WriteLine (""1"");
		Console.WriteLine (""2"");
	}
}
");
		}
	
		[Test]
		public void TestSimpleCase()
		{
			Test<RedundantEmptyFinallyBlockIssue>(@"
using System;
class Test
{
	static void Main (string[] args)
	{
		try {
			Console.WriteLine (""1"");
			Console.WriteLine (""2"");
		} catch (Exception) {
		} finally {
		}
	}
}
", @"
using System;
class Test
{
	static void Main (string[] args)
	{
		try {
			Console.WriteLine (""1"");
			Console.WriteLine (""2"");
		} catch (Exception) {
		}  
	}
}
");
		}
	
		[Test]
		public void TestInvalid()
		{
			TestWrongContext<RedundantEmptyFinallyBlockIssue>(@"
using System;
class Test
{
	static void Main(string[] args)
	{
		try {
			Console.WriteLine(""1"");
		} finally {
			Console.WriteLine(""2"");
		}
	}
}
");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<RedundantEmptyFinallyBlockIssue>(@"
using System;
class Test
{
	static void Main(string[] args)
	{
		try {
			Console.WriteLine(""1"");
			Console.WriteLine(""2"");
		}
		// ReSharper disable once RedundantEmptyFinallyBlock
 		finally {
		}
	}
}
");
		}
	}
}

