//
// PartOfBodyCanBeConvertedToQueryIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[Ignore]
	[TestFixture]
	public class PartOfBodyCanBeConvertedToQueryIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestWhere()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		foreach (var x in i) {
			if (x < 10) {
				Console.WriteLine(x);
			}
		}
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (int[] i)
	{
		foreach (var x in i.Where (x => x < 10)) {
			Console.WriteLine(x);
		}
	}
}
");
		}

		[Test]
		public void TestOfType()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo(object[] i)
	{
		foreach (var x in i) {
			if (x is int) {
				Console.WriteLine(x);
			}
		}
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo(object[] i)
	{
		foreach (var x in i.OfType<int> ()) {
			Console.WriteLine(x);
		}
	}
}
");
		}

		[Test]
		public void TestOfTypeTakeWhile()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo(object[] i)
	{
		foreach (var x in i) {
			if (x is int) {
				if ((int)x < 10)
					break;
				Console.WriteLine(x);
			}
		}
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo(object[] i)
	{
		foreach (var x in i.OfType<int> ().TakeWhile (x => x >= 10)) {
			Console.WriteLine(x);
		}
	}
}
");
		}

	}
}

