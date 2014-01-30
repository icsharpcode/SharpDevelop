// 
// NonReadonlyReferencedInGetHashCodeTetsts.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class NonReadonlyReferencedInGetHashCodeTetsts : InspectionActionTestBase
	{
		
		[Test]
		public void TestInspectorCase1()
		{
			var input = @"using System;
public class TestClass1
{
	public int a = 1;
}

public class TestClass2
{
	public override int GetHashCode()
	{
		TestClass1 c = new TestClass1();
		int b = 1;
		b++;
		return c.a;
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new NonReadonlyReferencedInGetHashCodeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}
		
		[Test]
		public void TestInspectorCase2()
		{
			var input = @"using System;
public class TestClass1
{
	public int a = 1;
}

public class TestClass2
{
	private int b;
	public override int GetHashCode()
	{
		TestClass1 c = new TestClass1();
		b++;
		return c.a;
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new NonReadonlyReferencedInGetHashCodeIssue(), input, out context);
			Assert.AreEqual(2, issues.Count);
		}
		
		[Test]
		public void TestInspectorCase3()
		{
			var input = @"using System;
public class TestClass1
{
	public int a = 1;
}

public class TestClass2
{
	public override int GetHashCode()
	{
		TestClass1 c = new TestClass1();
		int b = 1;
		b++;
		return c.GetHashCode();
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new NonReadonlyReferencedInGetHashCodeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void TestInspectorCase4()
		{
			var input = @"public class Test1
{
	public int a = 1;
}

public class Test2
{
	private int q;
	public Test1 r;
	public override int GetHashCode()
	{
		Test1 c = new Test1();
		q = 1 + q + r.a;
		return c.a;
	}
}


public class Test3
{
	private int q;
	public Test2 r;
	public override int GetHashCode()
	{
		Test1 c = new Test1();
		c.GetHashCode();
		q = 1 + q + r.r.a;
		return c.a;
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new NonReadonlyReferencedInGetHashCodeIssue(), input, out context);
			Assert.AreEqual(11, issues.Count);
		}
		
		
		[Test]
		public void TestResharperDisableRestore()
		{
			var input = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resharper_test
{
	class Foo
	{
		private readonly int fooval;
		private int tmpval;

		public override int GetHashCode()
		{
			int a = 6;
//Resharper disable NonReadonlyReferencedInGetHashCode
			tmpval = a + 3;
//Resharper restore NonReadonlyReferencedInGetHashCode
			a = tmpval + 5;
			return fooval;
		}
	}
}
";
			
			TestRefactoringContext context;
			var issues = GetIssues(new NonReadonlyReferencedInGetHashCodeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}
	}
}