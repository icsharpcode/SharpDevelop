// 
// TastBlankLineFormatting.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture()]
	public class TestBlankLineFormatting : TestBase
	{
		[Test]
		public void TestBlankLinesAfterUsings ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesAfterUsings = 2;
			
			var adapter = Test (policy, @"using System;
using System.Text;
namespace Test
{
}",
@"using System;
using System.Text;


namespace Test
{
}", FormattingMode.Intrusive);
		}

		[Test]
		public void TestBlankLinesBeforeUsings ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesAfterUsings = 0;
			policy.MinimumBlankLinesBeforeUsings = 2;
			
			var adapter = Test (policy, @"using System;
using System.Text;
namespace Test
{
}",
@"

using System;
using System.Text;
namespace Test
{
}", FormattingMode.Intrusive);
		}

		[Test]
		public void TestBlankLinesBeforeFirstDeclaration ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesBeforeFirstDeclaration = 2;
			
			var adapter = Test (policy, @"namespace Test
{
	class Test
	{
	}
}",
@"namespace Test
{


	class Test
	{
	}
}", FormattingMode.Intrusive);
		
		}

		[Test]
		public void TestBlankLinesBetweenTypes ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesBetweenTypes = 1;
			
			var adapter = Test (policy, @"namespace Test
{
	class Test1
	{
	}
	class Test2
	{
	}
	class Test3
	{
	}
}",
@"namespace Test
{
	class Test1
	{
	}

	class Test2
	{
	}

	class Test3
	{
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBlankLinesBetweenFields ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesBetweenFields = 1;
			
			var adapter = Test (policy, @"class Test
{
	int a;
	int b;
	int c;
}",
@"class Test
{
	int a;

	int b;

	int c;
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBlankLinesBetweenEventFields ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesBetweenEventFields = 1;
			
			var adapter = Test (policy, @"class Test
{
	public event EventHandler a;
	public event EventHandler b;
	public event EventHandler c;
}",
@"class Test
{
	public event EventHandler a;

	public event EventHandler b;

	public event EventHandler c;
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBlankLinesBetweenMembers ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesBetweenMembers = 1;

			var adapter = Test (policy, @"class Test
{
	void AMethod ()
	{
	}
	void BMethod ()
	{
	}
	void CMethod ()
	{
	}
}", @"class Test
{
	void AMethod ()
	{
	}

	void BMethod ()
	{
	}

	void CMethod ()
	{
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBlankLinesAroundRegion ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesAroundRegion = 2;

			var adapter = Test (policy, @"class Test
{
	#region FooBar
	
	void AMethod ()
	{
	}
	
	#endregion
	void BMethod ()
	{
	}
}", @"class Test
{
	#region FooBar

	void AMethod ()
	{
	}

	#endregion


	void BMethod ()
	{
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBlankLinesInsideRegion ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MinimumBlankLinesInsideRegion = 2;
			var adapter = Test (policy, @"class Test
{
	#region FooBar
	void AMethod ()
	{
	}
	#endregion
}", @"class Test
{
	#region FooBar


	void AMethod ()
	{
	}


	#endregion
}", FormattingMode.Intrusive);

		}

		/// <summary>
		/// Bug 13373 - XS adding linefeeds within #ifs
		/// </summary>
		[Test]
		public void TestBug13373_Global ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();

			Test (policy, @"
#if false
class Test2
{
}
#endif
",
			                    @"
#if false
class Test2
{
}
#endif
", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBug13373_TypeLevel ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();

			Test (policy, 
@"class Foo
{
	#if false
	class Test2
	{
	}
	#endif
}
",
@"class Foo
{
	#if false
	class Test2
	{
	}
	#endif
}
", FormattingMode.Intrusive);

		}

		[Test]
		public void TestBug13373_StatementLevel ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();

			Test (policy, 
			                    @"class Foo
{
	void Test ()
	{
		#if false
		class Test2
		{
		}
		#endif
	}
}",
@"class Foo
{
	void Test ()
	{
		#if false
		class Test2
		{
		}
		#endif
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestAddIndentOnBlankLines ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.EmptyLineFormatting = EmptyLineFormatting.Indent;

			Test (policy, @"class Foo
{
	void Test ()
	{
	}
	void Test2 ()
	{
	}
}", @"class Foo
{
	void Test ()
	{
	}
	
	void Test2 ()
	{
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestRemoveIndentOnBlankLines ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.EmptyLineFormatting = EmptyLineFormatting.DoNotIndent;

			Test (policy, @"class Foo
{
	void Test ()
	{
	}
	
	void Test2 ()
	{
	}
}", @"class Foo
{
	void Test ()
	{
	}

	void Test2 ()
	{
	}
}", FormattingMode.Intrusive);

		}

		[Test]
		public void TestDoNotChangeIndentOnBlankLines ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.EmptyLineFormatting = EmptyLineFormatting.DoNotChange;

			var indented = @"class Foo
{
	void Test ()
	{
	}
	
	void Test2 ()
	{
	}
}";
			var nonIndented = @"class Foo
{
	void Test ()
	{
	}

	void Test2 ()
	{
	}
}";
			Test(policy, indented, indented, FormattingMode.Intrusive);
			Test(policy, nonIndented, nonIndented, FormattingMode.Intrusive);
		}
	}
}

