// 
// ExtractMethodTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ExtractMethodTests : ContextActionTestBase
	{
		[Test]
		public void SimpleArgument()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		int i = 5;
		<-Console.WriteLine (i);->
	}
}
", @"class TestClass
{
	static void NewMethod (int i)
	{
		Console.WriteLine (i);
	}
	void TestMethod ()
	{
		int i = 5;
		NewMethod (i);
	}
}
");
		}

		[Test]
		public void NoArgument()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		int i = 5;
		<-Console.WriteLine (""Hello World"");->
	}
}
", @"class TestClass
{
	static void NewMethod ()
	{
		Console.WriteLine (""Hello World"");
	}
	void TestMethod ()
	{
		int i = 5;
		NewMethod ();
	}
}
");
		}

		[Test]
		public void ExtractMethodResultStatementTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	int member = 5;
	void TestMethod ()
	{
		int i = 5;
		<-i = member + 1;->
		Console.WriteLine (i);
	}
}
", @"class TestClass
{
	int member = 5;
	void NewMethod (ref int i)
	{
		i = member + 1;
	}
	void TestMethod ()
	{
		int i = 5;
		NewMethod (ref i);
		Console.WriteLine (i);
	}
}
");
		}
		
		[Test]
		public void ExtractMethodResultExpressionTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	int member = 5;
	void TestMethod ()
	{
		int i = <-member + 1->;
		Console.WriteLine (i);
	}
}
", @"class TestClass
{
	int member = 5;
	int NewMethod ()
	{
		return member + 1;
	}
	void TestMethod ()
	{
		int i = NewMethod ();
		Console.WriteLine (i);
	}
}
");
		}
		
		[Test]
		public void ExtractMethodStaticResultStatementTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		int i = 5;
		<-i = i + 1;->
		Console.WriteLine (i);
	}
}
", @"class TestClass
{
	static void NewMethod (ref int i)
	{
		i = i + 1;
	}
	void TestMethod ()
	{
		int i = 5;
		NewMethod (ref i);
		Console.WriteLine (i);
	}
}
");
		}
		
		[Test]
		public void ExtractMethodStaticResultExpressionTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		int i = <-5 + 1->;
		Console.WriteLine (i);
	}
}
", @"class TestClass
{
	static int NewMethod ()
	{
		return 5 + 1;
	}
	void TestMethod ()
	{
		int i = NewMethod ();
		Console.WriteLine (i);
	}
}
");
		}
		
		[Test]
		public void ExtractMethodMultiVariableTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	int member;
	void TestMethod ()
	{
		int i = 5, j = 10, k;
		<-j = i + j;
		k = j + member;->
		Console.WriteLine (k + j);
	}
}
", @"class TestClass
{
	int member;
	int NewMethod (int i, ref int j)
	{
		int k;
		j = i + j;
		k = j + member;
		return k;
	}
	void TestMethod ()
	{
		int i = 5, j = 10, k;
		k = NewMethod (i, ref j);
		Console.WriteLine (k + j);
	}
}
");
		}
		
		/// <summary>
		/// Bug 607990 - "Extract Method" refactoring sometimes tries to pass in unnecessary parameter depending on selection
		/// </summary>
		[Test]
		public void TestBug607990()
		{
			Test<ExtractMethodAction>(@"using System;
class TestClass
{
	void TestMethod ()
	{
		<-Object obj1 = new Object();
		obj1.ToString();->
	}
}
", @"using System;
class TestClass
{
	static void NewMethod ()
	{
		Object obj1 = new Object ();
		obj1.ToString ();
	}
	void TestMethod ()
	{
		NewMethod ();
	}
}
");
		}
		
		
		/// <summary>
		/// Bug 616193 - Extract method passes param with does not exists any more in main method
		/// </summary>
		[Test]
		public void TestBug616193()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		string ret;
		string x;
		int y;
		<-string z = ret + y;
		ret = x + z;->
	}
}
", @"class TestClass
{
	static string NewMethod (string x, int y)
	{
		string ret;
		string z = ret + y;
		ret = x + z;
		return ret;
	}
	void TestMethod ()
	{
		string ret;
		string x;
		int y;
		ret = NewMethod (x, y);
	}
}
");
		}
		
		/// <summary>
		/// Bug 616199 - Extract method forgets to return a local var which is used in main method
		/// </summary>
		[Test]
		public void TestBug616199()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		<-string z = ""test"" + ""x"";->
		string ret = ""test1"" + z;
	}
}
", @"class TestClass
{
	static string NewMethod ()
	{
		string z = ""test"" + ""x"";
		return z;
	}
	void TestMethod ()
	{
		var z = NewMethod ();
		string ret = ""test1"" + z;
	}
}
");
		}
		
		/// <summary>
		/// Bug 666271 - "Extract Method" on single line adds two semi-colons in method, none in replaced text
		/// </summary>
		[Test]
		public void TestBug666271()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		<-TestMethod ();->
	}
}
", @"class TestClass
{
	void NewMethod ()
	{
		TestMethod ();
	}
	void TestMethod ()
	{
		NewMethod ();
	}
}
");
		}
		
		
		/// <summary>
		/// Bug 693944 - Extracted method returns void instead of the correct type
		/// </summary>
		[Test]
		public void TestBug693944()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		TestMethod (<-""Hello""->);
	}
}
", @"class TestClass
{
	static string NewMethod ()
	{
		return ""Hello"";
	}
	void TestMethod ()
	{
		TestMethod (NewMethod ());
	}
}
");
		}
		
		[Ignore("Fix me!")]
		[Test]
		public void ExtractMethodMultiVariableWithLocalReturnVariableTest()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	int member;
	void TestMethod ()
	{
		int i = 5, j = 10, k;
		<-int test;
		j = i + j;
		k = j + member;
		test = i + j + k;->
		Console.WriteLine (test);
	}
}
", @"class TestClass
{
	int member;
	void NewMethod (int i, ref int j, out int k, out int test)
	{
		j = i + j;
		k = j + member;
		test = i + j + k;
	}
	void TestMethod ()
	{
		int i = 5, j = 10, k;
		int test;
		NewMethod (i, ref j, out k, out test);
		Console.WriteLine (test);
	}
}
");
		}

		/// <summary>
		/// Bug 8835 - missing "extract method" in the code editor
		/// </summary>
		[Test]
		public void TestBug8835 ()
		{
			Test<ExtractMethodAction>(@"class TestClass
{
	void TestMethod ()
	{
		<-// comment
		Foo ();->
	}
}
", @"class TestClass
{
	static void NewMethod ()
	{
		// comment
		Foo ();
	}
	void TestMethod ()
	{
		NewMethod ();
	}
}
");
		}

		/// <summary>
		/// Bug 9881 - Extract method is broken
		/// </summary>
		[Test]
		public void TestBug9881()
		{
			Test<ExtractMethodAction>(@"using System.Linq;
class TestClass
{
	void Foo ()
	{
		var val = Enumerable.Range (0, 10).ToList ();
		<-for (int j = 0; j < val.Count; j++) {
			int i = val [j];
			Console.WriteLine (i);
		}->

		foreach (var i in val) {
			Console.WriteLine (i + 2);
		}
	}
}", @"using System.Linq;
class TestClass
{
	static void NewMethod (System.Collections.Generic.List<int> val)
	{
		for (int j = 0; j < val.Count; j++) {
			int i = val [j];
			Console.WriteLine (i);
		}
	}
	void Foo ()
	{
		var val = Enumerable.Range (0, 10).ToList ();
		NewMethod (val);

		foreach (var i in val) {
			Console.WriteLine (i + 2);
		}
	}
}");
		}

		[Test]
		public void TestStaticSimpleExtractMethodFromExpression ()
		{
			Test<ExtractMethodAction> (@"class TestClass
{
	public static void TestMethod ()
	{
		int i;
		var f = <-i % 5 == 0->;
	}
}", @"class TestClass
{
	static bool NewMethod (int i)
	{
		return i % 5 == 0;
	}
	public static void TestMethod ()
	{
		int i;
		var f = NewMethod (i);
	}
}");
		}

		/// <summary>
		/// Bug 10858 - Extract Method should use return, not out
		/// </summary>
		[Test]
		public void TestBug10858 ()
		{
			Test<ExtractMethodAction> (@"class TestClass
{
	public static void TestMethod ()
	{
		int i = 5, j, k = 7;
		
		<-j = i + k;->

		System.Console.WriteLine (j);

	}
}", @"class TestClass
{
	static int NewMethod (int i, int k)
	{
		int j;
		j = i + k;
		return j;
	}
	public static void TestMethod ()
	{
		int i = 5, j, k = 7;
		
		j = NewMethod (i, k);

		System.Console.WriteLine (j);

	}
}");
		}

		/// <summary>
		/// Bug 13054 - Extract method creates params on new method for params declared on lambdas in method body
		/// </summary>
		[Test]
		public void TestBug13054 ()
		{
			Test<ExtractMethodAction> (@"class TestClass
{
	public static void TestMethod ()
	{
		<-int i = 0;
		Action<string> action = (str) =>  {
			Console.WriteLine (str);
		};->
	}
}", @"class TestClass
{
	static void NewMethod ()
	{
		int i = 0;
		Action<string> action = str =>  {
			Console.WriteLine (str);
		};
	}
	public static void TestMethod ()
	{
		NewMethod ();
	}
}");
		}

		/// <summary>
		/// Bug 13539 - Extracting method where last line is comment leaves comment outside new method
		/// </summary>
		[Test]
		public void TestBug13539 ()
		{
			Test<ExtractMethodAction> (@"class TestClass
{
	public static void TestMethod ()
	{
		<-
			Foo ();
			// Comment
->
	}
}", @"class TestClass
{
	static void NewMethod ()
	{
		Foo ();
		// Comment
	}
	public static void TestMethod ()
	{
		NewMethod ();
	}
}");
		}

		/// <summary>
		/// Bug 11948 - Extract method incorrectly extracts code that contains yield return
		/// </summary>
		[Test]
		public void TestBug11948()
		{
			TestWrongContext<ExtractMethodAction>(@"class TestClass
{
	int member = 5;
	IEnumerable<int> TestMethod ()
	{
		int i = 5;
		<-yield return i;->
		Console.WriteLine (i);
	}
}");
		}
	}
}

