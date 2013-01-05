using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeActions.AddUsing
{
	[TestFixture]
	public class AddUsingActionAlphabeticalTests : ContextActionTestBase
	{
		[Test]
		public void ShouldAddUsingAtStartIfItIsTheFirstAlphabetically()
		{
			string testCode =
@"namespace OuterNamespace
{
	using System.IO;

	class TestClass
	{
		private $List<TextWriter> writerList;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System.Collections.Generic;
	using System.IO;

	class TestClass
	{
		private List<TextWriter> writerList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}
		
		[Test]
		public void ShouldInsertUsingBetweenExistingUsings()
		{
			string testCode =
@"namespace OuterNamespace
{
	using System;
	using System.IO;

	class TestClass
	{
		private $List<TextWriter> writerList;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	class TestClass
	{
		private List<TextWriter> writerList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}
		
		[Test]
		public void ShouldInsertUsingAfterExistingUsings()
		{
			string testCode =
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;

	class TestClass
	{
		private List<$TextWriter> writerList;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	class TestClass
	{
		private List<TextWriter> writerList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}
		
		[Test]
		[Ignore("Add using does not honor the blank line setting yet")]
		public void ShouldAddBlankLinesAfterUsingsWhenAddingAtEnd()
		{
			string testCode =
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;
	class TestClass
	{
		private List<$TextWriter> writerList;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	class TestClass
	{
		private List<TextWriter> writerList;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		#region System Namespaces
		
		[Test]
		public void ShouldBeAbleToPlaceSystemNamespacesFirst()
		{
			string testCode =
@"namespace OuterNamespace
{
	using ANamespace;

	class TestClass
	{
		private $TextWriter writer;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System.IO;
	using ANamespace;

	class TestClass
	{
		private TextWriter writer;
	}
}";
			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldNotPlaceNonSystemNamespacesStartingWithSystemFirst()
		{
			string testCode =
@"namespace A { class B { } }
namespace OuterNamespace
{
	using SystemA;

	class TestClass
	{
		private $B b;
	}
}";

			string expectedOutput = 
@"namespace A { class B { } }
namespace OuterNamespace
{
	using A;
	using SystemA;

	class TestClass
	{
		private B b;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		[Test]
		public void ShouldPlaceSystemBeforeOtherNamespaces()
		{
			string testCode =
@"namespace OuterNamespace
{
	using System.Collections.Generic;

	class TestClass
	{
		private List<$DateTime> dates;
	}
}";

			string expectedOutput = 
@"namespace OuterNamespace
{
	using System;
	using System.Collections.Generic;

	class TestClass
	{
		private List<DateTime> dates;
	}
}";

			Test(new AddUsingAction(), testCode, expectedOutput);
		}

		#endregion
	}
}

