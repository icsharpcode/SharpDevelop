//
// LoopCanBeConvertedToQueryIssueTests.cs
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
	public class LoopCanBeConvertedToQueryIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestCount()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int count = 0;
		foreach (var x in i)
			count++;
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = i.Count ();
	}
}
");
		}

		[Test]
		public void TestCountCase2()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int count = 1;
		foreach (var x in i)
			count = count + 1;
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = 1 + i.Count ();
	}
}
");
		}

		[Test]
		public void TestCountPredicate()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int count = 0;
		foreach (var x in i)
			if (x < 10)
				count++;
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = 1 + i.Count (x => x < 10);
	}
}
");
		}

		[Test]
		public void TestForeachSum()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = 0;
		foreach (var j in i) {
			sum += j;
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
		int sum = i.Sum ();
	}
}
");
		}
	
		[Test]
		public void TestForSum()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = 0;
		for (int j = 0; j < i.Length; j++)
			sum += i [j];
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (int[] i)
	{
		int sum = i.Sum ();
	}
}
");
		}

		[Test]
		public void TestForBinaryExpression()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int mult = 1;
		for (int j = 0; j < i.Length; j++) {
			mult = mult * i[j];
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
		int mult = i.Aggregate (1, (current, t) => current*t);
	}
}
");
		}

		[Test]
		public void TestMaxCase1()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = Math.Max (max, x);
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
		}

		[Test]
		public void TestMaxCase2()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = Math.Max (x, max);
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
		}

		[Test]
		public void TestMaxCase3()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = x > max ? x : max;
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = x >= max ? x : max;
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
		}

		[Test]
		public void TestMaxCase4()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = x < max ? max : x;
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = x <= max ? max : x;
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
		int max = i.Concat (new[] {0}).Max ();
	}
}
");
		}

		[Test]
		public void TestMinCase1()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (int[] i)
	{
		int max = 0;
		foreach (var x in i) {
			max = Math.Min (max, x);
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
		int max = i.Concat (new[] {0}).Min ();
	}
}
");
		}

		[Test]
		public void TestCastMatch()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		List<Bar> list = new List<Bar>();
		foreach (var x in i) {
			list.Add((Bar)x);
		}
	}
}
", @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		List<Bar> list = i.Cast<Bar> ().ToList ();
	}
}
");
		}

		[Test]
		public void TestOfTypeMatch()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		List<Bar> list = new List<Bar>();
		foreach (var x in i) {
			if (x is Bar)
				list.Add((Bar)x);
		}
	}
}
", @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		List<Bar> list = i.OfType<Bar> ().ToList ();
	}
}
");
		}

		[Test]
		public void TestFirstOrDefault()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (object[] i)
	{
		object first = null;
		foreach (var x in i){
			first = x;
			break;
		}
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		object first = i.FirstOrDefault ();
	}
}
");
		}

		[Test]
		public void TestLastOrDefault()
		{
			Test<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo (object[] i)
	{
		object last = null;
		foreach (var x in i){
			last = x;
		}
	}
}
", @"
using System;
using System.Linq;

public class Bar
{
	public void Foo (object[] i)
	{
		object last = i.LastOrDefault ();
	}
}
");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<LoopCanBeConvertedToQueryIssue>(@"
using System;

public class Bar
{
	public void Foo(int[] i)
	{
		int sum = 0;
		// ReSharper disable once LoopCanBeConvertedToQuery
		foreach (var j in i) {
			sum += j;
		}
	}
}
");
		}
	}
}

