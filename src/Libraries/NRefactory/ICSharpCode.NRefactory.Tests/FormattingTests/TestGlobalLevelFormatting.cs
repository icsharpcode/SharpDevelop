//
// TestGlobalLevelFormatting.cs
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
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture]
	public class TestGlobalLevelFormatting : TestBase
	{
		[Test]
		public void TestGlobalAttributes ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy,
			      @"[assembly: AssemblyDescription   (""""  )   ]",
			      @"[assembly: AssemblyDescription ("""")]");
		}

		/// <summary>
		/// Bug 13361 - Format Document partially removes pragmas (#pragma
		/// </summary>
		[Test]
		public void TestBug13361()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, 
			     @"#pragma warning disable 0219

class Foo
{
	#pragma warning disable 123
	void Foo ()
	{
		#pragma warning disable 123
	}
}", @"#pragma warning disable 0219

class Foo
{
	#pragma warning disable 123
	void Foo ()
	{
		#pragma warning disable 123
	}
}");
		}

		/// <summary>
		/// Bug 13413 - Formatter inserts new line between #undef statements in an #if block on every reformat 
		/// </summary>
		[Test]
		public void TestBug13413()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy, 
			     @"#if foo
#undef a
#undef b
#undef c
#undef d
#endif
", @"#if foo
#undef a
#undef b
#undef c
#undef d
#endif
");
		}

		[Test]
		public void TestUsingBlankLines ()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.MinimumBlankLinesBeforeUsings = 2;
			Test(policy,
			      @"//
using System;",
			      @"//


using System;");
		}

		[Test]
		public void TestUsingBlankLinesCase2 ()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.MinimumBlankLinesBeforeUsings = 2;
			Test(policy,
			       @"//


using System;",
			       @"//


using System;");
		}

		[Test]
		public void TestUsingBlankLinesCase3 ()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.MinimumBlankLinesBeforeUsings = 2;
			Test(policy,
			      @"

using System;",
			      @"

using System;");
		}

		[Test]
		public void TestUsingBlankLinesCase4 ()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.MinimumBlankLinesBeforeUsings = 2;
			Test (policy,
			      @"using System;",
			      @"

using System;");

		}
	}
}
