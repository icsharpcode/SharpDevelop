using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeIssues;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeActions.AddUsing
{
	[TestFixture]
	public class AddUsingRunActionTests : ContextActionTestBase
	{
		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
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
@"using System.Collections.Generic;

namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddBlankLinesAfterUsings()
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
@"using System.Collections.Generic;


namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.MinimumBlankLinesAfterUsings = 2;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddBlankLinesBeforeUsing()
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
@"

using System.Collections.Generic;

namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			formattingOptions.MinimumBlankLinesBeforeUsings = 2;
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddAfterExistingUsingStatements()
		{
			string testCode =
@"using System;
namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"using System;
using System.Collections.Generic;

namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldNotAddBlankLinesAfterIfTheyAreAlreadyThere()
		{
			string testCode =
@"using System;

namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"using System;
using System.Collections.Generic;

namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldLeaveAdditionalBlankLinesThatAlreadyExist()
		{
			string testCode =
@"using System;


namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"using System;
using System.Collections.Generic;


namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddFirstUsingAfterComments()
		{
			string testCode =
@"// This is the file header.
// It contains any copyright information.
namespace TestNamespace
{
	class TestClass
	{
		private $List<string> stringList;
	}
}";

			string expectedOutput = 
@"// This is the file header.
// It contains any copyright information.
using System.Collections.Generic;

namespace TestNamespace
{
	class TestClass
	{
		private List<string> stringList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldBeAbleToFixAttributeWithShortName()
		{
			string testCode =
@"namespace TestNamespace
{
	[$Serializable]
	class TestClass
	{
	}
}";

			string expectedOutput = 
@"using System;

namespace TestNamespace
{
	[Serializable]
	class TestClass
	{
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}
	}
}

