//
// StaticEventSubscriptionIssueTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class StaticEventSubscriptionIssueTests: InspectionActionTestBase
	{
		[Test]
		public void TestAnonymousMethodSubscription()
		{
			TestIssue<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public static event EventHandler FooBar;
	
	public void Test ()
	{
		FooBar += delegate { 
			Console.WriteLine (""Hello World!"");
		};
	}
}
");
		}

		[Test]
		public void TestAnonymousMethodSubscription_ValidCase()
		{
			TestWrongContext<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public static event EventHandler FooBar;
	
	public static void Test ()
	{
		FooBar += delegate { 
			Console.WriteLine (""Hello World!"");
		};
	}
}
");
		}
	
	
		[Test]
		public void TestIssue()
		{
			TestIssue<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public static event EventHandler FooBar;
	
	public void Test ()
	{
		FooBar += MyMethod;
	}
	void MyMethod (object sender, EventArgs args)
	{
	}
}
");
		}

		[Test]
		public void TestNoIssue()
		{
			TestWrongContext<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public static event EventHandler FooBar;
	
	public void Test ()
	{
		FooBar += MyMethod;
	}
	
	public void Unsubscribe ()
	{
		FooBar -= MyMethod;
	}
	void MyMethod (object sender, EventArgs args)
	{
	}
}
");
		}

		[Test]
		public void TestNonStatic()
		{
			TestWrongContext<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public event EventHandler FooBar;

	public void Test ()
	{
		FooBar += MyMethod;
	}

	void MyMethod (object sender, EventArgs args)
	{
	}
}
");
		}

		[Test]
		public void TestNullAssignment()
		{
			TestWrongContext<StaticEventSubscriptionIssue>(@"
using System;

class Foo
{
	public static event EventHandler FooBar;
	
	public void Test ()
	{
		FooBar += MyMethod;
	}
	
	public void Unsubscribe ()
	{
		FooBar = null;
	}
	void MyMethod (object sender, EventArgs args)
	{
	}
}
");
		}
	}
}

