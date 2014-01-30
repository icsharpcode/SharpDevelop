// 
// RedundantAssignmentIssueTests.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
	public class RedundantAssignmentIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestVariableInitializerNotUsed()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int i = 1;
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestVariableInitializerNotUsedVar()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		var i = 1;
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestVariableAssignmentNotUsed()
		{
			var input = @"
class TestClass
{
	int TestMethod ()
	{
		int i = 1;
		int j = i;
		i = 2;
		return j;
	}
}";
			var output = @"
class TestClass
{
	int TestMethod ()
	{
		int i = 1;
		int j = i;
		return j;
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestParameterAssignmentNotUsed()
		{
			var input = @"
class TestClass
{
	int TestMethod (int i)
	{
		int j = i;
		i = 2;
		return j;
	}
}";
			var output = @"
class TestClass
{
	int TestMethod (int i)
	{
		int j = i;
		return j;
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestAssignmentInExpression()
		{
			var input = @"
class TestClass
{
	int TestMethod (int i)
	{
		int j = i = 2;
		return j;
	}
}";
			var output = @"
class TestClass
{
	int TestMethod (int i)
	{
		int j = 2;
		return j;
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestOutArgument()
		{
			var input = @"
class TestClass
{
	void Test (out int i)
	{
		i = 0;
	}
	int TestMethod ()
	{
		int i = 2;
		Test (out i);
		return i;
	}
}";
			var output = @"
class TestClass
{
	void Test (out int i)
	{
		i = 0;
	}
	int TestMethod ()
	{
		int i;
		Test (out i);
		return i;
	}
}";
			Test<RedundantAssignmentIssue>(input, 1, output);
		}

		[Test]
		public void TestOutArgument2()
		{
			var input = @"
class TestClass
{
	void Test (out int i)
	{
		i = 0;
	}
	int TestMethod ()
	{
		int i;
		Test (out i);
		i = 2;
		return i;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestRefArgument()
		{
			var input = @"
class TestClass
{
	void Test (ref int i)
	{
		i = 0;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentOperator()
		{
			var input = @"
class TestClass
{
	int TestMethod ()
	{
		int i = 1;
		i += 2;
		return i;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestIf()
		{
			var input = @"
class TestClass
{
	int TestMethod (int j)
	{
		int i = 1;
		if (j > 0) {
			i += 2;
		} else {
		}
		return i;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestConditionalExpression()
		{
			var input = @"
class TestClass
{
	int TestMethod (int j)
	{
		int i = 1;
		return j > 0 ? i : 0;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestLoop()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		var x = 0;
		for (int i = 0; i < 10; i++) {
			if (i > 5) {
				x++;
			} else {
				x = 2;
			}
		}
		if (x > 1) ;
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestForeach()
		{
			var input = @"
class TestClass
{
	void TestMethod (int[] array)
	{
		foreach (int j in array) {
			bool x = false;
			foreach (int k in array)
				foreach (int i in array)
					if (i > 5) x = true;
			if (x) break;
		}
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentInTryCatch()
		{
			var input = @"using System;
class TestClass
{
	void TestMethod ()
	{
		var a = new TestClass ();
		try {
			a = null;
		} catch (Exception) {
			if (a != null) {
				a.TestMethod ();
			}
		}
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentInTryCatchFinally()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		var a = new TestClass ();
		try {
			a = null;
		} finally {
			if (a != null) {
				a.TestMethod ();
			}
		}
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentInCatch()
		{
			var input = @"using System;
class TestClass
{
    void Test(TestClass a) { }

	void TestMethod ()
	{
		var a = new TestClass ();
		try {
		} catch (Exception) {
			a = null;
		}
        Test (a);
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentBeforeTry()
		{
			var input = @"using System;
class TestClass
{
    void Test(TestClass a) { }

	void TestMethod ()
	{
		var a = null;
		try {
			a = new TestClass ();
		} catch (Exception) {
		}
        Test (a);
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentInUsing()
		{
			var input = @"using System;
class TestClass
{
	void TestMethod ()
	{
		using (var tc = new TestClass ()) {
			// nothing
		}
	}
}";
			Test<RedundantAssignmentIssue>(input, 0);
		}

		[Test]
		public void TestAssignmentWithFunction()
		{
			var input = @"using System;
class TestClass
{
	TestClass  Func () { return null; }
	void TestMethod ()
	{
		var a = Func ();
	}
}";
			var output = @"using System;
class TestClass
{
	TestClass  Func () { return null; }
	void TestMethod ()
	{
		Func ();
	}
}";
			Test<RedundantAssignmentIssue>(input, output);
		}

        [Test]
        public void TestAssignmentWithFunctionUsedLater()
        {
            var input = @"using System;
class TestClass
{
	TestClass  Func () { return null; }
	void TestMethod ()
	{
		var a = Func ();
        a = 2;
	}
}";
            var output = @"using System;
class TestClass
{
	TestClass  Func () { return null; }
	void TestMethod ()
	{
		TestClass a;
		Func ();
        a = 2;
	}
}";
            Test<RedundantAssignmentIssue>(input, 2, output, 0);
        }


		/// <summary>
		/// Bug 11795 - Use of regex in linq statement not being recognized. 
		/// </summary>
		[Test]
		public void TestBug11795 ()
		{
			TestWrongContext<RedundantAssignmentIssue>(@"
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

public class Test
{
	public void Demo ()
	{
		Regex pattern = new Regex (@""^.*\.(jpg|png)$"", RegexOptions.IgnoreCase);
		string path = Path.Combine (""/"", ""speakers"");

		Console.WriteLine (
			from file in Directory.GetFiles (path)
			where pattern.IsMatch (file)
			select file
			);
	}

}");

		}


		/// <summary>
		/// Bug 14929 - Assignment greyed out (meaning "redundant") when it should not be
		/// </summary>
		[Test]
		public void TestBug14929 ()
		{
			TestWrongContext<RedundantAssignmentIssue>(@"
using system;

public class Test
{
	public void Demo ()
	{
		bool save = true;
		try {
			throw new Exception ();
		} catch (Exception) {
			save = false;
			throw;
		} finally {
			System.Console.WriteLine (save);
		}
	}

}");

		}

		[Test]
		public void TestMultipleVariableInitializers()
		{
			Test<RedundantAssignmentIssue>(@"using System;
public class MyClass
{
	public static void Main ()
	{
		string outputFile = null, inputFile = null;
		Console.WriteLine (outputFile);
	}
}
", 1, @"using System;
public class MyClass
{
	public static void Main ()
	{
		string outputFile = null, inputFile;
		Console.WriteLine (outputFile);
	}
}
", 0);
		}
	}
}