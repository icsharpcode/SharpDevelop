// 
// DelegateParametersAreUnusedTests.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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
	public class UnusedAnonymousMethodSignatureIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleUsage ()
		{
			Test<UnusedAnonymousMethodSignatureIssue>(@"
using System;

class TestClass
{
	void TestMethod()
	{
		Action<int> x = delegate (int p) {};
	}
}", @"
using System;

class TestClass
{
	void TestMethod()
	{
		Action<int> x = delegate {};
	}
}");
		}

		[Test]
		public void TestNecessaryParameter ()
		{
			TestWrongContext<UnusedAnonymousMethodSignatureIssue>(@"
class TestClass
{
	void TestMethod()
	{
		//Even if just one is used, it's still necessary
		Action<int> x = delegate(int p, int q) { Console.WriteLine(p); };
	}
}");
		}

		[Test]
		public void TestVisitChild ()
		{
			Test<UnusedAnonymousMethodSignatureIssue>(@"
using System;

class TestClass
{
	void TestMethod()
	{
		Action<int> x = delegate(int p) {
			Console.WriteLine(p);
			Action<int> y = delegate(int q) {};
		};
	}
}", @"
using System;

class TestClass
{
	void TestMethod()
	{
		Action<int> x = delegate(int p) {
			Console.WriteLine(p);
			Action<int> y = delegate {};
		};
	}
}");
		}

		[Test]
		public void TestAmbiguousCase ()
		{
			TestWrongContext<UnusedAnonymousMethodSignatureIssue>(@"
using System;
class TestClass
{
	void Foo(Action<int> action) {}
	void Foo(Action<string> action) {}
	void TestMethod()
	{
		Foo(delegate(int p) {});
	}
}
");
		}

		/// <summary>
		/// Bug 15058 - Wrong context for unused parameter list 
		/// </summary>
		[Test]
		public void TestBug15058 ()
		{
			TestWrongContext<UnusedAnonymousMethodSignatureIssue>(@"
using System;
using System.Threading;

class TestClass
{
	void TestMethod()
	{
		var myThing = new Thread (delegate () { doStuff (); });
	}
}
");
		}
	}
}
