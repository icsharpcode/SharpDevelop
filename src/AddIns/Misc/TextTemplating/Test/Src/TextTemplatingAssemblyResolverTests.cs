// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingAssemblyResolverTests
	{
		TextTemplatingAssemblyResolver resolver;
		IProject project;
		
		void CreateResolver()
		{
			var info = new ProjectCreateInformation();
			info.Solution = new Solution();
			info.OutputProjectFileName = @"d:\projects\MyProject\MyProject.cs";
			info.ProjectName = "MyProject";
			project = new MSBuildBasedProject(info);
			resolver = new TextTemplatingAssemblyResolver(project);
		}
		
		ReferenceProjectItem AddReferenceToProject(string referenceName)
		{
			ReferenceProjectItem projectItem = new ReferenceProjectItem(project, referenceName);
			ProjectService.AddProjectItem(project, projectItem);
			return projectItem;
		}

		[Test]
		public void Resolve_ProjectHasNoReferences_ReturnsAssemblyReferencePassedToMethod()
		{
			CreateResolver();
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
	}
}
