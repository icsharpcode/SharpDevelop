using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantWhereWithPredicateIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestWhereAnyCase1 ()
		{
			var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Where (x => x < 4).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new RedundantWhereWithPredicateIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Any (x => x < 4);
	}
}");
		}
		
		[Test]
		public void TestWhereAnyWrongWhere1 ()
		{
			var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Where ((x, i) => x + i < 4).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new RedundantWhereWithPredicateIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestWhereAnyWrongWhere2 ()
		{
			var input = @"using System;
using System.Linq;
public class X
{
	X Where (Func<int,int> f) { return null; }
	bool Any () { return false; }
	public void Bla () {
		X ex = null;
		var bla = ex.Where (x => x + 1).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new RedundantWhereWithPredicateIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestWhereCount()
		{
			var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Where (x => x < 4).Count ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new RedundantWhereWithPredicateIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Count (x => x < 4);
	}
}");
		}
	}
}
