//
// MethodOverloadWithOptionalParameterIssueTests.cs
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
	public class MethodOverloadWithOptionalParameterIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSingleMethod ()
		{
			TestIssue<MethodOverloadWithOptionalParameterIssue>(@"
using System;

public class FooBar
{
	public void Print(string message)
	{
		Console.WriteLine(message);
	}

	private void Print(string message, string messageDelimiter = ""==="")
	{
		Console.WriteLine(message + messageDelimiter);
	}
}
");
		}

		[Test]
		public void TestTwoParameters ()
		{
			TestIssue<MethodOverloadWithOptionalParameterIssue>(@"
using System;

public class FooBar
{
    public void Print(string message)
    {
        Console.WriteLine(message);
    }

    public void Print(string message, string str2)
    {
        Console.WriteLine(message);
    }

    private void Print(string message, string messageDelimiter = ""==="", string secondmessage = ""==="")
	{
		Console.WriteLine(message + messageDelimiter);
	}
}", 2);
		}

		[Test]
		public void TestIndexer ()
		{
			TestIssue<MethodOverloadWithOptionalParameterIssue>(@"
using System;

public class FooBar
{
	public string this[string message]
	{
		get { return message; }
	}

	private string this[string message, string messageDelimiter = ""===""]
	{
		get { return message + messageDelimiter; }
	}
}
");
		}


		[Test]
		public void TestDisable ()
		{
			TestWrongContext<MethodOverloadWithOptionalParameterIssue>(@"
using System;

public class FooBar
{
	public void Print(string message)
	{
		Console.WriteLine(message);
	}

	// ReSharper disable once MethodOverloadWithOptionalParameter
	private void Print(string message, string messageDelimiter = ""==="")
	{
		Console.WriteLine(message + messageDelimiter);
	}
}
");
		}

		[Test]
		public void Test ()
		{
			var input = @"
class TestClass
{
	void TestMethod (int a)
	{ }
	void TestMethod (int a, int b = 1)
	{ }
}";
			Test<MethodOverloadWithOptionalParameterIssue> (input, 1);
		}

		[Test]
		public void Test2 ()
		{
			var input = @"
class TestClass
{
	void TestMethod (int a, int b)
	{ }
	void TestMethod (int a, int b = 1, int c = 1)
	{ }
}";
			Test<MethodOverloadWithOptionalParameterIssue> (input, 1);
		}

		[Test]
		public void TestNoIssue ()
		{
			var input = @"
class TestClass
{
	void TestMethod (int a, int b = 1, int c = 1)
	{ }
}";
			Test<MethodOverloadWithOptionalParameterIssue> (input, 0);
		}

		[Test]
		public void TestNoIssue_Generics ()
		{
			var input = @"
class TestClass
{
	void TestMethod (object obj) { }
	void TestMethod<T> (object obj, int arg = 0) { }
}";
			Test<MethodOverloadWithOptionalParameterIssue> (input, 0);
		}

	}
}

