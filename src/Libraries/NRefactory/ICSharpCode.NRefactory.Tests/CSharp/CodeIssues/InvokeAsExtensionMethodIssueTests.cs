//
// InvokeAsExtensionMethodIssueTests.cs
//
// Author:
//   Simon Lindgren <simon.n.lindgren@gmail.com>
//   Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class StaticMethodInvocationToExtensionMethodInvocationTests : InspectionActionTestBase
	{

		[Test]
		public void HandlesBasicCase()
		{
			Test<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static bool Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		B.$Ext (a, 1);
	}
}", @"
class A { }
static class B
{
	public static bool Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		a.Ext (1);
	}
}");
		}

		[Test]
		public void HandlesReturnValueUsage()
		{
			Test<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static void Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		if (B.$Ext (a, 1))
			return;
	}
}", @"
class A { }
static class B
{
	public static void Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		if (a.Ext (1))
			return;
	}
}");
		}

		[Test]
		public void IgnoresIfNullArgument()
		{
			TestWrongContext<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static void Ext (this A a);
}
class C
{
	void F()
	{
		B.$Ext (null);
	}
}");
		}

		[Test]
		public void IgnoresIfNotExtensionMethod()
		{
			TestWrongContext<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static void Ext (A a);
}
class C
{
	void F()
	{
		B.$Ext (new A());
	}
}");
		}

		[Test]
		public void IgnoresIfAlreadyExtensionMethodCallSyntax()
		{
			TestWrongContext<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static void Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		a.$Ext (1);
	}
}");
		}

		[Test]
		public void IgnoresPropertyInvocation()
		{
			TestWrongContext<InvokeAsExtensionMethodIssue>(@"
static class B
{
	public static int Ext { get; set; }
}
class C
{
	void F()
	{
		B.$Ext();
	}
}");
		}


		[Test]
		public void TestDisable()
		{
			TestWrongContext<InvokeAsExtensionMethodIssue>(@"
class A { }
static class B
{
	public static bool Ext (this A a, int i);
}
class C
{
	void F()
	{
		A a = new A();
		// ReSharper disable once InvokeAsExtensionMethod
		B.Ext (a, 1);
	}
}");
		}
	}
}

