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
        x = (Enum)i;
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
            var output = @"
class Base {}
class Test: Base {}
class TestClass
{
	void TestMethod (Test i)
	{
		Base x;
        x = (Test)i;
	}
}";
			Test<ExpressionOfCompatibleTypeCastIssue>(input, output);
        }

    }
}