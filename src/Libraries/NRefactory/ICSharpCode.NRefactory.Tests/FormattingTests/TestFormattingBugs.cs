// 
// TestFormattingBugs.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture()]
	public class TestFormattingBugs : TestBase
	{
		/// <summary>
		/// Bug 325187 - Bug in smart indent
		/// </summary>
		[Test]
		public void TestBug325187()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ElseNewLinePlacement = NewLinePlacement.NewLine;
			
			TestStatementFormatting (policy,
@"foreach (int i in myints)
if (i == 6)
Console.WriteLine (""Yeah"");
else
Console.WriteLine (""Bad indent"");",
@"foreach (int i in myints)
	if (i == 6)
		Console.WriteLine (""Yeah"");
	else
		Console.WriteLine (""Bad indent"");");
		}
		
		/// <summary>
		/// Bug 415469 - return ternary in a switch is not tabbed properly
		/// </summary>
		[Test]
		public void TestBug415469 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			
			TestStatementFormatting (policy,
@"switch (condition) {
case CONDITION1:
return foo != null ? foo.Bar : null;
case CONDITION2:
string goo = foo != null ? foo.Bar : null;
return ""Should be indented like this"";
}", @"switch (condition) {
case CONDITION1:
	return foo != null ? foo.Bar : null;
case CONDITION2:
	string goo = foo != null ? foo.Bar : null;
	return ""Should be indented like this"";
}");
		}
		
		/// <summary>
		/// Bug 540043 - Format option for alignment of using-statements
		/// </summary>
		[Test]
		public void TestBug540043 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			
			TestStatementFormatting (policy,
@"using (IDisposable a = null)
   using (IDisposable b = null) {
			int c;
   }
", @"using (IDisposable a = null)
using (IDisposable b = null) {
	int c;
}");
		}
		
		/// <summary>
		/// Bug 655635 - Auto format document doesn't indent comments as well
		/// </summary>
		[Test]
		public void TestBug655635 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			
			TestStatementFormatting (policy,
@"try {
 // Comment 1
	myObject.x = Run ();
} catch (InvalidOperationException e) {
	Console.WriteLine (e.Message);
}", @"try {
	// Comment 1
	myObject.x = Run ();
} catch (InvalidOperationException e) {
	Console.WriteLine (e.Message);
}");
		}

		void TestStatementFormatting (CSharpFormattingOptions policy, string input, string expectedOutput)
		{
			var result = GetResult (policy, @"class Test
{
	MyType TestMethod ()
	{
		" + input + @"
	}
}");
			int start = result.GetOffset (5, 1);
			int end = result.GetOffset (result.LineCount - 1, 1);
			string text = result.GetText (start, end - start).Trim ();
			expectedOutput = NormalizeNewlines(expectedOutput).Replace ("\n", "\n\t\t");
			if (expectedOutput != text)
				Console.WriteLine (text);
			Assert.AreEqual (expectedOutput, text);
		}

		/// <summary>
		///  Bug 659675 - on-the-fly code formatting breaks @-prefixed identifiers
		/// </summary>
		[Test]
		public void TestBug659675 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			TestStatementFormatting (policy, "@string=@int;", "@string = @int;");
		}
		
		/// <summary>
		/// Bug 670213 - Document formatter deletes valid text!
		/// </summary>
		[Test]
		public void TestBug670213 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MethodBraceStyle = BraceStyle.EndOfLine;
			
			Test (policy, @"class Test
{
	Test MyMethod() // Comment
	{
	}
}",
			      @"class Test
{
	Test MyMethod () { // Comment
	}
}");
		Test (policy, @"class Test
{
	Test MyMethod() // Comment
	{
		Foo ();
	}
}",
		      @"class Test
{
	Test MyMethod () { // Comment
		Foo ();
	}
}");
	}
		
		
		/// <summary>
		/// Bug 677261 - Format Document with constructor with over-indented opening curly brace
		/// </summary>
		[Test]
		public void TestBug677261 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.ConstructorBraceStyle = BraceStyle.EndOfLine;
			
			Test (policy, @"class Test
{
	Test ()
	   {
	}
}",
@"class Test
{
	Test () {
	}
}");
		}
		
		/// <summary>
		/// Bug 3586 -Format code is removing try { code blocks
		/// </summary>
		[Test]
		public void TestBug3586 ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			policy.ConstructorBraceStyle = BraceStyle.EndOfLine;
			
			Test (policy, @"public class A
{
	public object GetValue ()
        {
            try
            {
foo ();
            }
            catch
            {
            }
        }
}",
@"public class A
{
	public object GetValue ()
	{
		try {
			foo ();
		} catch {
		}
	}
}");
		}

        /// <summary>
        /// Bug GH35 - Formatter issues with if/else statements and // comments
        /// </summary>
        [Ignore]
        public void TestBugGH35()
        {
            var policy = FormattingOptionsFactory.CreateMono ();
            policy.ConstructorBraceStyle = BraceStyle.EndOfLine;

            Test(policy, @"public class A : B
{
    public void Test()
    {
        // Comment before
        if (conditionA) {
            DoSomething();
        }
        // Comment before else ends up incorporating it
        else if (conditionB) {
            DoSomethingElse();
        }
    }
}",
@"public class A : B
{
	public void Test()
	{
		// Comment before
		if (conditionA) {
			DoSomething();
		}
		// Comment before else ends up incorporating it
		else if (conditionB) {
			DoSomethingElse();
		}
	}
}");
        }

        /// <summary>
        /// Bug GH35a - Formatter issues with if/else statements and // comments else variant
        /// </summary>
        [Ignore]
        public void TestBugGH35a()
        {
            var policy = FormattingOptionsFactory.CreateMono ();
            policy.ConstructorBraceStyle = BraceStyle.EndOfLine;

            Test(policy, @"public class A : B
{
    public void Test()
    {
        // Comment before
        if (conditionA) {
            DoSomething();
        }
        // Comment before else ends up incorporating it
        else (conditionB) {
            DoSomethingElse();
        }
    }
}",
@"public class A : B
{
	public void Test()
	{
		// Comment before
		if (conditionA) {
			DoSomething();
		}
		// Comment before else ends up incorporating it
		else (conditionB) {
			DoSomethingElse();
		}
	}
}");
        }


		/// <summary>
		/// Bug 11601 - Formatting document moves "{" into the #endif line 
		/// </summary>
		[Test]
		public void TestBug11601 ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();
			policy.MethodBraceStyle = BraceStyle.EndOfLine;

			Test (policy, @"class Test
{
	#if true
	Test MyMethod () 
	#else
	Test MyMethod2 () 
	#endif	
	{
	}
}",
@"class Test
{
	#if true
	Test MyMethod () 
	#else
	Test MyMethod2 () 
	#endif	
	{
	}
}");
		}
	

		/// <summary>
		/// Bug 12270 - Code formatter breaks new() constraints
		/// </summary>
		[Test]
		public void TestBug12270()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy,
@"class C
{
	public void Test<T> () where T : new  ()
	{
	}
}",
@"class C
{
	public void Test<T> () where T : new()
	{
	}
}");
		}

		/// <summary>
		/// Bug 14324 - System.ArgumentOutOfRangeException while formatting C# code
		/// </summary>
		[Test]
		public void TestBug14324()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy,
			     @"class C
{
	public void Test ()
	{
		if
(true ||
false) {
}
	}
}",
			     @"class C
{
	public void Test ()
	{
		if (true ||
		false) {
		}
	}
}");
		}



		/// <summary>
		/// Bug 14957 - System.InvalidOperationException: Sequence contains no elements
		/// </summary>
		[Test]
		public void TestBug14957()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.ForceOneLine;
			Test(policy,
				@"class X
{
	public int Foo { get {
	} }
}", @"class X
{
	public int Foo { get { } }
}");
		}

		/// <summary>
		/// Bug 15218 - Saving causing a crash
		/// </summary>
		[Test]
		public void TestBug15218()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.ForceOneLine;
			Test(policy,
				@"class Foo
{
	void Bar ()
	{
		foo += delegate(object s, EventArgs a)
		{
			var myDelegate = delegate()
			{
					
			};
		};  
	}
}
", @"class Foo
{
	void Bar ()
	{
		foo += delegate(object s, EventArgs a) {
			var myDelegate = delegate() {
					
			};
		};  
	}
}
");
		}


		/// <summary>
		/// Bug 17047 - Smart indent failure
		/// </summary>
		[Test]
		public void TestBug17047()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ChainedMethodCallWrapping = Wrapping.WrapAlways;
			Test(policy,
				@"class Foo
{
	void Bar ()
	{
		Observable.Merge (shotStats, touchStats)
			.GroupBy (e => e.Type)
			.Subscribe (statGroup => {
				Stats.Add (new StatItemViewModel (statGroup));
			});
	}
}
", @"class Foo
{
	void Bar ()
	{
		Observable.Merge (shotStats, touchStats)
			.GroupBy (e => e.Type)
			.Subscribe (statGroup => {
				Stats.Add (new StatItemViewModel (statGroup));
			});
	}
}
");
		}
	}
}

