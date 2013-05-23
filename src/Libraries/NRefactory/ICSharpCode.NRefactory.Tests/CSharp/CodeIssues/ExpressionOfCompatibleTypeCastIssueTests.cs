using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ExpressionOfCompatibleTypeCastIssueTests : InspectionActionTestBase
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
			Test<ExpressionOfCompatibleTypeCastIssue>(input, output);
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
			Test<ExpressionOfCompatibleTypeCastIssue>(input, 0);
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

			Test<ExpressionOfCompatibleTypeCastIssue>(input, output);
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
			Test<ExpressionOfCompatibleTypeCastIssue>(input, output);
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
			Test<ExpressionOfCompatibleTypeCastIssue>(input, 0);
		}
	}
}