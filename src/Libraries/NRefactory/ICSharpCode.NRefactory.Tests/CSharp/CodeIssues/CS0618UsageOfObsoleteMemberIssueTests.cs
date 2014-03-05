//
// CS0618UsageOfObsoleteMemberIssueTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0618UsageOfObsoleteMemberIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestObsoleteMethodUsage()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete (""Use NewBar"")]
	public static void OldBar ()
	{
	}

	public static void NewBar ()
	{
	}
}

class MyClass
{
	public static void Main ()
	{
		Foo.OldBar ();
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}

		[Test]
		public void TestObsoleteProperty()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete ()]
	public int Bar { get; set; }
}

class MyClass
{
	public static void Main ()
	{
		new Foo().Bar = 5;
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}

		[Test]
		public void TestObsoleteIndexer()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete ()]
	public int this[int i] {
		get {
			return 0;
		}
	}
}

class MyClass
{

	public static void Main ()
	{
		var f = new Foo ();
		Console.WriteLine (f[0]);
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}

		[Test]
		public void TestObsoleteEvent()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete ()]
	public event EventHandler A;
}

class MyClass
{

	public static void Main ()
	{
		var f = new Foo ();
		f.A += delegate {

		};
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}

		[Test]
		public void TestObsoleteField()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete]
	public int A;
}

class MyClass
{

	public static void Main ()
	{
		var f = new Foo ();
		Console.WriteLine (f.A);
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}

		[Test]
		public void TestObsoleteBinaryOperator()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete]
	public static Foo operator+(Foo l, Foo r)
	{
		return l;
	}
}

class MyClass
{
	public static void Main ()
	{
		var f = new Foo ();
		Console.WriteLine (f + f);
	}
}
";
			TestIssue<CS0618UsageOfObsoleteMemberIssue>(input);
		}


		[Test]
		public void TestPragmaDisable()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete ()]
	public int Bar { get; set; }
}

class MyClass
{
	public static void Main ()
	{
#pragma warning disable 618
		new Foo().Bar = 5;
#pragma warning restore 618
	}
}
";
			TestWrongContext<CS0618UsageOfObsoleteMemberIssue>(input);
		}


		/// <summary>
		/// Bug 17859 - Source analysis should not warn when obsolete member is used by obsolete member
		/// </summary>
		[Test]
		public void TestBug17859()
		{
			var input = @"
using System;

public class Foo
{
	[Obsolete]
	public static void OldBar ()
	{
	}

	[Obsolete]
	public static void OldBar2 ()
	{
		OldBar ();
	}
}
";
			TestWrongContext<CS0618UsageOfObsoleteMemberIssue>(input);
		}
	}
}

