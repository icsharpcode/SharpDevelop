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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.MSTest;
using NUnit.Framework;

namespace MSTest.SharpDevelop.Tests
{
	[TestFixture]
	public class MSTestProjectTests : MSTestBaseTests
	{
		MSTestProject testProject;
		
		void CreateTestProject()
		{
			testProject = new MSTestProject(project);
		}
		
		List<MSTestMember> GetTestMembersFor()
		{
			return testProject.GetTestMembersFor(GetFirstTypeDefinition()).ToList();
		}
		
		[Test]
		public void GetTestMembersFor_OneMethodHasNoAttributes_ReturnsOneMember()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    public void MyMethod() {}" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual(0, members.Count);
		}
		
		[Test]
		public void GetTestMembersFor_MethodHasTestMethodAttributeWithoutAttributePart_ReturnsOneMember()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    [TestMethod]" +
				"    public void MyMethod() {}" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("MyMethod", members[0].DisplayName);
		}
		
		[Test]
		public void GetTestMembersFor_MethodHasTestMethodAttributeAttribute_ReturnsOneMember()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    [TestMethodAttribute]" +
				"    public void MyMethod() {}" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual("MyMethod", members[0].DisplayName);
		}

		[Test]
		public void GetTestMembersFor_MethodHasFullyQualifiedMSTestTestMethodAttribute_ReturnsOneMember()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute]" +
				"    public void MyMethod() {}" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual("MyMethod", members[0].DisplayName);
		}
		
		[Test]
		public void GetTestMembersFor_MemberNotMethod_ReturnsNoItems()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    public int MyProperty { get; set; }" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual(0, members.Count);
		}
		
		[Test]
		public void GetTestMembersFor_ClassHasNoMethods_ReturnsNoItems()
		{
			AddCodeFile("myclass.cs", "[TestClass]class MyTest {}");
			CreateTestProject();
			
			List<MSTestMember> testMembers = GetTestMembersFor();
			
			Assert.AreEqual(0, testMembers.Count);
		}
	
		[Test]
		public void GetTestMembersFor_ClassHasTwoMethodsAndSecondOneIsTestMethod_ReturnsSecondTestMethodOnly()
		{
			string code = 
				"[TestClass]" +
				"class MyTest {" +
				"    public void MyFirstMethod() {}" +
				"" +
				"    [TestMethod]" +
				"    public void MySecondMethod() {}" +
				"}";
			AddCodeFile("myclass.cs", code);
			CreateTestProject();
			
			List<MSTestMember> members = GetTestMembersFor();
			
			Assert.AreEqual("MySecondMethod", members[0].DisplayName);
		}
	}
}
