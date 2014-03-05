using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using System;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0029InvalidConversionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestConversion()
		{
			var input = @"
class TestClass
{
enum Enum{ };
	void TestMethod (Enum i)
	{
		int x;
		x = i;
	}
}";
			var output = @"
class TestClass
{
enum Enum{ };
	void TestMethod (Enum i)
	{
		int x;
		x = (int)i;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}
		
		[Test]
		public void TestConversionInInitializer()
		{
			var input = @"
class TestClass
{
enum Enum{ };
	void TestMethod (Enum i)
	{
		int x = i;
	}
}";
			var output = @"
class TestClass
{
enum Enum{ };
	void TestMethod (Enum i)
	{
		int x = (int)i;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}
		
		[Test]
		public void TestClassConversion()
		{
			var input = @"
class Base {}
class Test: Base {}
class TestClass
{
	void TestMethod (Test i)
	{
		Base x;
		x = i;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, 0);
		}

		[Test]
		public void TestConversionDoubleFloat()
		{
			var input = @"
class Foo
{
	void Bar () {
		double c = 3.5;
		float fc;
		fc = c;
	}
}";
			var output = @"
class Foo
{
	void Bar () {
		double c = 3.5;
		float fc;
		fc = (float)c;
	}
}";

			Test<CS0029InvalidConversionIssue>(input, output);
		}

		[Test]
		public void TestConversionEnumToInt()
		{
			var input = @"
class Foo
{
	enum Enum { Zero }
	void Bar () {
		var e = Enum.Zero;
		int val;
		val = e;
	}
}";
			var output = @"
class Foo
{
	enum Enum { Zero }
	void Bar () {
		var e = Enum.Zero;
		int val;
		val = (int)e;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}

		[Test]
		public void TestConversionSameType()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int x =0;
		int y = 1;
		$x = i;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, 0);
		}
		
		[Test]
		public void TestUserDefinedAddition()
		{
			var input = @"
struct Vector {
  public static explicit operator Point(Vector v) { return new Point(); }
}
struct Point {
  public static Point operator +(Point p, Vector v) { return new Point(); }
}
class TestClass
{
	void TestMethod ()
	{
		Point p = new Point();
		$p += new Vector();
	}
}";
			Test<CS0029InvalidConversionIssue>(input, 0);
		}

		[Test]
		public void TestImplicitOperator()
		{
			var input = @"
struct Vector {
  public static implicit operator Point(Vector v) { return new Point(); }
}

struct Point {
	
}

class TestClass
{
	void TestMethod ()
	{
		Point p;
		p = new Vector ();
	}
}";
			Test<CS0029InvalidConversionIssue>(input, 0);
		}
		
		[Test]
		public void TestAssignZeroToEnum()
		{
			var input = @"using System;
class TestClass
{
	void TestMethod ()
	{
		StringComparison c = 0;
		c = 0;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, 0);
		}
		
		[Test]
		public void AssignCustomClassToString()
		{
			Test<CS0029InvalidConversionIssue>(@"
class TestClass
{
	void TestMethod ()
	{
		string x = this;
	}
}", @"
class TestClass
{
	void TestMethod ()
	{
		var x = this;
	}
}");
		}

		/// <summary>
		/// Bug 12490 - Cast warnings with literals 
		/// </summary>
		[Test]
		public void TestBug12490()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		uint t;
		t = 6;
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new CS0029InvalidConversionIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void TestReturnInMethod()
		{
			var input = @"
class TestClass
{
	enum Enum{ };
	int TestMethod (Enum i)
	{
		return i;
	}
}";
			var output = @"
class TestClass
{
	enum Enum{ };
	int TestMethod (Enum i)
	{
		return (int)i;
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}

		[Test]
		public void TestReturnInMethodChangeReturnType()
		{
			var input = @"
class TestClass
{
	int TestMethod ()
	{
		return ""foo"";
	}
}";
			var output = @"
class TestClass
{
	string TestMethod ()
	{
		return ""foo"";
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}

		
		[Test]
		public void TestReturnInAnonymousMethod()
		{
			var input = @"using System;

class TestClass
{
	enum Enum{ };
	void TestMethod (Enum i)
	{
		Func<int> foo = delegate {
			return i;
		};
	}
}";
			var output = @"using System;

class TestClass
{
	enum Enum{ };
	void TestMethod (Enum i)
	{
		Func<int> foo = delegate {
			return (int)i;
		};
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}


		[Test]
		public void TestReturnInProperty()
		{
			var input = @"
class TestClass
{
	enum Enum{ A };
	int Test {
		get {
			return Enum.A;
		}
	}
}";
			var output = @"
class TestClass
{
	enum Enum{ A };
	int Test {
		get {
			return (int)Enum.A;
		}
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}

		[Test]
		public void TestCall()
		{
			var input = @"
class TestClass
{
	enum Enum{ };
	void Foo(string s, int i) {}
	void TestMethod (Enum i)
	{
		Foo (""Bar"", i);
	}
}";
			var output = @"
class TestClass
{
	enum Enum{ };
	void Foo(string s, int i) {}
	void TestMethod (Enum i)
	{
		Foo (""Bar"", (int)i);
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}


		[Test]
		public void TestCallWithOverloads()
		{
			var input = @"
class TestClass
{
	enum Enum{ };
	void Foo(string s, object o) {}
	void Foo(string s, int i) {}
	void TestMethod (Enum i)
	{
		Foo (""Bar"", i);
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new CS0029InvalidConversionIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestCallWithOverloads2()
		{
			var input = @"
class TestClass
{
	enum Enum{ };
	void Foo(string s, string o) {}
	void Foo(string s, int i) {}
	void TestMethod (Enum i)
	{
		Foo (""Bar"", i);
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new CS0029InvalidConversionIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}

		[Test]
		public void TestArrayInitializer()
		{
			var input = @"
class TestClass
{
	enum Enum{ A }
	public static void Main (string[] args)
	{
		System.Console.WriteLine (new int[] { Enum.A });
	}
}";
			var output = @"
class TestClass
{
	enum Enum{ A }
	public static void Main (string[] args)
	{
		System.Console.WriteLine (new int[] { (int)Enum.A });
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}
		
		[Test]
		public void ExplicitConversionFromUnknownType()
		{
			string input = @"
class Test {
	void M(MissingInterface m) {
		this.Project = (Project)m;
	}
	public Project Project { get; set; }
}
class Project : MissingInterface {}";
			TestWrongContext<CS0029InvalidConversionIssue>(input);
		}

		[Test]
		public void TestFixedConversion()
		{
			var input = @"unsafe struct TestMe
{
	fixed int textureID[8], fooBar[12];

	public void Randomize ()
	{
		fixed (int* buf = textureID) {
			buf [0] = 1;
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new CS0029InvalidConversionIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestBinaryOperator()
		{
			var input = @"
class TestClass
{
enum Enum{ };
	void TestMethod (ulong i)
	{
		int x;
		x = i + i;
	}
}";
			var output = @"
class TestClass
{
enum Enum{ };
	void TestMethod (ulong i)
	{
		int x;
		x = (int)(i + i);
	}
}";
			Test<CS0029InvalidConversionIssue>(input, output);
		}
	
		[Test]
		public void TestDeclarationFix ()
		{
			Test<CS0029InvalidConversionIssue>(@"
using System.Collections.Generic;
class TestClass
{
	string[] str = new List<string> ();
}", @"
using System.Collections.Generic;
class TestClass
{
	List<string> str = new List<string> ();
}");
		}

		[Test]
		public void TestLocalDeclarationFix ()
		{
			Test<CS0029InvalidConversionIssue>(@"
using System.Collections.Generic;
class TestClass
{
	void Foo ()
	{
		string[] str = new List<string> ();
	}
}", @"
using System.Collections.Generic;
class TestClass
{
	void Foo ()
	{
		var str = new List<string> ();
	}
}");
		}

		[Test]
		public void LinqQueryBug()
		{
			var input = @"
using System;

public static class FooExt
{
	public static May<TOut> Select<TIn, TOut>(this May<TIn> value, Func<TIn, TOut> projection)
	{
		return new May<TOut> ();
	}
}

public class May<T>
{
	public T Property { get; set; }
}

public class EmptyClass
{
	public May<EmptyClass> Foo ()
	{
		return 
			from foo in new May<EmptyClass> ()
			select foo;
	}
}
";
			TestWrongContext<CS0029InvalidConversionIssue> (input);
		}
	}
}