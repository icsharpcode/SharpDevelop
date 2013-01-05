using ICSharpCode.NRefactory.CSharp.CodeIssues;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeActions.AddUsing
{
	[TestFixture]
	public class AddUsingActionTests : ContextActionTestBase
	{
		void UnresolvedTypeName(string code, string typeName, params string[] namespaces)
		{
			TestActionDescriptions(
				new AddUsingAction(), code,
				namespaces.SelectMany(ns => new[] {
				                      	"using " + ns + ";",
				                      	ns + "." + typeName
				                      }).ToArray());
		}
		
		#region Field Declarations
		[Test]
		public void ShouldReturnAnIssueForUnresolvedFieldDeclarations()
		{
			UnresolvedTypeName(@"class Foo {
	private $TextWriter textWriter;
}", "TextWriter", "System.IO");
		}

		[Test]
		public void ShouldNotReturnAnyIssuesIfFieldTypeIsResolved()
		{
			TestWrongContext<AddUsingAction>(@"using System.IO;
class Foo {
	private $TextWriter textWriter;
}");
		}

		[Test]
		public void ShouldReturnAnIssueIfFieldTypeArgumentIsNotResolvable()
		{
			UnresolvedTypeName(
				@"using System.Collections.Generic;

class Foo
{
	private List<$AttributeTargets> targets;
}", "AttributeTargets", "System");
		}

		[Test]
		public void ShouldNotReturnAnIssueIfFieldTypeArgumentIsResolvable()
		{
			TestWrongContext<AddUsingAction>(
				@"using System;
using System.Collections.Generic;

class Foo
{
	private List<$AttributeTargets> notifiers;
}");
		}
		
		[Test]
		public void ShouldNotReturnAnIssueIfFieldTypeArgumentIsPrimitiveType()
		{
			TestWrongContext<AddUsingAction>(
				@"using System.Collections.Generic;

class Foo
{
	private List<$string> notifiers;
}");
		}
		#endregion

		#region Method Return Types
		[Test]
		public void ShouldReturnIssueForUnresolvedReturnType()
		{
			UnresolvedTypeName(
				@"class Foo
{
	$TextWriter Bar ()
	{
		return null;
	}
}", "TextWriter", "System.IO");
		}

		[Test]
		public void ShouldNotReturnIssueForResolvedReturnType()
		{
			TestWrongContext<AddUsingAction>(
				@"using System.IO;

class Foo
{
	$TextWriter Bar ()
	{
		return null;
	}
}");
		}
		#endregion

		#region Local Variables
		[Test]
		public void ShouldReturnIssueForUnresolvedLocalVariableDeclaration()
		{
			UnresolvedTypeName(
				@"class Foo
{
	void Bar ()
	{
		$TextWriter writer;
	}
}", "TextWriter", "System.IO");
		}

		[Test]
		public void ShouldNotReturnIssueForResolvedLocalVariableDeclaration()
		{
			TestWrongContext<AddUsingAction>(
				@"using System.IO;

class Foo
{
	void Bar ()
	{
		$TextWriter writer;
	}
}");
		}
		#endregion

		#region Method Parameters
		[Test]
		public void ShouldReturnIssueIfMethodParameterIsNotResolvable()
		{
			UnresolvedTypeName(
				@"class Foo
{
	void Bar ($TextWriter writer)
	{
	}
}", "TextWriter", "System.IO");
		}

		[Test]
		public void ShouldNotReturnAnIssueIfMethodParameterIsResolvable()
		{
			TestWrongContext<AddUsingAction>(
				@"using System.IO;

class Foo
{
	void Bar ($TextWriter writer)
	{
	}
}");
		}
		#endregion

		#region Base Types
		[Test]
		public void ShouldReturnIssueIfBaseClassIsNotResolvable()
		{
			UnresolvedTypeName(
				@"class Foo : $List<string>
{
}", "List<>", "System.Collections.Generic");
		}

		[Test]
		public void ShouldNotReturnIssueIfBaseClassIsResolvable()
		{
			TestWrongContext<AddUsingAction>(
				@"using System.Collections.Generic;

class Foo : $List<string>
{
}");
		}
		
		[Test]
		public void ShouldReturnIssueIfGenericInterfaceIsMissingButNonGenericIsPresent()
		{
			UnresolvedTypeName(
				@"using System.Collections;
class Foo : $IEnumerable<string>
{
}", "IEnumerable<>", "System.Collections.Generic");
		}

		[Test]
		public void ShouldReturnIssueIfNonGenericInterfaceIsMissingButGenericIsPresent()
		{
			UnresolvedTypeName(
				@"using System.Collections.Generic;
class Foo : $IEnumerable
{
}", "IEnumerable", "System.Collections");
		}

		#endregion

		#region Member Access
		[Test]
		public void ShouldReturnIssueIfEnumValueIsNotResolvable()
		{
			UnresolvedTypeName(
				@"class Foo
{
	void Bar ()
	{
		var support = $AttributeTargets.Assembly;
	}
}", "AttributeTargets", "System");
		}

		[Test]
		public void ShouldNotReturnIssueIfEnumValueIsResolvable()
		{
			TestWrongContext<AddUsingAction>(
				@"using System;
class Foo
{
	void Bar ()
	{
		var support = $AttributeTargets.Assembly;
	}
}");
		}
		#endregion

		[Test]
		public void ShouldReturnIssueIfAttributeIsNotResolvable()
		{
			UnresolvedTypeName(
				@"[$Serializable]
class Foo
{
}", "SerializableAttribute", "System");
		}

		[Test]
		public void ShouldNotReturnIssueIfAttributeIsResolvable()
		{
			TestWrongContext<AddUsingAction>(
				@"using System;

[$Serializable]
class Foo
{
}");
		}

		[Test]
		public void ShouldReturnIssueIfTypeArgumentIsNotResolvable()
		{
			UnresolvedTypeName(
				@"using System.Collections.Generic;

class Test
{
	void TestMethod()
	{
		var list = new List<$Stream>();
	}
}", "Stream", "System.IO");
		}

		[Test]
		public void ShouldReturnIssueForUnresolvedExtensionMethod()
		{
			TestActionDescriptions(
				new AddUsingAction(),
				@"using System.Collections.Generic;

class Test
{
	void TestMethod()
	{
		var list = new List<string>();
		var first = list.$First();
	}
}", "using System.Linq;");
		}

		[Test]
		public void ShouldReturnMultipleNamespaceSuggestions()
		{
			UnresolvedTypeName(
				@"namespace A { public class TestClass { } }
namespace B { public class TestClass { } }
namespace C
{
	public class Test
	{
		private $TestClass testClass;
	}
}", "TestClass", "A", "B");
		}
		
		[Test]
		public void InnerTypeCanOnlyBeReferredToByFullName()
		{
			TestActionDescriptions(
				new AddUsingAction(),
				@"class Outer { public class Inner {} }
public class Test
{
	private $Inner t;
}
", "Outer.Inner");
		}
	}
}
