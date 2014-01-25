// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
