using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	class InvertIfAndSimplifyTests : ContextActionTestBase
	{
		[Test]
		public void Test()
		{
			Test<InvertIfAndSimplify>(
				@"class TestClass
{
	void Test ()
	{
		$if (true) {
			Case1 ();
			Case2 ();
		}
        else 
        {
            return;
        }
	}
}",
				@"class TestClass
{
	void Test ()
	{
		if (false)
			return;
		Case1 ();
		Case2 ();
	}
}"
				);
		}

		[Test]
		public void TestNonVoidMoreComplexMethod()
		{
			Test<InvertIfAndSimplify>(
				@"class TestClass
{
	int Test ()
	{
		$if (true) {
			Case1 ();
		}
		else 
		{
			return 0;
			testDummyCode ();
		}
	}
}",
				@"class TestClass
{
	int Test ()
	{
		if (false) {
			return 0;
			testDummyCode ();
		}
		Case1 ();
	}
}"
				);

		}

		[Test]
		public void TestComplexMethod()
		{
			Test<InvertIfAndSimplify>(
				@"class TestClass
{
	int Test ()
	{
		$if (true) {
			Case1 ();
		}
        else 
            continue;
        return 0;
	}
}",
				@"class TestClass
{
	int Test ()
	{
		if (false)
			continue;
		Case1 ();
		return 0;
	}
}"
				);
		}

		[Test]
		public void TestComment()
		{
			Test<InvertIfAndSimplify>(
				@"class TestClass
{
	int Test ()
	{
		$if (true) {
			Case1 ();
		}
		else 
		{
			//TestComment
			return 0;
		}
	}
}",
				@"class TestClass
{
	int Test ()
	{
		if (false) {
			//TestComment
			return 0;
		}
		Case1 ();
	}
}"
				);

		}
	}
}
