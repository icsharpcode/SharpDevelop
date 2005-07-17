using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class InnerClassesResolverTests
	{
		#region Test helper methods
		NRefactoryResolverTests nrrt = new NRefactoryResolverTests();
		
		ResolveResult Resolve(string program, string expression, int line)
		{
			return nrrt.Resolve(program, expression, line);
		}
		
		ResolveResult ResolveVB(string program, string expression, int line)
		{
			return nrrt.ResolveVB(program, expression, line);
		}
		#endregion
		
		[Test]
		public void OuterclassPrivateFieldResolveTest()
		{
			string program = @"class A
{
	int myField;
	class B
	{
		void MyMethod(A a)
		{
		
		}
	}
}
";
			ResolveResult result = Resolve(program, "a", 8);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			ArrayList arr = result.GetCompletionData(nrrt.lastPC);
			Assert.IsNotNull(arr, "arr");
			foreach (object o in arr) {
				if (o is IField) {
					Assert.AreEqual("myField", ((IField)o).Name);
					return;
				}
			}
			Assert.Fail("private field not visible from inner class");
		}
	}
}
