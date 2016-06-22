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

using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture, Ignore("Class Browser")]
	public class TestableConditionTests
	{
		/*(
		TestableCondition testableCondition;
		MockRegisteredTestFrameworks testFrameworks;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager.Initialize();
		}
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockRegisteredTestFrameworks();
			testableCondition = new TestableCondition(testFrameworks);
		}
		
		[Test]
		public void GetMemberFromNullOwner()
		{
			Assert.IsNull(TestableCondition.GetMember(null));
		}
		
		[Test]
		public void GetClassFromNullOwner()
		{
			Assert.IsNull(TestableCondition.GetClass(null));
		}
		
		[Test]
		public void GetProjectFromNullOwner()
		{
			Assert.IsNull(TestableCondition.GetProject(null));
		}
		
		[Test]
		public void GetNamespaceFromNullOwner()
		{
			Assert.IsNull(TestableCondition.GetNamespace(null));
		}
		
		[Test]
		public void IsValidFromNullOwner()
		{
			Assert.IsFalse(testableCondition.IsValid(null, null));
		}
		
		[Test]
		public void GetMemberFromTreeView()
		{
			MockTestTreeView mockTreeView = new MockTestTreeView();
			MockMember mockMember = new MockMember();
			mockTreeView.SelectedMember = mockMember;
			
			Assert.IsTrue(Object.ReferenceEquals(mockMember, TestableCondition.GetMember(mockTreeView)));
		}
		
		[Test]
		public void GetClassFromTreeView()
		{
			MockTestTreeView mockTreeView = new MockTestTreeView();
			MockClass mockClass = new MockClass();
			mockTreeView.SelectedClass = mockClass;
			
			Assert.IsTrue(Object.ReferenceEquals(mockClass, TestableCondition.GetClass(mockTreeView)));
		}
		
		[Test]
		public void GetProjectFromTreeView()
		{
			MockTestTreeView mockTreeView = new MockTestTreeView();
			MSBuildBasedProject project = new MockCSharpProject();
			mockTreeView.SelectedProject = project;
			
			Assert.IsTrue(Object.ReferenceEquals(project, TestableCondition.GetProject(mockTreeView)));
		}
		
		[Test]
		public void GetNamespaceFromTreeView()
		{
			MockTestTreeView mockTreeView = new MockTestTreeView();
			MSBuildBasedProject project = new MockCSharpProject();
			mockTreeView.SelectedProject = project;
			mockTreeView.SelectedNamespace = "MyProject.Tests";
			
			Assert.AreEqual("MyProject.Tests", TestableCondition.GetNamespace(mockTreeView));
		}
		
		[Test]
		public void GetMemberFromMemberNode()
		{
			MockMethod mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			MockMemberNode memberNode = new MockMemberNode(mockMethod);
			
			Assert.IsTrue(Object.ReferenceEquals(mockMethod, TestableCondition.GetMember(memberNode)));
		}
		
		[Test]
		public void GetProjectFromMemberNode()
		{
			MSBuildBasedProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
			MockMethod mockMethod = new MockMethod(mockClass);
			MockMemberNode memberNode = new MockMemberNode(mockMethod);
			
			Assert.IsTrue(Object.ReferenceEquals(project, TestableCondition.GetProject(memberNode)));
		}
		
		[Test]
		public void GetClassFromClassNode()
		{
			MockClass mockClass = new MockClass(new MockProjectContent());
			ClassNode classNode = new ClassNode(null, mockClass);
			
			Assert.IsTrue(Object.ReferenceEquals(mockClass, TestableCondition.GetClass(classNode)));
		}
		
		[Test]
		public void GetProjectFromClassNode()
		{
			MSBuildBasedProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
			ClassNode classNode = new ClassNode(project, mockClass);
			
			Assert.IsTrue(Object.ReferenceEquals(project, TestableCondition.GetProject(classNode)));
		}
		
		[Test]
		public void GetMemberFromClassMemberBookmark()
		{
			MockMethod mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			mockMethod.Region = new DomRegion(1, 1);
			ClassMemberBookmark bookmark = new ClassMemberBookmark(mockMethod);
			
			Assert.IsTrue(Object.ReferenceEquals(mockMethod, TestableCondition.GetMember(bookmark)));
		}
		
		[Test]
		public void GetClassFromClassBookmark()
		{
			MockClass mockClass = new MockClass();
			mockClass.Region = new DomRegion(1, 1);
			ClassBookmark bookmark = new ClassBookmark(mockClass);
			
			Assert.IsTrue(Object.ReferenceEquals(mockClass, TestableCondition.GetClass(bookmark)));
		}
		
		/// <summary>
		/// Tests references to class.ProjectContent when 
		/// it is null.
		/// </summary>
		[Test]
		public void GetProjectWhenProjectContentIsNull()
		{
			MockClass mockClass = new MockClass(new MockProjectContent());
			ClassNode classNode = new ClassNode(null, mockClass);
			
			Assert.IsNull(TestableCondition.GetProject(classNode));
		}*/
	}
}
