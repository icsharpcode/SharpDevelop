//
// EventUnsubscriptionViaAnonymousDelegateIssueTests.cs
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
	public class EventUnsubscriptionViaAnonymousDelegateIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestDelegateCase ()
		{
			const string usingSystemClassFooPublicEventEventHandlerFooVoidTestFooDelegate = @"using System;

class Bar
{
	public event EventHandler Foo;

	void Test ()
	{
		Foo -= delegate { };
	}
}";
			string input = usingSystemClassFooPublicEventEventHandlerFooVoidTestFooDelegate;
			TestRefactoringContext context;
			var issues = GetIssues (new EventUnsubscriptionViaAnonymousDelegateIssue(), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestLambdaCase ()
		{
			string input = @"using System;

class Bar
{
	public event EventHandler Foo;

	void Test ()
	{
		Foo -= (s ,e) => { };
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new EventUnsubscriptionViaAnonymousDelegateIssue(), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestDisable ()
		{
			string input = @"using System;

class Bar
{
	public event EventHandler Foo;

	void Test ()
	{
		// ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
		Foo -= delegate { };
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new EventUnsubscriptionViaAnonymousDelegateIssue(), input, out context);
			Assert.AreEqual (0, issues.Count);
		}


	}
}

