using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;

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
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		string x = this;
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues (new CS0029InvalidConversionIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			Assert.IsFalse(issues[0].Actions.Any());
		}
	}
}