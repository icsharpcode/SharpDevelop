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
	public class TextTemplatingAssemblyResolverTests
	{
		TextTemplatingAssemblyResolver resolver;
		IProject project;
		FakeAssemblyParserService fakeAssemblyParserService;
		
		void CreateResolver()
		{
			project = ProjectHelper.CreateProject();
			fakeAssemblyParserService = new FakeAssemblyParserService();
			resolver = new TextTemplatingAssemblyResolver(project, fakeAssemblyParserService);
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
		public void Resolve_ProjectHasNoReferences_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
			fakeAssemblyParserService.FakeReflectionProjectContent = null;
			string result = resolver.Resolve("Test");
			
			Assert.AreEqual("Test", result);
		}
		
		[Test]
		public void Resolve_ProjectHasOneReferenceToTestAssemblyWithHintPathSet_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.HintPath = expectedFileName;
			
			string result = resolver.Resolve("Test");
			
			Assert.AreEqual(expectedFileName, result);
		}

		[Test]
		public void Resolve_AssemblyReferenceNameIsInDifferentCase_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.HintPath = expectedFileName;
			
			string result = resolver.Resolve("TEST");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void Resolve_ProjectHasOneReferenceToTestAssemblyWithFileNameSet_ReturnsFullPathToTestAssembly()
		{
			CreateResolver();
			ReferenceProjectItem reference = AddReferenceToProject("Test");
			string expectedFileName = @"d:\projects\MyProject\lib\Test.dll";
			reference.FileName = expectedFileName;
			
			string result = resolver.Resolve("Test");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void Resolve_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReturnsFullPathToAssemblyFoundFromAssemblyParserService()
		{
			CreateResolver();
			string expectedFileName = @"c:\Windows\System32\Gac\System.Data.dll";
			fakeAssemblyParserService.FakeReflectionProjectContent.AssemblyLocation = expectedFileName;
			
			string result = resolver.Resolve("System.Data");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void Resolve_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemPassedToAssemblyParserServiceUsesProject()
		{
			CreateResolver();
			string result = resolver.Resolve("System.Data");
			IProject expectedProject = GetProjectPassedToAssemblyParserService();
			
			Assert.AreEqual(project, expectedProject);
		}
		
		[Test]
		public void Resolve_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemPassedToAssemblyParserServiceIsReference()
		{
			CreateResolver();
			string result = resolver.Resolve("System.Data");
			ItemType type = GetReferenceItemTypePassedToAssemblyParserService();
			
			Assert.AreEqual(ItemType.Reference, type);
		}
		
		[Test]
		public void Resolve_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReferenceItemIncludePassedToAssemblyParserServiceIsAssemblyNameToResolve()
		{
			CreateResolver();
			string result = resolver.Resolve("System.Data");
			string referenceInclude = ReferenceProjectItemPassedToGetReflectionProjectContentForReference.Include;
			
			Assert.AreEqual("System.Data", referenceInclude);
		}
		
		[Test]
		public void Resolve_ProjectHasNoReferencesAndAssemblyReferenceNotFoundInGac_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
			fakeAssemblyParserService.FakeReflectionProjectContent = null;
			
			string result = resolver.Resolve("System.Data");
			
			Assert.AreEqual("System.Data", result);
		}
	}
}
