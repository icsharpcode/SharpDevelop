//
// DoNotCallOverridableMethodsInConstructorIssueTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
	public class DoNotCallOverridableMethodsInConstructorIssueTests : InspectionActionTestBase
	{ 

		[Test]
		public void CatchesBadCase()
		{
			var input = @"class Foo
{
	Foo()
	{
		Bar();
		Bar();
	}

	virtual void Bar ()
	{
	}
}";
			TestRefactoringContext context;
            var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(2, issues.Count);
		}

		[Test]
		public void TestDisable()
		{
            var input = @"class Foo
{
	Foo()
	{
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
		Bar();
	}

	virtual void Bar ()
	{
	}
}";
            TestWrongContext<DoNotCallOverridableMethodsInConstructorIssue>(input);
		}



		[Test]
		public void IgnoresGoodCase()
		{
			var input = @"class Foo
{
	Foo()
	{
		Bar();
		Bar();
	}

	void Bar ()
	{
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void IgnoresSealedClasses()
		{
			var input = @"sealed class Foo
{
	Foo()
	{
		Bar();
		Bar();
	}

	virtual void Bar ()
	{
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void IgnoresNonLocalCalls()
		{
			var input = @"class Foo
{
	Foo()
	{
		Foo f = new Foo();
		f.Bar();
	}

	virtual void Bar ()
	{
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void IgnoresEventHandlers()
		{
			var input = @"class Foo
{
	Foo()
	{
		SomeEvent += delegate { Bar(); };
	}

	virtual void Bar ()
	{
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}


		/// <summary>
		/// Bug 14450 - False positive of "Virtual member call in constructor"
		/// </summary>
		[Test]
		public void TestBug14450()
		{
			var input = @"
using System;

public class Test {
    public Test(Action action) {
        action();
    }
}
";		
			TestWrongContext<DoNotCallOverridableMethodsInConstructorIssue>(input);
		}
		
		[Test]
		public void SetVirtualProperty()
		{
			var input = @"class Foo
{
	Foo()
	{
		this.AutoProperty = 1;
	}

	public virtual int AutoProperty { get; set; }
}";
			TestRefactoringContext context;
            var issues = GetIssues(new DoNotCallOverridableMethodsInConstructorIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}
	}
}
