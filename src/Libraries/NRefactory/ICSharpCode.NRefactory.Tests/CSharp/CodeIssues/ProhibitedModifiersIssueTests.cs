//
// ProhibitedModifiersIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ProhibitedModifiersIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestNonStaticMembersInStaticClass ()
		{
			Test<ProhibitedModifiersIssue>(@"
static class Foo
{
	public void Bar () 
	{
	}
}
", @"
static class Foo
{
	public static void Bar () 
	{
	}
}
");
		}
	
		[Test]
		public void TestValidStaticClass ()
		{
			TestWrongContext<ProhibitedModifiersIssue>(@"
static class Foo
{
	public const int f = 1;
	static Foo () {}

	public static void Bar () 
	{
	}
}
");
		}
	

		[Test]
		public void TestNonStaticFieldsInStaticClass ()
		{
			Test<ProhibitedModifiersIssue>(@"
static class Foo
{
	int a, b, c;
}
", 3, @"
static class Foo
{
	static int a, b, c;
}
", 1);
		}

		[Ignore("Code issues don't run with errors atm.")]
		[Test]
		public void TestStaticConstructorWithPublicModifier()
		{
			Test<ProhibitedModifiersIssue>(@"
class Test
{
	static int a;

	public static Test ()
	{
		a = 100;
	}
}", @"
class Test
{
	static int a;

	static Test ()
	{
		a = 100;
	}
}");
		}
	
		[Test]
		public void TestVirtualMemberInSealedClass ()
		{
			Test<ProhibitedModifiersIssue>(@"
sealed class Test
{
	public virtual void FooBar ()
	{
	}
}", @"
sealed class Test
{
	public void FooBar ()
	{
	}
}");
		}
	

		[Test]
		public void TestPrivateVirtualMembers ()
		{
			TestIssue<ProhibitedModifiersIssue>(@"
class Foo
{
	virtual void Bar () 
	{
	}
}
");
		}


		[Test]
		public void TestSealed ()
		{
			Test<ProhibitedModifiersIssue>(@"
class Foo
{
	public sealed void Bar () 
	{
	}
}
", @"
class Foo
{
	public void Bar () 
	{
	}
}
");
		}

		[Test]
		public void TestValidSealed ()
		{
			TestWrongContext<ProhibitedModifiersIssue>(@"
class Foo
{
	public override sealed void Bar () 
	{
	}
}
");
		}


		[Test]
		public void TestExtensionMethodInNonStaticClass ()
		{
			Test<ProhibitedModifiersIssue>(@"
class Foo
{
	public static void Bar (this int i) 
	{
	}
}
", @"
static class Foo
{
	public static void Bar (this int i) 
	{
	}
}
");
		}

		[Test]
		public void TestExtensionMethodInNonStaticClassFix2 ()
		{
			Test<ProhibitedModifiersIssue>(@"
class Foo
{
	public static void Bar (this int i) 
	{
	}
}
", @"
class Foo
{
	public static void Bar (int i) 
	{
	}
}
", 1);
		}

	}
}

