using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeIssues;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeActions.AddUsing
{
	[TestFixture]
	public class AddUsingActionInsideNamespaceTests : ContextActionTestBase
	{
		[Test]
		public void ShouldInsertUsingStatement()
		{
			string testCode =
@"namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"namespace TestNamespace
{
	using System.Collections.Generic;
	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			formattingOptions.MinimumBlankLinesAfterUsings = 0;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddBlankLinesBeforeUsingStatement()
		{
			string testCode =
@"namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"namespace TestNamespace
{


	using System.Collections.Generic;
	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			formattingOptions.MinimumBlankLinesBeforeUsings = 2;
			formattingOptions.MinimumBlankLinesAfterUsings = 0;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddBlankLinesAfterUsingStatements()
		{
			string testCode =
@"namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"namespace TestNamespace
{
	using System.Collections.Generic;


	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			formattingOptions.MinimumBlankLinesAfterUsings = 2;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldAddUsingAfterExistingUsings()
		{
			string testCode =
@"namespace TestNamespace
{
	using System;

	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"namespace TestNamespace
{
	using System;
	using System.Collections.Generic;

	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddUsingInMostNestedNamespace()
		{
			string testCode =
@"namespace OuterNamespace
{
	namespace InnerNamespace
	{
		class TestClass
		{
			private $List<string> stringList;
		}
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	namespace InnerNamespace
	{
		using System.Collections.Generic;

		class TestClass
		{
			private List<string> stringList;
		}
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldAddUsingAfterExistingUsingsInMostNestedNamespace()
		{
			string testCode =
@"namespace OuterNamespace
{
	namespace InnerNamespace
	{
		using System;

		class TestClass
		{
			private $List<string> stringList;
		}
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	namespace InnerNamespace
	{
		using System;
		using System.Collections.Generic;

		class TestClass
		{
			private List<string> stringList;
		}
	}
}";

			formattingOptions.UsingPlacement = UsingPlacement.InsideNamespace;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}
	}
}

