//
// ConstructFixerTests.cs
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
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture]
	public class ConstructFixerTests
	{
		static void Test (string input, string expectedOutput)
		{
			input = input.Replace("\r\n", "\n");
			expectedOutput = expectedOutput.Replace("\r\n", "\n");
			int caretPositon = input.IndexOf('$');
			if (caretPositon > 0)
				input = input.Substring(0, caretPositon) + input.Substring(caretPositon + 1);

			var document1 = new StringBuilderDocument(input);

			int expectedCaretPosition = expectedOutput.IndexOf('$');
			if (expectedCaretPosition > 0)
				expectedOutput = expectedOutput.Substring(0, expectedCaretPosition) + expectedOutput.Substring(expectedCaretPosition + 1);

			var fixer = new ConstructFixer(FormattingOptionsFactory.CreateMono (), new TextEditorOptions { EolMarker = "\n" });
			int newCaretPosition;
			Assert.IsTrue(fixer.TryFix(document1, caretPositon, out newCaretPosition));   
			var isEqual = expectedOutput == document1.Text.Replace("\r\n", "\n");
			if (!isEqual) {
				System.Console.WriteLine("expected:");
				System.Console.WriteLine(expectedOutput);
				System.Console.WriteLine("was:");
				System.Console.WriteLine(document1.Text);
			}
			Assert.IsTrue(isEqual); 
			Assert.AreEqual(expectedCaretPosition, newCaretPosition); 
		}

		[Test]
		public void TestMethodCallCase1()
		{
			Test(
				@"
class Foo
{
	void Bar (int i)
	{
		Bar (42$
	}
}", @"
class Foo
{
	void Bar (int i)
	{
		Bar (42);$
	}
}");
		}

		[Test]
		public void TestMethodCallCase2()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		Bar$ (;
	}
}", @"
class Foo
{
	void Bar ()
	{
		Bar ();$
	}
}");
		}

		[Test]
		public void TestMethodCallCase3()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		Bar ($)
	}
}", @"
class Foo
{
	void Bar ()
	{
		Bar ();$
	}
}");
		}
	
		[Test]
		public void TestClassDeclaration()
		{
			Test(@"class Foo$", @"class Foo
{
	$
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestClassDeclaration_TestErrorRecovery()
		{
			Test(@"class Foo$
// Foo
", @"class Foo
{
	$
}

// Foo");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestDelegateDeclaration()
		{
			Test(
@"delegate void FooHandler (object foo$", 
@"delegate void FooHandler (object foo);
$");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestMethodDeclaration()
		{
			Test(
				@"
class Foo
{
	void Bar (int i$
}
", @"
class Foo
{
	void Bar (int i)
	{
		$
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestFieldDeclaration()
		{
			Test(
				@"
class Foo
{
	int f$
}", @"
class Foo
{
	inf f;
}");
		}


		[Test]
		public void TestIfStatement()
		{
			Test(
				@"
class Foo
{
	void Bar (int i)
	{
		if (true$
	}
}", @"
class Foo
{
	void Bar (int i)
	{
		if (true) {
			$
		}
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestFixedStatement()
		{
			Test(
				@"
unsafe class Foo
{
	void Bar (int i)
	{
		fixed (int* ptr = &i$
	}
}", @"
unsafe class Foo
{
	void Bar (int i)
	{
		fixed (int* ptr = &i) {
			$
		}
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestUsingStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		using (var foo = new Foo()$
	}
}", @"
class Foo
{
	void Bar ()
	{
		using (var foo = new Foo()) {
			$
		}
	}
}");
		}

		[Test]
		public void TestLockStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		lock (foo$
	}
}", @"
class Foo
{
	void Bar ()
	{
		lock (foo) {
			$
		}
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestForeachStatement()
		{
			Test(
				@"
class Foo
{
	void Bar (string s)
	{
		foreach (var c in s$
	}
}", @"
class Foo
{
	void Bar (string s)
	{
		foreach (var c in s) {
			$
		}
	}
}");
		}

		[Test]
		public void TestWhileStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		while (true$
	}
}", @"
class Foo
{
	void Bar ()
	{
		while (true) {
			$
		}
	}
}");
		}


		[Test]
		public void TestSwitch()
		{
			Test(
				@"
class Foo
{
	void Bar (int i)
	{
		switch (i$
	}
}
", @"
class Foo
{
	void Bar (int i)
	{
		switch (i) {
			$
		}
	}
}
");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestCase()
		{
			Test(
				@"
class Foo
{
	void Bar (int i)
	{
		switch (i) {
			case 1$
		}
	}
}
", @"
class Foo
{
	void Bar (int i)
	{
		switch (i) {
			case 1:
				$
		}
	}
}");
		}


		[Ignore("Fixme - parser error")]
		[Test]
		public void TestBreakStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		break$
	}
}", @"
class Foo
{
	void Bar ()
	{
		break;
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestGotoStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		goto foo$
	}
}", @"
class Foo
{
	void Bar ()
	{
		goto foo;
	}
}");
		}

		[Test]
		public void TestReturnStatement()
		{
			Test(
				@"
class Foo
{
	int Bar ()
	{
		return 5$
	}
}", @"
class Foo
{
	int Bar ()
	{
		return 5;$
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestYieldBreakStatement()
		{
			Test(
				@"
class Foo
{
	int Bar ()
	{
		yield break$
	}
}", @"
class Foo
{
	int Bar ()
	{
		yield break;$
	}
}");
		}

		[Test]
		public void TestYieldReturnStatement()
		{
			Test(
				@"
class Foo
{
	int Bar ()
	{
		yield return 5$
	}
}", @"
class Foo
{
	int Bar ()
	{
		yield return 5;$
	}
}");
		}

		[Test]
		public void TestThrowStatement()
		{
			Test(
				@"
class Foo
{
	int Bar ()
	{
		throw new Exception()$
	}
}", @"
class Foo
{
	int Bar ()
	{
		throw new Exception();$
	}
}");
		}


		[Test]
		public void TestCheckedStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		checked$
	}
}", @"
class Foo
{
	void Bar ()
	{
		checked {
			$
		}
	}
}");
		}

		[Test]
		public void TestUncheckedStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		unchecked$
	}
}", @"
class Foo
{
	void Bar ()
	{
		unchecked {
			$
		}
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestUnsafeStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		unsafe$
	}
}", @"
class Foo
{
	void Bar ()
	{
		unsafe {
			$
		}
	}
}");
		}


		[Ignore("Fixme - parser error")]
		[Test]
		public void TestContinueStatement()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		continue$
	}
}", @"
class Foo
{
	void Bar ()
	{
		continue;
	}
}");
		}


		[Test]
		public void TestDoWhile()
		{
			Test(
				@"
class Foo
{
	void Bar ()
	{
		do {
		} while (true$
	}
}", @"
class Foo
{
	void Bar ()
	{
		do {
		} while (true);$
	}
}");
		}

		[Ignore("Fixme - parser error")]
		[Test]
		public void TestComplexCase()
		{
			Test(
				@"
class Foo
{
	bool Bar (int i)
	{
		if$ (Bar (1
	}
}", @"
class Foo
{
	bool Bar (int i)
	{
		if (Bar ((((Bar((1))))))) {
			$
		}
	}
}");
		}
	}
}

