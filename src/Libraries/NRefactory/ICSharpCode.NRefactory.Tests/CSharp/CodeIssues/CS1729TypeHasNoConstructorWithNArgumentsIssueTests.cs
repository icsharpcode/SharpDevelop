//
// CS1729TypeHasNoConstructorWithNArgumentsIssueTests.cs
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

using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeIssues;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using System.Linq;


namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS1729TypeHasNoConstructorWithNArgumentsIssueTests : InspectionActionTestBase
	{

		[Test]
		public void ShouldReturnIssueIfBaseConstructorNotInvoked()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(string input) {}
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 1);
		}

		[Test]
		public void ShouldNotReturnIssueIfBaseClassHasDefaultConstructor()
		{
			var testInput =
				@"class BaseClass
{
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldNotReturnIssueIfBaseConstructorIsInvoked()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(string input) {}
}

class ChildClass : BaseClass
{
	public ChildClass() : base(""test"") {}
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldIgnoreInterfaces()
		{
			var testInput =
				@"class TestClass : System.Collections.IList
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldMakeSureAllConstructorsInvokeBaseConstructor()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(string test) {}
}

class ChildClass : BaseClass
{
	public ChildClass() : base(""test"") {}
	public ChildClass(string test) {}
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 1);
		}

		[Test]
		public void ShouldOnlyLookAtDirectBaseClasses()
		{
			var testInput =
				@"class TopLevelClass
{
	public TopLevelClass(string test) {}
}

class BaseClass : TopLevelClass
{
	public BaseClass() : base(""hello"") {}
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldReturnAnIssueIfBaseConstructorIsPrivate()
		{
			var testInput =
				@"class BaseClass
{
	private BaseClass() {}
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 1);
		}

		[Test]
		public void ShouldReturnAnIssueIfBaseConstructorIsPrivate_ExplicitInitializer()
		{
			var testInput =
				@"class BaseClass
{
	private BaseClass() {}
}

class ChildClass : BaseClass
{
	public ChildClass() : base() {}
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 1);
		}

		[Test]
		public void ShouldReturnIssuesForNestedTypes()
		{
			TestRefactoringContext context;

			var testInput =
				@"class B {
	public B(string test) {}
}

class D {
	public D(string test) {}
}

class A : B {
	class C : D {}
	public A() {}
}";

			var issues = GetIssues(new CS1729TypeHasNoConstructorWithNArgumentsIssue(), testInput, out context, false);

			Assert.AreEqual("CS1729: The type 'B' does not contain a constructor that takes '0' arguments", issues.ElementAt(1).Description);
			Assert.AreEqual("CS1729: The type 'D' does not contain a constructor that takes '0' arguments", issues.ElementAt(0).Description);
		}

		[Test]
		public void ShouldNotReturnIssueIfBaseClassCtorHasOptionalParameters()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(int i = 0)
	{
	}
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldNotReturnIssueIfBaseClassCtorHasVariadicParameters()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(params string[] str)
	{
	}
}

class ChildClass : BaseClass
{
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShouldNotReturnIssueForStaticConstructor()
		{
			var testInput =
				@"class BaseClass
{
	public BaseClass(string text)
	{
	}
}

class ChildClass : BaseClass
{
	public ChildClass() : base(""text"") {}
	
	static ChildClass() {}
}";

			Test<CS1729TypeHasNoConstructorWithNArgumentsIssue>(testInput, 0);
		}

		[Test]
		public void ShowIssueForObjectCreateExpression()
		{
			var testInput =
				@"class Test {
	public void M() {
		new Test(1);
	}
}
";

			TestRefactoringContext context;
			var issues = GetIssues(new CS1729TypeHasNoConstructorWithNArgumentsIssue(), testInput, out context);

			Assert.AreEqual("CS1729: The type 'Test' does not contain a constructor that takes '1' arguments", issues.Single().Description);
		}
	}
}

