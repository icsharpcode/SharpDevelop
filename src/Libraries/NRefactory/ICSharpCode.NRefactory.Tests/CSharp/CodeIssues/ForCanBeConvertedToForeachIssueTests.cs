//
// ForCanBeConvertedToForeachIssueTests.cs
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
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ForCanBeConvertedToForeachIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestArrayCase ()
		{
			Test<ForCanBeConvertedToForeachIssue>(@"
class Test
{
	void Foo (object[] o)
	{
		for (int i = 0; i < o.Length; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		foreach (var p in o) {
			System.Console.WriteLine (p);
		}
	}
}");
		}

		[Test]
		public void TestIListCase ()
		{
			Test<ForCanBeConvertedToForeachIssue>(@"
using System.Collections.Generic;

class Test
{
	void Foo(IList<int> o)
	{
		for (int i = 0; i < o.Count; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
using System.Collections.Generic;

class Test
{
	void Foo(IList<int> o)
	{
		foreach (var p in o) {
			System.Console.WriteLine (p);
		}
	}
}");
		}

		[Test]
		public void TestInvalid ()
		{
			TestWrongContext<ForCanBeConvertedToForeachIssue>(@"
class Test
{
	void Foo (object[] o)
	{
		for (int i = 0; i < o.Length; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
			System.Console.WriteLine (i++);
		}
	}
}");
		}

		[Test]
		public void TestInvalidCase2 ()
		{
			TestWrongContext<ForCanBeConvertedToForeachIssue>(@"
class Test
{
	void Foo (object[] o)
	{
		for (int i = 0; i < o.Length; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
			System.Console.WriteLine (i);
		}
	}
}");
		}

		[Test]
		public void TestInvalidCase3 ()
		{
			TestWrongContext<ForCanBeConvertedToForeachIssue>(@"
class Test
{
	void Foo (object[] o)
	{
		for (int i = 0; i < o.Length; i++) {
			var p = o [i];
			p = o[0];
			System.Console.WriteLine (p);
		}
	}
}");
		}


		[Test]
		public void TestComplexExpression ()
		{
			Test<ForCanBeConvertedToForeachIssue>(@"
using System.Collections.Generic;

class Test
{
	IList<int> Bar { get { return null; } }

	void Foo(object o)
	{
		for (int i = 0; i < ((Test)o).Bar.Count; i++) {
			var p = ((Test)o).Bar [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
using System.Collections.Generic;

class Test
{
	IList<int> Bar { get { return null; } }

	void Foo(object o)
	{
		foreach (var p in ((Test)o).Bar) {
			System.Console.WriteLine (p);
		}
	}
}");
		}


		[Test]
		public void TestOptimizedFor ()
		{
			Test<ForCanBeConvertedToForeachIssue>(@"
using System.Collections.Generic;

class Test
{
	IList<int> Bar { get { return null; } }

	void Foo(object o)
	{
		for (var i = 0, maxBarCount = ((Test)o).Bar.Count; i < maxBarCount; ++i) {
			var p = ((Test)o).Bar[i];
			System.Console.WriteLine(p);
		}
	}
}", @"
using System.Collections.Generic;

class Test
{
	IList<int> Bar { get { return null; } }

	void Foo(object o)
	{
		foreach (var p in ((Test)o).Bar) {
			System.Console.WriteLine (p);
		}
	}
}");
		}



		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ForCanBeConvertedToForeachIssue>(@"
class Test
{
	void Foo (object[] o)
	{
		// ReSharper disable once ForCanBeConvertedToForeach
		for (int i = 0; i < o.Length; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}");
		}
	}
}

