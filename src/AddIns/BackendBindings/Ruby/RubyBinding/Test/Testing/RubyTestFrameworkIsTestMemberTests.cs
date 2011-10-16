// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestFrameworkIsTestMemberTests
	{
		RubyTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new RubyTestFramework(); 
		}
		
		[Test]
		public void IsTestMemberReturnsTrueForMethodThatStartsWithTest()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "testRunThis");
			Assert.IsTrue(testFramework.IsTestMember(method));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseForNull()
		{
			Assert.IsFalse(testFramework.IsTestMember(null));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseForMethodThatDoesNotStartWithTest()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "RunThis");
			Assert.IsFalse(testFramework.IsTestMember(method));
		}
	}
}
