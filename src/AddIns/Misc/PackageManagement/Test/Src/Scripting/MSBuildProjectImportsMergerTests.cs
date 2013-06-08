// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class MSBuildProjectImportsMergerTests
	{
		MSBuildProjectImportsMerger importsMerger;
		IPackageManagementProjectService projectService;
		TestableProject sharpDevelopProject;
		Project msbuildProject;
		
		[SetUp]
		public void Init()
		{
			CreateMSBuildProject();
			CreateSharpDevelopProject();
			CreateProjectImportsMerger();
		}
		
		void CreateMSBuildProject()
		{
			msbuildProject = new Project();
		}
		
		void CreateSharpDevelopProject()
		{
			sharpDevelopProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateProjectImportsMerger()
		{
			projectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			importsMerger = new MSBuildProjectImportsMerger(msbuildProject, sharpDevelopProject, projectService);
		}
		
		void Merge()
		{
			importsMerger.Merge();
		}
		
		void AddImportToSharpDevelopProject(string project)
		{
			sharpDevelopProject.MSBuildProjectFile.AddImport(project);
		}
		
		void AddImportToMSBuildProject(string project)
		{
			msbuildProject.Xml.AddImport(project);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNoImportsAndSharpDevelopProjectHasOneImport_ImportRemovedFromSharpDevelopProject()
		{
			AddImportToSharpDevelopProject("MyImport.targets");
			
			Merge();
			
			Assert.AreEqual(0, sharpDevelopProject.MSBuildProjectFile.Imports.Count);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNoImportsAndSharpDevelopProjectHasOneImport_SharpDevelopProjectIsSaved()
		{
			AddImportToSharpDevelopProject("MyImport.targets");
			
			Merge();
			
			projectService.AssertWasCalled(service => service.Save(sharpDevelopProject));
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNoImportsAndSharpDevelopProjectHasOneImport_ImportRemovedFromSharpDevelopProjectIncludedInMergeResult()
		{
			AddImportToSharpDevelopProject("MyImport.targets");
			
			Merge();
			
			var expectedImports = new string[] { "MyImport.targets" };
			CollectionAssert.AreEqual(expectedImports, importsMerger.Result.ProjectImportsRemoved);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNoImportsAndSharpDevelopProjectHasTwoImport_ImportsRemovedFromSharpDevelopProjectIncludedInMergeResult()
		{
			AddImportToSharpDevelopProject("MyImport.targets");
			AddImportToSharpDevelopProject("AnotherImport.targets");
			
			Merge();
			
			var expectedImports = new string[] { "MyImport.targets", "AnotherImport.targets" };
			CollectionAssert.AreEqual(expectedImports, importsMerger.Result.ProjectImportsRemoved);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNoImportsAndSharpDevelopProjectHasNoImports_SharpDevelopProjectIsNotSaved()
		{
			Merge();
			
			projectService.AssertWasNotCalled(service => service.Save(sharpDevelopProject));
		}
		
		[Test]
		public void Merge_MSBuildProjectHasOneImportAndSharpDevelopProjectHasTwoImportsOneWhichIsDifferent_DifferentImportRemovedFromSharpDevelopProject()
		{
			AddImportToMSBuildProject("MyImport.targets");
			AddImportToSharpDevelopProject("MyImport.targets");
			AddImportToSharpDevelopProject("Different.targets");
			
			Merge();
			
			ProjectImportElement import = sharpDevelopProject.MSBuildProjectFile.Imports.FirstOrDefault();
			Assert.AreEqual("MyImport.targets", import.Project);
			Assert.AreEqual(1, sharpDevelopProject.MSBuildProjectFile.Imports.Count);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasOneImportAndSharpDevelopProjectHasNoImports_MissingImportAddedToSharpDevelopProject()
		{
			AddImportToMSBuildProject("MyImport.targets");
			
			Merge();
			
			ProjectImportElement import = sharpDevelopProject.MSBuildProjectFile.Imports.FirstOrDefault();
			Assert.AreEqual("MyImport.targets", import.Project);
			Assert.AreEqual(1, sharpDevelopProject.MSBuildProjectFile.Imports.Count);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasOneImportAndSharpDevelopProjectHasNoImports_MissingImportIncludedInMergeResult()
		{
			AddImportToMSBuildProject("MyImport.targets");
			
			Merge();
			
			var expectedImports = new string[] { "MyImport.targets" };
			CollectionAssert.AreEqual(expectedImports, importsMerger.Result.ProjectImportsAdded);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasOneImportAndSharpDevelopProjectHasNoImports_SharpDevelopProjectIsSaved()
		{
			AddImportToMSBuildProject("MyImport.targets");
			
			Merge();
			
			projectService.AssertWasCalled(service => service.Save(sharpDevelopProject));
		}
		
		[Test]
		public void Merge_MSBuildProjectHasTwoImportsAndSharpDevelopProjectHasOneImportWhichMatchesOneInMSBuildProject_DifferentImportAddedToSharpDevelopProject()
		{
			AddImportToMSBuildProject("MyImport.targets");
			AddImportToMSBuildProject("Different.targets");
			AddImportToSharpDevelopProject("MyImport.targets");
			
			Merge();
			
			ProjectImportElement import = sharpDevelopProject.MSBuildProjectFile.Imports.LastOrDefault();
			Assert.AreEqual("Different.targets", import.Project);
			Assert.AreEqual(2, sharpDevelopProject.MSBuildProjectFile.Imports.Count);
		}
	}
}
