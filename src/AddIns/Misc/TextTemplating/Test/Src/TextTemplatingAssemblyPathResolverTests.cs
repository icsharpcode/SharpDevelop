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
