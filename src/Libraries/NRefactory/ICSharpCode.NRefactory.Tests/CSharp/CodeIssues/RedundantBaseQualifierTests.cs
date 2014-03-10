// 
// RedundantBaseQualifierTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
	public class RedundantBaseQualifierTests : InspectionActionTestBase
	{

		[Test]
		public void TestInspectorCase1()
		{
			var input = @"using System;
	namespace Application
	{
		public class BaseClass
		{
			public int a;
			public virtual void method()
			{
				Console.Write(Environment.NewLine);
			}
			public void method1()
			{
				Console.Write(Environment.NewLine);
			}
		}

		class Program : BaseClass
		{
			public void method2(int a)
			{
				base.a = 1;
			}
			public override void method()
			{
				base.method1();
			}
		}
	}
";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantBaseQualifierIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			CheckFix(context, issues, @"using System;
	namespace Application
	{
		public class BaseClass
		{
			public int a;
			public virtual void method()
			{
				Console.Write(Environment.NewLine);
			}
			public void method1()
			{
				Console.Write(Environment.NewLine);
			}
		}

		class Program : BaseClass
		{
			public void method2(int a)
			{
				base.a = 1;
			}
			public override void method()
			{
				method1 ();
			}
		}
	}
");
		}

		[Test]
		public void TestInspectorCase2()
		{
			var input = @"using System;
	namespace Application
	{
		public class BaseClass
		{
			public int a;
			public int b;
			public void method()
			{
				Console.Write(Environment.NewLine);
			}
			public virtual void method1()
			{
				Console.Write(Environment.NewLine);
			}
		}

		class Program : BaseClass
		{
			public new void method(int b)
			{
				base.a = 1;
				base.b = 2;
			}
			public void method2()
			{
				base.method ();
			}
			public new int a;
		}
	}
";
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantBaseQualifierIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void ComplexTests ()
		{
			TestWrongContext<RedundantBaseQualifierIssue>(@"

class Base {
	public int a;
}

class Foo : Base
{
	void Bar ()
	{
		{
			int a = 343;
		}
		base.a = 2;
		
	}

	void FooBar ()
	{
		int a = base.a;
	}
}");
		}

		[Test]
		public void TestResharperDisableRestore()
		{
			var input = @"using System;
	namespace Application
	{
		public class BaseClass
		{
			public int a;
			public virtual void print()
			{
				Console.Write(Environment.NewLine);
			}
			public void print1()
			{
				Console.Write(Environment.NewLine);
			}
		}

		class Program : BaseClass
		{
			public void print2()
			{
//Resharper disable RedundantBaseQualifier
				base.a = 1;
//Resharper restore RedundantBaseQualifier
				base.a = 1;
			}
			public override void print()
			{
				base.print1();
			}
		}
	}";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantBaseQualifierIssue(), input, out context);
			Assert.AreEqual(2, issues.Count);
		}
		
		[Test]
		public void InvalidUseOfBaseInFieldInitializer()
		{
			var input = @"class Foo
{
	int a = base.a;
}";
			TestWrongContext<RedundantBaseQualifierIssue>(input);
		}
	}
}