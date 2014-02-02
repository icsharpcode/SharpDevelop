//
// CS0169FieldIsNeverUsedIssueTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0169FieldIsNeverUsedIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestInitializedField ()
		{
			TestWrongContext<CS0169FieldIsNeverUsedIssue>(@"class Test
{
	object fooBar = new object ();
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}

		[Test]
		public void TestFieldAssignedInConstructor ()
		{
			TestWrongContext<CS0169FieldIsNeverUsedIssue>(@"class Test
{
	object fooBar;
	public Test ()
	{
		fooBar = new object ();
	}
	public static void Main (string[] args)
	{
		Console.WriteLine (fooBar);
	}
}");
		}
	
		[Test]
		public void TestUnassignedField ()
		{
			TestIssue<CS0169FieldIsNeverUsedIssue>(@"class Test
{
	object fooBar;
}");
		}



		[Test]
		public void TestFieldOnlyUsed ()
		{
			TestIssue<CS0169FieldIsNeverUsedIssue>(@"class Test
{
	object fooBar;
	public Test ()
	{
		Console.WriteLine (fooBar);
	}
}");
		}


		[Test]
		public void TestReadonlyField ()
		{
			TestIssue<CS0169FieldIsNeverUsedIssue>(@"class Test
{
	readonly object fooBar;
}");
		}


		[Test]
		public void TestPragmaDisable ()
		{
			TestWrongContext<CS0169FieldIsNeverUsedIssue>(@"class Test
{
#pragma warning disable 169
	object fooBar;
#pragma warning restore 169
	public Test ()
	{
		Console.WriteLine (fooBar);
	}
}");
		}
	}
}

