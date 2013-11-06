//
// ReplaceWithOfTypeAnyIssueTests.cs
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
	public class ReplaceWithOfTypeAnyIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestCaseBasic ()
		{
			Test<ReplaceWithOfTypeAnyIssue>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (q => q as Test).Any (q => q != null);
	}
}", @"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.OfType<Test> ().Any ();
	}
}");
		}

		[Test]
		public void TestCaseBasicWithFollowUpExpresison ()
		{
			Test<ReplaceWithOfTypeAnyIssue>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (q => q as Test).Any (q => q != null && Foo (q));
	}
}", @"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.OfType<Test> ().Any (q => Foo (q));
	}
}");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ReplaceWithOfTypeAnyIssue>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		// ReSharper disable once ReplaceWithOfType.Any
		obj.Select (q => q as Test).Any (q => q != null);
	}
}");
		}

		[Test]
		public void TestJunk ()
		{
			TestWrongContext<ReplaceWithOfTypeAnyIssue>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (x => q as Test).Any (q => q != null);
	}
}");
			TestWrongContext<ReplaceWithOfTypeAnyIssue>(@"using System.Linq;
class Test
{
	public void Foo(object[] obj)
	{
		obj.Select (q => q as Test).Any (q => 1 != null);
	}
}");

		}

	}
}

