//
// InvalidXmlTypeReferenceIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class XmlDocIssueTests: InspectionActionTestBase
	{
		[Test]
		public void TestBeforeNamespace()
		{
			Test<XmlDocIssue>(@"
/// foo
namespace Foo {}
", @"
namespace Foo {}
");
		}

		[Test]
		public void TestBeforeUsing()
		{
			Test<XmlDocIssue>(@"
/// foo
using System;
", @"
using System;
");
		}

		[Test]
		public void TestBeforeUsingAlias()
		{
			Test<XmlDocIssue>(@"
/// foo
using A = System;
", @"
using A = System;
");
		}
		
		[Test]
		public void TestBeforeExternAlias()
		{
			Test<XmlDocIssue>(@"
/// foo
extern alias System;
", @"
extern alias System;
");
		}

		[Test]
		public void TestTypeParameter()
		{
			TestRefactoringContext ctx;
			var issues = GetIssues(new XmlDocIssue(), @"
/// <typeparam name=""Undefined""></typeparam>
class Foo {}

/// <typeparam name=""T""></typeparam>
class Foo2<T> {}
", out ctx);
			Assert.AreEqual(1, issues.Count);
		}

		[Test]
		public void TestWrongMethodParameter()
		{
			TestRefactoringContext ctx;
			var issues = GetIssues(new XmlDocIssue(), @"
class Foo {
	/// <param name=""undefined""></param>
	/// <param name=""y""></param>
	/// <param name=""z""></param>
	public void FooBar(int x, int y, int z)
	{
	}

	/// <param name=""x1""></param>
	/// <param name=""y""></param>
	int this[int x, int y] { get { return 1;  } }
}
", out ctx);
			Assert.AreEqual(2, issues.Count);
		}

		[Test]
		public void TestSeeCref()
		{
			TestRefactoringContext ctx;
			var issues = GetIssues(new XmlDocIssue(), @"
/// <summary>
/// </summary>
/// <see cref=""Undefined""/>
class Foo {
	public void Undefined () {}
}

/// <summary>
/// <seealso cref=""Foo""/>
/// </summary>
/// <see cref=""Foo2""/>
class Foo2 {}
", out ctx);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestValidCref()
		{
			TestWrongContext<XmlDocIssue>(@"
using System;

namespace Foo {
	/// <summary>
	/// </summary>
	/// <see cref=""IDisposable""/>
	class Foo {
		public void Undefined () {}
	}
}");
		}

		/// <summary>
		/// Bug 17729 - Incorrect XML-docs warning about 'value' paramref 
		/// </summary>
		[Test]
		public void TestBug17729 ()
		{
			TestWrongContext<XmlDocIssue>(@"
using System;

class Foo {
	/// <summary>
	/// If <paramref name=""value""/> is 0 ...
	/// </summary>
	public int Bar { get; set; }
}
");
		}


		[Test]
		public void TestSeeCRefMember ()
		{
			TestWrongContext<XmlDocIssue>(@"
using System;

namespace Foo
{
	public interface IGroupingProvider
	{
		IGroupingProvider Next { get; set; }
		
		/// <summary>
		/// Occurs when <see cref=""Next""/> changes.
		/// </summary>
		event EventHandler<EventArgs> NextChanged;
	}
}
");
		}

		[Test]
		public void TestEventComment ()
		{
			TestIssue<XmlDocIssue>(@"
using System;

namespace Foo
{
	public interface IGroupingProvider
	{
		/// <summa
		event EventHandler<EventArgs> NextChanged;
	}
}
", 1);
		}
	}


}

