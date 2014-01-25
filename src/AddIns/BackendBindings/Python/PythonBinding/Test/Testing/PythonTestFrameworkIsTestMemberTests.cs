// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestFrameworkIsTestMemberTests
	{
		PythonTestFramework testFramework;
		
		void CreateTestFramework()
		{
			testFramework = new PythonTestFramework(); 
		}
		
		[Test]
		public void IsTestMember_MethodThatStartsWithTest_ReturnsTrue()
		{
			CreateTestFramework();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "testRunThis");
			
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
			MockMethod method = new MockMethod(c, "RunThis");
			
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
