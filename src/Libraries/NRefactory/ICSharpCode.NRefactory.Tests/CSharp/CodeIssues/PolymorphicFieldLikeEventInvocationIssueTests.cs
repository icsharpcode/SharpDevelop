//
// PolymorphicFieldLikeEventInvocationIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using System;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{

	[TestFixture]
	public class PolymorphicFieldLikeEventInvocationIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleCase()
		{
			TestIssue<PolymorphicFieldLikeEventInvocationIssue>(@"
using System;

public class Bar
{
	public virtual event EventHandler FooBarEvent;
}

public class Foo : Bar
{
	public override event EventHandler FooBarEvent;

	public void FooBar()
	{
		FooBarEvent(this, EventArgs.Empty);
	}
}
");
		}

		[Test]
		public void TestCustomEvent()
		{
			// Should be marked as error
			TestIssue<PolymorphicFieldLikeEventInvocationIssue>(@"
using System;

public class Bar
{
	public virtual event EventHandler FooBarEvent;
}

public class Foo : Bar
{
	public override event EventHandler FooBarEvent {
		add {}
		remove {}
	}

	public void FooBar()
	{
		FooBarEvent(this, EventArgs.Empty);
	}
}
");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<PolymorphicFieldLikeEventInvocationIssue>(@"
using System;

public class Bar
{
	public virtual event EventHandler FooBarEvent;
}

public class Foo : Bar
{
	public override event EventHandler FooBarEvent;

	public void FooBar()
	{
		// ReSharper disable once PolymorphicFieldLikeEventInvocation
		FooBarEvent(this, EventArgs.Empty);
	}
}
");
		}

	}
}

