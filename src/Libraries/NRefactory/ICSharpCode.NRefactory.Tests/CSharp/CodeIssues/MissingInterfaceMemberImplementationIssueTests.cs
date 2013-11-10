//
// MissingInterfaceMemberImplementationIssueTests.cs
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
	public class MissingInterfaceMemberImplementationIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestUnimplementedInterface ()
		{
			TestIssue<MissingInterfaceMemberImplementationIssue>(@"
interface IF
{
	void Foo();
}

class Test : IF
{
}
");
		}

		[Test]
		public void TestPartiallyImplementedInterface ()
		{
			TestIssue<MissingInterfaceMemberImplementationIssue>(@"
interface IF
{
	void Foo();
	void Foo2();
}

class Test : IF
{
	public void Foo()
	{
	}
}
");
		}

		[Test]
		public void TestMultiple ()
		{
			TestIssue<MissingInterfaceMemberImplementationIssue>(@"
interface IF
{
	void Foo();
}

interface IB
{
	void Bar();
	void Bar2();
}


class Test : IF, IB
{
	public void Bar()
	{
	}
}
", 2);
		}

		[Test]
		public void TestImplementedInterface ()
		{
			TestWrongContext<MissingInterfaceMemberImplementationIssue>(@"
interface IF
{
	void Foo();
}

class Test : IF
{
	public void Foo()
	{
	}
}
");
		}


		[Test]
		public void TestInterfaceInheritance ()
		{
			TestWrongContext<MissingInterfaceMemberImplementationIssue>(@"
	public interface IService
	{
		string ServiceName { get; }
	}

	public interface IExtensionService : IService
	{
		void Initialize ();
	}
");
		}


		/// <summary>
		/// Bug 14944 - Incorrect issue context for Interface not implemented 
		/// </summary>
		[Test]
		public void TestBug14944 ()
		{
			TestWrongContext<MissingInterfaceMemberImplementationIssue>(@"
using System.Collections;
using System.Collections.Generic;
using System;

abstract class Test<T> : IEnumerable, IEnumerable<T>
{
	#region IEnumerable implementation

	public IEnumerator<T> GetEnumerator ()
	{
		throw new NotImplementedException ();
	}

	#endregion

	#region IEnumerable implementation

	IEnumerator IEnumerable.GetEnumerator ()
	{
		throw new NotImplementedException ();
	}
	#endregion
}
");
		}


		[Test]
		public void TestAlreadyImplementedByInheritance ()
		{
			TestWrongContext<MissingInterfaceMemberImplementationIssue>(@"
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class FooBase<T> : IEnumerable, IEnumerable<T>
{
	public IEnumerator<T> GetEnumerator ()
	{
		return null;
	}

	IEnumerator IEnumerable.GetEnumerator ()
	{
		return null;
	}
}

public class Foo<T> : FooBase<T>, IEnumerable, IEnumerable<T>
{
}
");
		}


	}
}

