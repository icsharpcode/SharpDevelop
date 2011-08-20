// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcProjectTests
	{
		MvcProject project;
		TestableProject testableProject;
		FakeMvcModelClassLocator fakeModelClassLocator;
		
		void CreateProject()
		{
			testableProject = TestableProject.CreateProject();
			fakeModelClassLocator = new FakeMvcModelClassLocator();
			project = new MvcProject(testableProject, fakeModelClassLocator);
		}
		
		[Test]
		public void Project_ProjectPassedToConstructor_ReturnsProjectPassedToConstructor()
		{
			CreateProject();
			
			IProject msbuildProject = project.Project;
			
			Assert.AreEqual(testableProject, msbuildProject);
		}
		
		[Test]
		public void RootNamespace_ProjectPassedToConstructorHasRootNamespace_ReturnsRootNamespace()
		{
			CreateProject();
			testableProject.RootNamespace = "MyProjectNamespace";
			
			string rootNamespace = project.RootNamespace;
			
			Assert.AreEqual("MyProjectNamespace", rootNamespace);
		}
		
		[Test]
		public void Save_ProjectPassedToConstructor_ProjectIsSaved()
		{
			CreateProject();
			project.Save();
			
			bool saved = testableProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsVisualBasicProject_ReturnsVisualBasicTemplateLanguage()
		{
			CreateProject();
			testableProject.SetLanguage("VBNet");
			
			MvcTextTemplateLanguage language = project.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, language);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsCSharpProject_ReturnsCSharpTemplateLanguage()
		{
			CreateProject();
			testableProject.SetLanguage("C#");
			
			MvcTextTemplateLanguage language = project.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, language);
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ReturnsOneModelClass()
		{
			CreateProject();
			fakeModelClassLocator.AddModelClass("MyNamespace.MyClass");
			
			string[] modelClasses = project
				.GetModelClasses()
				.Select(m => m.FullName)
				.ToArray();
			
			string[] expectedModelClasses = new string[] {
				"MyNamespace.MyClass"
			};
			
			Assert.AreEqual(expectedModelClasses, modelClasses);
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ProjectUsedToFindModelClasses()
		{
			CreateProject();
			fakeModelClassLocator.AddModelClass("MyNamespace.MyClass");
			project.GetModelClasses();
			
			Assert.AreEqual(project, fakeModelClassLocator.ProjectPassedToGetModelClasses);
		}
	}
}
