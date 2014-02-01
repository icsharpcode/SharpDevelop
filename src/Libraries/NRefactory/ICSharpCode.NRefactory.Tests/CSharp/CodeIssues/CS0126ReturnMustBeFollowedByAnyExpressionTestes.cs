//
// CS0126ReturnMustBeFollowedByAnyExpressionTests.cs
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
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0126ReturnMustBeFollowedByAnyExpressionTests : InspectionActionTestBase
	{
		static void Test(string type, string defaultValue)
		{
			Test<CS0126ReturnMustBeFollowedByAnyExpression>(@"class Foo
{
	" + type + @" Bar (string str)
	{
		return;
	}
}", @"class Foo
{
	" + type + @" Bar (string str)
	{
		return " + defaultValue + @";
	}
}");
		}

		[Test]
		public void TestSimpleCase ()
		{
			Test("int", "0");
			Test("string", "\"\"");
			Test("char", "' '");
			Test("bool", "false");
			Test("object", "null");
			Test("System.DateTime", "default(System.DateTime)");
		}

		[Test]
		public void TestReturnTypeFix ()
		{
			var input = @"class Foo
{
	int Bar (string str)
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	void Bar (string str)
	{
		return;
	}
}", 1);
		}


		[Test]
		public void TestProperty ()
		{
			var input = @"class Foo {
	string Bar 
	{
		get {
			return;
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		
		[Test]
		public void TestPropertySetter ()
		{
			var input = @"class Foo {
	string Bar 
	{
		set {
			return;
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}


		[Test]
		public void TestIndexer ()
		{
			var input = @"class Foo {
	string this [int idx]
	{
		get {
			return;
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestAnonymousMethod ()
		{
			var input = @"
using System;
class Foo
{
	void Bar (string str)
	{
		System.Func<string> func = delegate {
			return;
		};
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}


		[Test]
		public void TestAnonymousMethodReturnTypeFix ()
		{
			Test<CS0126ReturnMustBeFollowedByAnyExpression>(@"
using System;
class Foo
{
	int Bar (string str)
	{
		System.Func<string> func = delegate {
			return;
		};
	}
}", @"
using System;
class Foo
{
	int Bar (string str)
	{
		System.Func<string> func = delegate {
			return """";
		};
	}
}");
		}

		[Test]
		public void TestAnonymousMethodReturningVoid ()
		{
			var input = @"using System;

class Foo
{
	void Bar (string str)
	{
		Action func = delegate {
			return;
		};
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}


		[Test]
		public void TestLambdaMethod ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		System.Func<string> func = () => {
			return;
		};
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestOperatorFalsePositives ()
		{
			var input = @"class Foo
{
	public static bool operator == (Foo left, Foo right)
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}

		[Test]
		public void TestConstructor ()
		{
			var input = @"class Foo
{
	Foo ()
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestDestructor ()
		{
			var input = @"class Foo
{
	~Foo ()
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestDontShowUpOnUndecidableCase ()
		{
			TestWrongContext<CS0126ReturnMustBeFollowedByAnyExpression>(@"
using System;

class Test
{
	void Foo (Func<int, int> func) {}
	void Foo (Action<int> func) {}

	void Bar (string str)
	{
		Foo(delegate {
			return;
		});
	}
}");
		}

		[Test]
		public void TestParallelForBug ()
		{
			TestWrongContext<CS0126ReturnMustBeFollowedByAnyExpression>(@"
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

class Test
{
	void FooBar(IEnumerable<string> str)
	{
		Parallel.ForEach(str, p => {
			return;
		});
	}
}");
		}


		[Test]
		public void TestConstructorInitializer ()
		{
			TestWrongContext<CS0126ReturnMustBeFollowedByAnyExpression>(@"
using System;

class Test
{
	Test (Func<int, int> func) {}
	Test (Action<int> func) {}
	
	Test () : this (delegate { return; }) 
	{
	}
}");
		}
		
		[Test]
		public void TestAsyncMethod_Void()
		{
			var input = @"using System;
using System.Threading.Tasks;

class Test
{
	public async void M()
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestAsyncMethod_Task()
		{
			var input = @"using System;
using System.Threading.Tasks;

class Test
{
	public async Task M()
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestAsyncMethod_TaskOfInt()
		{
			var input = @"using System;
using System.Threading.Tasks;

class Test
{
	public async Task<int> M()
	{
		return;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new CS0126ReturnMustBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}
	}
}

