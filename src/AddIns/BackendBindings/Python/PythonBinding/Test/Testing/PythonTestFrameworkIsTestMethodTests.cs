// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestFrameworkIsTestMethodTests
	{
		PythonTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new PythonTestFramework(); 
		}
		
		[Test]
		public void IsTestMethodReturnsTrueForMethodThatStartsWithTest()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "testRunThis");
			Assert.IsTrue(testFramework.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseForNull()
		{
			Assert.IsFalse(testFramework.IsTestMethod(null));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseForMethodThatDoesNotStartWithTest()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "RunThis");
			Assert.IsFalse(testFramework.IsTestMethod(method));
		}
	}
}
