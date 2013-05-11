using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeIssues;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
    [TestFixture]
	public class NoDefaultConstructorIssueTests : InspectionActionTestBase
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

			Test<NoDefaultConstructorIssue>(testInput, 1);
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

			Test<NoDefaultConstructorIssue>(testInput, 0);
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

			Test<NoDefaultConstructorIssue>(testInput, 0);
		}

		[Test]
		public void ShouldReturnIssueIfInvalidArgumentsArePassedToBaseConstructor()
		{
			var testInput =
@"class BaseClass
{
	public BaseClass(string input) {}
}

class ChildClass : BaseClass
{
	public ChildClass() : base(123) {}
}";

			Test<NoDefaultConstructorIssue>(testInput, 1);
		}

		[Test]
		public void ShouldIgnoreInterfaces()
		{
			var testInput =
@"class TestClass : System.Collections.IList
{
}";

			Test<NoDefaultConstructorIssue>(testInput, 0);
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

			Test<NoDefaultConstructorIssue>(testInput, 1);
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

			Test<NoDefaultConstructorIssue>(testInput, 0);
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

			Test<NoDefaultConstructorIssue>(testInput, 1);
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

			var issues = GetIssues(new NoDefaultConstructorIssue(), testInput, out context, false);

			Assert.AreEqual("CS1729: The type 'B' does not contain a constructor that takes '0' arguments", issues.ElementAt(1).Description);
			Assert.AreEqual("CS1729: The type 'D' does not contain a constructor that takes '0' arguments", issues.ElementAt(0).Description);
		}
	}
}

