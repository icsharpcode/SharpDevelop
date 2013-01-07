using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class SortUsingsTests : ContextActionTestBase
	{
		[Test]
		public void TestActiveWhenCursorAtUsing()
		{
			Test<SortUsingsAction>(@"using Sys$tem.Linq;
using System;", @"using System;
using System.Linq;");
		}

		[Test]
		public void TestActiveWhenCursorBehindUsing()
		{
			Test<SortUsingsAction>(@"using System.Linq;$
using System;", @"using System;
using System.Linq;");
		}

		[Test]
		public void TestInActiveWhenCursorOutsideUsings()
		{
			TestWrongContext<SortUsingsAction>(@"using System.Linq;
using System;
$");
		}

		[Test]
		public void TestSortsAllUsingBlocksInFile()
		{
			Test<SortUsingsAction>(@"using $System.Linq;
using System;

namespace Foo
{
	using System.IO;
	using System.Collections;
}

namespace Bar
{
	using System.IO;
	using System.Runtime;
	using System.Diagnostics;
}", @"using System;
using System.Linq;

namespace Foo
{
	using System.Collections;
	using System.IO;
}

namespace Bar
{
	using System.Diagnostics;
	using System.IO;
	using System.Runtime;
}");
		}

		[Test]
		public void TestAliasesGoesToTheEnd()
		{
			Test<SortUsingsAction>(@"$using Sys = System;
using System;", @"using System;
using Sys = System;");
		}

		[Test]
		public void TestUnknownNamespacesGoesAfterKnownOnes()
		{
			Test<SortUsingsAction>(@"$using Foo;
using System;", @"using System;
using Foo;");
		}

		[Test]
		public void TestMixedStuff()
		{
			Test<SortUsingsAction>(@"$using Foo;
using System.Linq;
using Sys = System;
using System;
using FooAlias = Foo;
using Linq = System.Linq;", @"using System;
using System.Linq;
using Foo;
using Linq = System.Linq;
using Sys = System;
using FooAlias = Foo;");
		}

		[Test]
		public void TestPreservesEmptyLinesWhichIsInFactABug()
		{
			Test<SortUsingsAction>(@"$using System.Linq;

using System;", @"using System;

using System.Linq;");
		}
		
		
		[Test]
		public void TestPreservesPreprocessorDirectives()
		{
			Test<SortUsingsAction>(@"$using D;
using A;
#if true
using C;
using B;
#endif", @"using A;
using D;
#if true
using B;
using C;
#endif");
		}
	}
}
