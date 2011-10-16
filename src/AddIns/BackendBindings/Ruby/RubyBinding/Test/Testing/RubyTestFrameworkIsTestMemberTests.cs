// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestFrameworkIsTestMemberTests
	{
		RubyTestFramework testFramework;
		
		void CreateTestFramework()
		{
			testFramework = new RubyTestFramework(); 
		}
		
		[Test]
		public void IsTestMember_MethodThatStartsWithTest_ReturnsTrue()
		{
			CreateTestFramework();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			var method = new MockMethod(c, "testRunThis");
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_NullPassed_ReturnsFalse()
		{
			CreateTestFramework();
			bool result = testFramework.IsTestMember(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodThatDoesNotStartWithTest_ReturnsFalse()
		{
			CreateTestFramework();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			var method = new MockMethod(c, "RunThis");
			
			bool result = testFramework.IsTestMember(method);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_FieldThatStartsWithTest_ReturnsFalse()
		{
			CreateTestFramework();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			var field = new DefaultField(c, "testField");
			
			bool result = testFramework.IsTestMember(field);
			
			Assert.IsFalse(result);
		}
	}
}
