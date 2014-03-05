//
// DelegateSubtractionIssueTests.cs
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
	[TestFixture]
	public class DelegateSubtractionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSubtraction()
		{
			TestIssue<DelegateSubtractionIssue>(@"
using System;
class Foo
{
	void Bar (Action a, Action b)
	{
		(a - b) ();
	}
}
");
		}

		[Test]
		public void TestAssignmentSubtraction()
		{
			TestIssue<DelegateSubtractionIssue>(@"
using System;
class Foo
{
	void Bar (Action a, Action b)
	{
		a -= b;
	}
}
");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<DelegateSubtractionIssue>(@"
using System;
class Foo
{
	void Bar (Action a, Action b)
	{
		// ReSharper disable once DelegateSubtraction
		(a - b) ();
	}
}
");
		}

		/// <summary>
		/// Bug 18061 - Incorrect "delegate subtraction has unpredictable result" warning
		/// </summary>
		[Test]
		public void TestBug18061()
		{
			TestWrongContext<DelegateSubtractionIssue>(@"
using System;
class Test
{
	public event EventHandler Foo;

	void Bar (EventHandler bar)
	{
		Foo -= bar;
	}
}
");
		}

	}
}

