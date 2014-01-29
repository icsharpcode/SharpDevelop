// 
// RedundantOverridenMemberTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun<jikun.nus@gmail.com>
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
	public class RedundantOverridenMemberTests : InspectionActionTestBase
	{
		[Test]
		public void TestInspectorCase1()
		{
			var input = @"namespace Demo
{
	public class BaseClass
	{
		virtual public void run()
		{
			int a = 1+1;
		}
	}
	public class CSharpDemo:BaseClass
	{
		override public void run()
		{
			base.run();
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);

			
			CheckFix(context, issues, @"namespace Demo
{
	public class BaseClass
	{
		virtual public void run()
		{
			int a = 1+1;
		}
	}
	public class CSharpDemo:BaseClass
	{
	}
}");
		}

		[Test]
		public void TestResharperDisable()
		{
			var input = @"namespace Demo
{
	public class BaseClass
	{
		virtual public void run()
		{
			int a = 1+1;
		}
	}
	//Resharper disable RedundantOverridenMember
	public class CSharpDemo:BaseClass
	{
		override public void run()
		{
			base.run();
		}
	}
	//Resharper restore RedundantOverridenMember
}";
			
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestInspectorCase2()
		{
			var input = @"namespace Demo
{
	public class BaseClass
	{
		virtual public void run()
		{
			int a = 1+1;
		}
	}
	public class CSharpDemo:BaseClass
	{
		override public void run()
		{
			int b = 1+1;
			base.run();
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestTestInspectorCase3()
		{
			var input = 
				@"namespace Demo
{
	public class BaseClass
	{
		virtual public void run()
		{
			int a = 1+1;
		}
	}
	public class CSharpDemo:BaseClass
	{
		public void run1()
		{
			base.run();
		}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestTestInspectorCase4()
		{
			var input = 
				@"namespace Demo
{
	public class BaseClass
	{
		private int a;
		virtual public int A
		{
			get{ return a; }
			set{ a = value; }
		}
	}
	public class CSharpDemo:BaseClass
	{
		public override int A
		{
			get{ return base.A; }
			set{ base.A = value; }
		}
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);

			CheckFix(context, issues,
@"namespace Demo
{
	public class BaseClass
	{
		private int a;
		virtual public int A
		{
			get{ return a; }
			set{ a = value; }
		}
	}
	public class CSharpDemo:BaseClass
	{
	}
}");
		}

		[Test]
		public void TestTestInspectorCase5()
		{
			var input = 
				@"namespace Application
{
	public class SampleCollection<T>
	{ 
		private T[] arr = new T[100];
		public virtual T this[int i]
		{
			get{ return arr[i];}
			set{ arr[i] = value;}
		}
	}

	class Class2<T> : SampleCollection<T>
	{
		public override T this[int i]
		{
			get { return base[i];}
			set { base[i] = value; }
		}
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			
			CheckFix(context, issues,
			@"namespace Application
{
	public class SampleCollection<T>
	{ 
		private T[] arr = new T[100];
		public virtual T this[int i]
		{
			get{ return arr[i];}
			set{ arr[i] = value;}
		}
	}

	class Class2<T> : SampleCollection<T>
	{
	}
}");
		}

		[Test]
		public void TestTestInspectorCase6()
		{
			var input = 
				@"using System;
using System.IO;

partial class A
{
	public virtual int AProperty
	{
		get;
		set;
	}

	public virtual int this[int i]
	{
		get { return i; }
		set { Console.WriteLine(value); }
	}
}

class B : A
{
	public override int AProperty
	{
		set
		{
			base.AProperty = value;
		}
	}
	public override int this[int i]
	{
		get
		{
			return base[i];
		}
	}
	public override string ToString()
	{
		return base.ToString();
	}
}

class C : A
{
	public override int AProperty
	{
		get
		{
			return base.AProperty;
		}
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(4, issues.Count);
			
			CheckFix(context, issues,
			@"using System;
using System.IO;

partial class A
{
	public virtual int AProperty
	{
		get;
		set;
	}

	public virtual int this[int i]
	{
		get { return i; }
		set { Console.WriteLine(value); }
	}
}

class B : A
{
}

class C : A
{
}");
		}

		[Test]
		public void TestRedundantEvent()
		{
			var input = @"namespace Demo
{
	public class BaseClass
	{
		public virtual event EventHandler FooBar { add {} remove {} }
	}
	public class CSharpDemo:BaseClass
	{
		public override event EventHandler FooBar { add { base.FooBar += value; } remove { base.FooBar -= value; } }
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new RedundantOverridenMemberIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);


			CheckFix(context, issues, @"namespace Demo
{
	public class BaseClass
	{
		public virtual event EventHandler FooBar { add {} remove {} }
	}
	public class CSharpDemo:BaseClass
	{
	}
}");
		}


		[Test]
		public void TestNonRedundantEvent()
		{
			var input = @"namespace Demo
{
	public class BaseClass
	{
		public virtual event EventHandler FooBar { add {} remove {} }
		public virtual event EventHandler FooBar2 { add {} remove {} }
	}
	public class CSharpDemo:BaseClass
	{
		public override event EventHandler FooBar { add { base.FooBar += value; } remove { base.FooBar2 -= value; } }
	}
}";
			TestWrongContext<RedundantOverridenMemberIssue>(input);
		}


		[Test]
		public void TestGetHashCode()
		{
			TestWrongContext<RedundantOverridenMemberIssue>(@"
class Bar
{
	public override bool Equals (object obj)
	{
		return false;
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}");
		}


		[Test]
		public void TestRedundantGetHashCode()
		{
			TestIssue<RedundantOverridenMemberIssue>(@"
class Bar
{
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}");
		}


		[Test]
		public void TestPropertyBug()
		{
			TestWrongContext<RedundantOverridenMemberIssue>(@"
class BaseFoo
{
	public virtual int Foo { get; set; }
}

class Bar : BaseFoo
{
	int bar;
	public override int Foo {
		get {
			return base.Foo;
		}
		set {
			base.Foo = bar = value;
		}
	}
}");
		}

	}
}