// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestFrameworkIsTestMethodTests
	{
		RubyTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new RubyTestFramework(); 
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
