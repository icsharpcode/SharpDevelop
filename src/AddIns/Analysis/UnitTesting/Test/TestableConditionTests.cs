// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests
{
	[TestFixture]
	public class TestableConditionTests
	{
		[SetUp]
		public void SetUpFixture()
		{
			ResourceManager.Initialize();
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
			TestableCondition testableCondition = new TestableCondition();
			Assert.IsFalse(testableCondition.IsValid(null, null));
		}
		
		[Test]
		public void GetMemberFromTreeView()
		{
			MockTestTreeView mockTreeView = new MockTestTreeView();
			MockMember mockMember = new MockMember();
			mockTreeView.SelectedMethod = mockMember;
			
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
			MockMethod mockMethod = new MockMethod();
			MockMemberNode memberNode = new MockMemberNode(mockMethod);
			
			Assert.IsTrue(Object.ReferenceEquals(mockMethod, TestableCondition.GetMember(memberNode)));
		}
		
		[Test]
		public void GetProjectFromMemberNode()
		{
			MockMethod mockMethod = new MockMethod();
			MSBuildBasedProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			mockMethod.DeclaringType = mockClass;
			MockMemberNode memberNode = new MockMemberNode(mockMethod);
				
			Assert.IsTrue(Object.ReferenceEquals(project, TestableCondition.GetProject(memberNode)));
		}
		
		[Test]
		public void GetClassFromClassNode()
		{
			MockClass mockClass = new MockClass();
			ClassNode classNode = new ClassNode(null, mockClass);
			
			Assert.IsTrue(Object.ReferenceEquals(mockClass, TestableCondition.GetClass(classNode)));
		}
		
		[Test]
		public void GetProjectFromClassNode()
		{
			MSBuildBasedProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			ClassNode classNode = new ClassNode(project, mockClass);
				
			Assert.IsTrue(Object.ReferenceEquals(project, TestableCondition.GetProject(classNode)));
		}
		
		[Test]
		public void GetMemberFromClassMemberBookmark()
		{
			MockMethod mockMethod = new MockMethod();
			mockMethod.Region = new DomRegion(1,1);
			ClassMemberBookmark bookmark = new ClassMemberBookmark(mockMethod);
			
			Assert.IsTrue(Object.ReferenceEquals(mockMethod, TestableCondition.GetMember(bookmark)));
		}
		
		[Test]
		public void GetClassFromClassBookmark()
		{
			MockClass mockClass = new MockClass();
			mockClass.Region = new DomRegion(1,1);
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
			MockClass mockClass = new MockClass();
			ClassNode classNode = new ClassNode(null, mockClass);
			
			Assert.IsNull(TestableCondition.GetProject(classNode));
		}
		
		/// <summary>
		/// Tests references to class.ProjectContent when 
		/// it is null.
		/// </summary>
		[Test]
		public void IsValidWhenProjectContentIsNull()
		{
			MockClass mockClass = new MockClass();
			ClassNode classNode = new ClassNode(null, mockClass);
			
			TestableCondition testableCondition = new TestableCondition();
			Assert.IsFalse(testableCondition.IsValid(classNode, null));
		}
		
		/// <summary>
		/// When class.ProjectContent.Project == null the 
		/// TestableCondition.IsValid should return false.
		/// </summary>
		[Test]
		public void IsValidWhenProjectIsNull()
		{
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockClass.ProjectContent = mockProjectContent;
			ClassNode classNode = new ClassNode(null, mockClass);
			
			TestableCondition testableCondition = new TestableCondition();
			Assert.IsFalse(testableCondition.IsValid(classNode, null));
		}
		
		/// <summary>
		/// Tests what happens when the class.ProjectContent.Language 
		/// is null.
		/// </summary>
		[Test]
		public void IsValidWhenLanguageIsNull()
		{
			MSBuildBasedProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			ClassNode classNode = new ClassNode(project, mockClass);

			TestableCondition testableCondition = new TestableCondition();
			Assert.IsFalse(testableCondition.IsValid(classNode, null));
		}
		
		/// <summary>
		/// Tests what happens when the 
		/// class.ProjectContent.Language.NameComparer is null.
		/// </summary>
		[Test]
		public void IsValidWhenNameComparerIsNull()
		{
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			mockClass.ProjectContent = mockProjectContent;
			ClassNode classNode = new ClassNode(project, mockClass);

			TestableCondition testableCondition = new TestableCondition();
			Assert.IsFalse(testableCondition.IsValid(classNode, null));
		}
		
		[Test]
		public void IsValidWhenMethodHasTestAttribute()
		{
			MockMethod mockMethod = new MockMethod();
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = LanguageProperties.None;
			mockClass.ProjectContent = mockProjectContent;
			mockMethod.DeclaringType = mockClass;
			MockMemberNode memberNode = new MockMemberNode(mockMethod);

			TestableCondition testableCondition = new TestableCondition();
			Assert.IsTrue(testableCondition.IsValid(memberNode, null));
		}
		
		[Test]
		public void IsValidWhenClassHasTestFixtureAttribute()
		{
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = LanguageProperties.None;
			mockClass.ProjectContent = mockProjectContent;
			ClassNode classNode = new ClassNode(project, mockClass);

			TestableCondition testableCondition = new TestableCondition();
			Assert.IsTrue(testableCondition.IsValid(classNode, null));
		}
	}
}
