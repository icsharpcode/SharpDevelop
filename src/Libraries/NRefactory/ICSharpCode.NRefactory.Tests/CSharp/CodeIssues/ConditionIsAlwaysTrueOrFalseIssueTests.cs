//
// ConditionIsAlwaysTrueOrFalseIssueTests.cs
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
	public class ConditionIsAlwaysTrueOrFalseIssueTests: InspectionActionTestBase
	{
		[Test]
		public void TestComparsionWithNull ()
		{
			Test<ConditionIsAlwaysTrueOrFalseIssue>(@"
class Test
{
	void Foo(int i)
	{
		if (i == null) {
		}
	}
}
", @"
class Test
{
	void Foo(int i)
	{
		if (false) {
		}
	}
}
");
		}


		[Test]
		public void TestComparsionWithNullCase2 ()
		{
			Test<ConditionIsAlwaysTrueOrFalseIssue>(@"
enum Bar { A, B }
class Test
{
	void Foo(Bar i)
	{
		if (i != null) {
		}
	}
}
", @"
enum Bar { A, B }
class Test
{
	void Foo(Bar i)
	{
		if (true) {
		}
	}
}
");
		}


		[Test]
		public void TestComparison ()
		{
			Test<ConditionIsAlwaysTrueOrFalseIssue>(@"
class Test
{
	void Foo(int i)
	{
		if (1 > 2) {
		}
	}
}
", @"
class Test
{
	void Foo(int i)
	{
		if (false) {
		}
	}
}
");
		}

		[Test]
		public void TestUnary ()
		{
			Test<ConditionIsAlwaysTrueOrFalseIssue>(@"
class Test
{
	void Foo(int i)
	{
		if (!true) {
		}
	}
}
", @"
class Test
{
	void Foo(int i)
	{
		if (false) {
		}
	}
}
");
		}


		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ConditionIsAlwaysTrueOrFalseIssue>(@"
class Test
{
	void Foo(int i)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalse
		if (i == null) {
		}
	}
}
");
		}


		[Test]
		public void CompareWithNullable ()
		{
			TestWrongContext<ConditionIsAlwaysTrueOrFalseIssue>(@"
class Bar
{
	public void Test(int? a)
	{
		if (a != null) {

		}
	}
}
");
		}

		[Test]
		public void UserDefinedOperatorsNoReferences()
		{
			TestIssue<ConditionIsAlwaysTrueOrFalseIssue>(@"
struct Foo 
{
	public static bool operator ==(Foo value, Foo o)
	{
		return false;
	}

	public static bool operator !=(Foo value, Foo o)
	{
		return false;
	}
}

class Bar
{
	public void Test(Foo a)
	{
		if (a != null) {

		}
	}
}
");
		}

		[Test]
		public void UserDefinedOperators()
		{
			TestWrongContext<ConditionIsAlwaysTrueOrFalseIssue>(@"
struct Foo 
{
	public static bool operator ==(Foo value, object o)
	{
		return false;
	}

	public static bool operator !=(Foo value, object o)
	{
		return false;
	}
}

class Bar
{
	public void Test(Foo a)
	{
		if (a != null) {

		}
	}
}
");
		}


		/// <summary>
		/// Bug 15099 - Wrong always true context
		/// </summary>
		[Test]
		public void TestBug15099()
		{
			TestWrongContext<ConditionIsAlwaysTrueOrFalseIssue>(@"
struct Foo 
{
	string name;

	public Foo (string name)
	{
		this.name = name;
	}

	public static bool operator ==(Foo value, Foo o)
	{
		return value.name == o.name;
	}

	public static bool operator !=(Foo value, Foo o)
	{
		return !(value == o);
	}

	public static implicit operator Foo (string name)
	{
		return new Foo (name);
	}
}

class Bar
{
	public static void Main (string[] args)
	{
		var foo = new Foo (null);
		System.Console.WriteLine (foo == null);
	}
}");
		}



	}
}

