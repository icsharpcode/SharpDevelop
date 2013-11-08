//
// CanBeReplacedWithTryCastAndCheckForNullIssueTests.cs
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
	public class CanBeReplacedWithTryCastAndCheckForNullIssueTests : InspectionActionTestBase
	{
		[Test]
		public void SimpleCase()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		if (foo is Bar) {
			Baz ((Bar)foo);
			return (Bar)foo;
		}
		return null;
	}
}
");
		}

		[Test]
		public void ComplexCase()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public IDisposable Baz (object foo)
	{
		if (((foo) is Bar)) {
			Baz ((Bar)foo);
			Baz (foo as Bar);
			Baz (((foo) as Bar));
			Baz ((Bar)(foo));
			return (IDisposable)foo;
		}
		return null;
	}
}
");
		}

		[Test]
		public void IfElseCase()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		if (foo is Bar) {
			Baz ((Bar)foo);
			return (Bar)foo;
		} else {
			Console.WriteLine (""Hello World "");
		}
		return null;
	}
}
");
		}

		[Test]
		public void InvalidIfNoTypeCast()
		{
			TestWrongContext<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		if (foo is Bar) {
			Console.WriteLine (""Hello World "");
		}
		return null;
	}
}
");
		}

		[Test]
		public void NestedIf()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		if (foo is string) {
		} else if (foo is Bar) {
			Baz ((Bar)foo);
			return (Bar)foo;
		}
		return null;
	}
}
");
		}

		[Test]
		public void TestNegatedCaseWithReturn()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		if (!(foo is Bar))
			return null;
		Baz ((Bar)foo);
		return (Bar)foo;
	}
}
");
		}

		[Test]
		public void TestNegatedCaseWithBreak()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		for (int i = 0; i < 10; i++) {
			if (!(foo is Bar))
				break;
			Baz ((Bar)foo);
		}
		return (Bar)foo;
	}
}
");
		}

		[Test]
		public void TestCaseWithContinue()
		{
			TestIssue<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		for (int i = 0; i < 10; i++) {
			if (!(foo is Bar)) {
				continue;
			} else {
				foo = new Bar ();
			}
			Baz ((Bar)foo);
		}
		return (Bar)foo;
	}
}
");
		}


		[Test]
		public void TestDisable()
		{
			TestWrongContext<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public Bar Baz (object foo)
	{
		// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
		if (foo is Bar) {
			Baz ((Bar)foo);
			return (Bar)foo;
		}
		return null;
	}
}
");
		}

		[Test]
		public void TestInvaludValueType()
		{
			TestWrongContext<CanBeReplacedWithTryCastAndCheckForNullIssue>(@"
class Bar
{
	public int Baz (object foo)
	{
		if (foo is int) {
			Baz ((int)foo);
			return (int)foo;
		}
		return 0;
	}
}
");
		}
	}
}

