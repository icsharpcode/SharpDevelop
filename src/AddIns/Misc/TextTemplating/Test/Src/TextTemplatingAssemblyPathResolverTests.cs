// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using Rhino.Mocks;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingAssemblyPathResolverTests
	{
		TextTemplatingAssemblyPathResolver resolver;
		IProject project;
		IGlobalAssemblyCacheService fakeGacService;
		FakeTextTemplatingPathResolver fakePathResolver;
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		void CreateResolver()
		{
			project = ProjectHelper.CreateProject();
			fakeGacService = MockRepository.GenerateStub<IGlobalAssemblyCacheService>();
			fakePathResolver = new FakeTextTemplatingPathResolver();
			resolver = new TextTemplatingAssemblyPathResolver(project, fakeGacService, fakePathResolver);
		}
		
		ReferenceProjectItem AddReferenceToProject(string referenceName)
		{
			ReferenceProjectItem projectItem = new ReferenceProjectItem(project, referenceName);
			ProjectService.AddProjectItem(project, projectItem);
			return projectItem;
		}
		
		void AddFileNameForGacReference(string fileName, string reference)
		{
			DomAssemblyName assemblyName = AddMatchingAssemblyNameForGacReference(reference);
			fakeGacService
				.Stub(gac => gac.FindAssemblyInNetGac(assemblyName))
				.Return(new FileName(fileName));
		}
		
		DomAssemblyName AddMatchingAssemblyNameForGacReference(string reference)
		{
			var assemblyName = new DomAssemblyName(reference);
			var assemblyNameToReturn = new DomAssemblyName("GacReference");
			fakeGacService
				.Stub(gac => gac.FindBestMatchingAssemblyName(assemblyName))
				.Return(assemblyNameToReturn);
			
			return assemblyNameToReturn;
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferences_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
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
			reference.FileName = FileName.Create(expectedFileName);
			
			string result = resolver.ResolvePath("Test");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceInGac_ReturnsFullPathToAssemblyFoundFromAssemblyParserService()
		{
			CreateResolver();
			string expectedFileName = @"c:\Windows\System32\Gac\System.Data.dll";
			AddFileNameForGacReference(expectedFileName, "System.Data");
			
			string result = resolver.ResolvePath("System.Data");
			
			Assert.AreEqual(expectedFileName, result);
		}
		
		[Test]
		public void ResolvePath_ProjectHasNoReferencesAndAssemblyReferenceNotFoundInGac_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
			
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
			
			fakeGacService.AssertWasNotCalled(gac => gac.FindBestMatchingAssemblyName(Arg<DomAssemblyName>.Is.Anything));
		}
		
		[Test]
		public void ResolvePath_AssemblyReferenceInGacButNoFileNameFound_ReturnsAssemblyReferenceNamePassedToResolvePath()
		{
			CreateResolver();
			AddMatchingAssemblyNameForGacReference("System.Data");
			
			string result = resolver.ResolvePath("System.Data");
			
			Assert.AreEqual("System.Data", result);
		}
	}
}
