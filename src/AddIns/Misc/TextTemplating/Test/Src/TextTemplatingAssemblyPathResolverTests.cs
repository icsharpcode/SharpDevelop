// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingAssemblyPathResolverTests
	{
		TextTemplatingAssemblyPathResolver resolver;
		IProject project;
		FakeAssemblyParserService fakeAssemblyParserService;
		FakeTextTemplatingPathResolver fakePathResolver;
		
		void CreateResolver()
		{
			project = ProjectHelper.CreateProject();
			fakeAssemblyParserService = new FakeAssemblyParserService();
			fakePathResolver = new FakeTextTemplatingPathResolver();
			resolver = new TextTemplatingAssemblyPathResolver(project, fakeAssemblyParserService, fakePathResolver);
		}
		
		ReferenceProjectItem AddReferenceToProject(string referenceName)
		{
			ReferenceProjectItem projectItem = new ReferenceProjectItem(project, referenceName);
			ProjectService.AddProjectItem(project, projectItem);
			return projectItem;
		}

		IProject GetProjectPassedToAssemblyParserService()
		{
			return ReferenceProjectItemPassedToGetReflectionProjectContentForReference.Project;
		}
		
		ReferenceProjectItem ReferenceProjectItemPassedToGetReflectionProjectContentForReference {
			get { return fakeAssemblyParserService.ItemPassedToGetReflectionProjectContentForReference; }
		}
		
		ItemType GetReferenceItemTypePassedToAssemblyParserService()
		{
			return ReferenceProjectItemPassedToGetReflectionProjectContentForReference.ItemType;
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferences_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
			fakeAssemblyParserService.FakeReflectionProjectContent = null;
			string result = resolver.ResolvePath("Test");
			
			Assert.AreEqual("Test", result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasOneReferenceToTestAssemblyWithHintPathSet_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.HintPath = expectedFileName;
			
			string result = resolver.ResolvePath("Test");
			
			Assert.AreEqual(expectedFileName, result);
		}

		[Test]
		public void ResolvePath_AssemblyReferenceNameIsInDifferentCase_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.HintPath = expectedFileName;
			
			string result = resolver.ResolvePath("TEST");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasOneReferenceToTestAssemblyWithFileNameSet_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("Test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.FileName = expectedFileName;
			
			string result = resolver.ResolvePath("Test");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReturnsFullPathToAssemblyFoundFromAssemblyParserService()
		{
			CreateResolver();
			string expectedFileName = @"c:\Windows\System32\Gac\System.Data.dll";
			fakeAssemblyParserService.FakeReflectionProjectContent.AssemblyLocation = expectedFileName;
			
			string result = resolver.ResolvePath("System.Data");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemPassedToAssemblyParserServiceUsesProject()
		{
			CreateResolver();
			string result = resolver.ResolvePath("System.Data");
			IProject expectedProject = GetProjectPassedToAssemblyParserService();
			
			Assert.AreEqual(project, expectedProject);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemPassedToAssemblyParserServiceIsReference()
		{
			CreateResolver();
			string result = resolver.ResolvePath("System.Data");
			ItemType type = GetReferenceItemTypePassedToAssemblyParserService();
			
			Assert.AreEqual(ItemType.Reference, type);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemIncludePassedToAssemblyParserServiceIsAssemblyNameToResolvePath()
		{
			CreateResolver();
			string result = resolver.ResolvePath("System.Data");
			string referenceInclude = ReferenceProjectItemPassedToGetReflectionProjectContentForReference.Include;
			
			Assert.AreEqual("System.Data", referenceInclude);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceNotFoundInGac_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
			fakeAssemblyParserService.FakeReflectionProjectContent = null;
			
			string result = resolver.ResolvePath("System.Data");
			
			Assert.AreEqual("System.Data", result);
		}
		
		[Test]
		public void ResolvePath_AssemblyReferenceHasTemplateVariable_ReturnsExpandedAssemblyReferenceFileName()
		{
			CreateResolver();
			string path = @"$(SolutionDir)lib\Test.dll";
			string expectedPath = @"d:\projects\MyProject\lib\Test.dll";
			fakePathResolver.AddPath(path, expectedPath);
			
			string resolvedPath = resolver.ResolvePath(path);
			
			Assert.AreEqual(expectedPath, resolvedPath);
		}
		
		[Test]
		public void ResolvePath_AssemblyReferenceHasTemplateVariable_AssemblyParserServiceIsNotUsed()
		{
			CreateResolver();
			string path = @"$(SolutionDir)lib\Test.dll";
			string expectedPath = @"d:\projects\MyProject\lib\Test.dll";
			fakePathResolver.AddPath(path, expectedPath);
			
			string result = resolver.ResolvePath(path);
			
			Assert.IsFalse(fakeAssemblyParserService.IsGetReflectionProjectContentForReferenceCalled);
		}
	}
}
