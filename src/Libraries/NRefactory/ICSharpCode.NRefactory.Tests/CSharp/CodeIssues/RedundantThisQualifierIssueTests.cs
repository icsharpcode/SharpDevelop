// 
// RedundantThisQualifierIssueTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantThisQualifierIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestInspectorCase1 ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		this.Bar (str);
	}
}";

			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new RedundantThisQualifierIssue (), input, RedundantThisQualifierIssue.EverywhereElse, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	void Bar (string str)
	{
		Bar (str);
	}
}");
		}
		
		[Test]
		public void TestSkipConstructors ()
		{
			var input = @"class Foo
{
	public Foo ()
	{
		this.Bar (str);
	}
}";
			TestWrongContextWithSubIssue<RedundantThisQualifierIssue>(input, RedundantThisQualifierIssue.EverywhereElse);
		}


		[Test]
		public void TestInsideConstructors ()
		{
			var input = @"class Foo
{
	public Foo ()
	{
		this.Bar (str);
	}
	void Bar (string str)
	{
	}
}";

			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new RedundantThisQualifierIssue (), input, RedundantThisQualifierIssue.InsideConstructors, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	public Foo ()
	{
		Bar (str);
	}
	void Bar (string str)
	{
	}
}");
		}

		[Test]
		public void TestInsideConstructorsSkipMembers ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		this.Bar (str);
	}
}";
			TestWrongContextWithSubIssue<RedundantThisQualifierIssue>(input, RedundantThisQualifierIssue.InsideConstructors);
		}
		
		[Test]
		public void TestRequiredThisInAssignmentFromFieldToLocal ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		var a = this.a;
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisInAssignmentFromDelegateToLocal ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		Action a = () => {
			Console.WriteLine (this.a);
		};
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRedundantThisInAssignmentFromFieldToLocal ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		var b = this.a;
	}
}", @"class Foo
{
	int a;
	void Bar ()
	{
		var b = a;
	}
}", RedundantThisQualifierIssue.EverywhereElse);
		}
		
		[Test]
		public void TestRedundantThisInAssignmentFromDelegateToLocal ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		Action b = () => {
			Console.WriteLine (this.a);
		};
	}
}", @"class Foo
{
	int a;
	void Bar ()
	{
		Action b = () => {
			Console.WriteLine (a);
		};
	}
}", RedundantThisQualifierIssue.EverywhereElse);
		}
		
		[Test]
		public void TestRequiredThisInExtensionMethodCall ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"static class Extensions
{
	public static void Ext (this Foo foo)
	{
	}
}

class Foo
{
	void Bar ()
	{
		this.Ext ();
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}

		[Test]
		public void TestRequiredThisToAvoidCS0135 ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		{
			int a = 0;
		}
		this.a = 2;
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}

		[Test]
		public void TestRequiredThisToAvoidCS0844 ()
		{
			Test<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		{
			this.a = 0;
		}
		var a = 2;
	}
}", 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithLambda ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		Action<int> action = (int a) => a.ToString();
		this.a = 2;
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithDelegate ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		Action<int> action = delegate (int a) { a.ToString(); };
		this.a = 2;
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithForeach ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		this.a = 2;
		foreach (var a in ""abc"")
			System.Console.WriteLine (a);
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithFor ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		this.a = 2;
		for (int a = 0; a < 2; a++)
			System.Console.WriteLine (a);
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithUsing ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Foo
{
	int a;
	void Bar ()
	{
		this.a = 2;
		using (var a = new System.IO.MemoryStream())
			a.Flush();
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestRequiredThisToAvoidCS0135WithFixed ()
		{
			TestWithSubIssue<RedundantThisQualifierIssue>(@"class Baz
{
	public int c;
}
class Foo
{
	int a;
	unsafe void Bar ()
	{
		this.a = 2;
		var b = new Baz();
		fixed (int* a = &b.c)
			Console.WriteLine(a == null);
	}
}", RedundantThisQualifierIssue.EverywhereElse, 0);
		}
		
		[Test]
		public void TestResharperDisableRestore ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		// ReSharper disable RedundantThisQualifier
		this.Bar (str);
		// ReSharper restore RedundantThisQualifier
		this.Bar (str);
	}
}";

			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new RedundantThisQualifierIssue (), input, RedundantThisQualifierIssue.EverywhereElse, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestBatchFix ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		this.Bar (str);
		this.Bar (str);
	}
}";

			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new RedundantThisQualifierIssue (), input, RedundantThisQualifierIssue.EverywhereElse, out context);
			Assert.AreEqual (2, issues.Count);
			CheckBatchFix (context, issues, issues[0].Actions.First().SiblingKey, @"class Foo
{
	void Bar (string str)
	{
		Bar (str);
		Bar (str);
	}
}");
		}
		
		[Test]
		public void InvalidUseOfThisInFieldInitializer()
		{
			var input = @"class Foo
{
	int a = this.a;
}";
			TestWrongContextWithSubIssue<RedundantThisQualifierIssue>(input, RedundantThisQualifierIssue.EverywhereElse);
		}
	}
}