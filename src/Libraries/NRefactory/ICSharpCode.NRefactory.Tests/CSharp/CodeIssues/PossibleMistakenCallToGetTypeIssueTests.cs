//
// PossibleMistakenCallToGetTypeIssueTests.cs
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
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class PossibleMistakenCallToGetTypeIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestGetTypeCase ()
		{
			Test<PossibleMistakenCallToGetTypeIssue>(@"
using System;

public class Bar
{
	public void FooBar (Type a)
	{
		Console.WriteLine (a.GetType ());
	}
}
", @"
using System;

public class Bar
{
	public void FooBar (Type a)
	{
		Console.WriteLine (a);
	}
}
");
		}

		[Test]
		public void TestStaticCall ()
		{
			TestWrongContext<PossibleMistakenCallToGetTypeIssue>(@"
using System;

public class Bar
{
	public void FooBar(Type a)
	{
		string abc = ""def"";
		Type.GetType (abc, true);
	}
}
");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<PossibleMistakenCallToGetTypeIssue>(@"
public class Bar
{
	public void FooBar(Type a)
	{
		// ReSharper disable once PossibleMistakenCallToGetType
		Console.WriteLine(a.GetType());
	}
}
");
		}


	}
}

