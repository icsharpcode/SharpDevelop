// 
// CreateFieldTests.cs
//  
// Author:
//       Nieve Goor
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ExtractFieldTests : ContextActionTestBase
	{
		[Test]
		public void TestWrongContext1 ()
		{
			TestWrongContext<ExtractFieldAction> (
				"using System;" + Environment.NewLine +
					"class TestClass" + Environment.NewLine +
					"{" + Environment.NewLine +
					"	void Test ()" + Environment.NewLine +
					"	{" + Environment.NewLine +
					"		$foo = 2;" + Environment.NewLine +
					"	}" + Environment.NewLine +
					"}"
			);
		}
		
		[Test]
		public void TestWrongContext2 ()
		{
			TestWrongContext<ExtractFieldAction> (
				"using System;" + Environment.NewLine +
					"class TestClass" + Environment.NewLine +
					"{" + Environment.NewLine +
					"	int foo;" + Environment.NewLine +
					"	void Test ()" + Environment.NewLine +
					"	{" + Environment.NewLine +
					"		$foo = 2;" + Environment.NewLine +
					"	}" + Environment.NewLine +
					"}"
			);
		}

		[Test]
		public void TestLocalInitializer()
		{
			string result = RunContextAction (
				new ExtractFieldAction (),
				"using System;" + Environment.NewLine +
					"class TestClass" + Environment.NewLine +
					"{" + Environment.NewLine +
					"	void Test ()" + Environment.NewLine +
					"	{" + Environment.NewLine +
					"		int $foo = 5;" + Environment.NewLine +
					"	}" + Environment.NewLine +
					"}"
			);
			Console.WriteLine (result);
			Assert.AreEqual (
				"using System;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	int foo;" + Environment.NewLine +
				"	void Test ()" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		foo = 5;" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}", result);
		}

		[Test]
		public void TestLocalDeclaration()
		{
			string result = RunContextAction (
				new ExtractFieldAction (),
				"using System;" + Environment.NewLine +
					"class TestClass" + Environment.NewLine +
					"{" + Environment.NewLine +
					"	void Test ()" + Environment.NewLine +
					"	{" + Environment.NewLine +
					"		int $foo;" + Environment.NewLine +
					"	}" + Environment.NewLine +
					"}"
			);
			Console.WriteLine (result);
			Assert.AreEqual (
				"using System;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	int foo;" + Environment.NewLine +
				"	void Test ()" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}", result);
		}
		
		[Test]
		public void TestCtorParam ()
		{
			string result = RunContextAction (
				new ExtractFieldAction (),
				"using System;" + Environment.NewLine +
					"class TestClass" + Environment.NewLine +
					"{" + Environment.NewLine +
					"	TestClass (int $foo)" + Environment.NewLine +
					"	{" + Environment.NewLine +
					"		" + Environment.NewLine +
					"	}" + Environment.NewLine +
					"}"
			);

			Assert.AreEqual (
				"using System;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	int foo;" + Environment.NewLine +
				"	TestClass (int foo)" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		this.foo = foo;" + Environment.NewLine +
				"		" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}", result);
		}

		[Test]
		public void TestConstructor ()
		{
			Test<ExtractFieldAction>(@"
class TestClass
{
	TestClass () {
		int $i = 0;
	}
}", @"
class TestClass
{
	int i;
	TestClass () {
		i = 0;
	}
}");
		}

		[Test]
		public void TestGetter ()
		{
			Test<ExtractFieldAction>(@"
class TestClass
{
	static int X {
		get {
			int $i = 0;
			return i;
		}
	}
}", @"
class TestClass
{
	static int i;
	static int X {
		get {
			i = 0;
			return i;
		}
	}
}");
		}

		[Test]
		public void TestTypeInferenceDeclaration ()
		{
			Test<ExtractFieldAction> (@"
class TestClass
{
	void Test ()
	{
		var $i = 0;
	}
}", @"
class TestClass
{
	int i;
	void Test ()
	{
		i = 0;
	}
}");
		}

		[Test]
		public void TestTypeInferenceAnonymousType ()
		{
			TestWrongContext<ExtractFieldAction> (@"
class TestClass
{
	void Test ()
	{
		var $i = new { A = 1 };
	}
}
");
		}

		[Test]
		public void TestTypeInferenceAnonymousArrayType ()
		{
			TestWrongContext<ExtractFieldAction> (@"
class TestClass
{
	void Test ()
	{
		var $i = new [] { new { A = 1 } };
	}
}
");
		}

		[Test]
		public void TestStaticField ()
		{
			Test<ExtractFieldAction> (@"
class TestClass
{
	static void Test ()
	{
		int $i = 0;
	}
}", @"
class TestClass
{
	static int i;
	static void Test ()
	{
		i = 0;
	}
}");
		}
	}
}

