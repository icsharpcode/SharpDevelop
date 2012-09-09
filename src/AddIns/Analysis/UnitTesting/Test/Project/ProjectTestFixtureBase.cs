// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Base class that helps setting up a unit test project.
	/// </summary>
	public class ProjectTestFixtureBase
	{
		public static readonly IUnresolvedAssembly Corlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		public static readonly IUnresolvedAssembly NUnitFramework = new CecilLoader().LoadAssemblyFile(typeof(TestAttribute).Assembly.Location);
		
		protected IProject project;
		protected TestProject testProject;
		protected IProjectContent projectContent;
		
		public TestProject CreateProject(ITestFramework framework, params IUnresolvedFile[] codeFiles)
		{
			SD.InitializeForUnitTests();
			var parserService = MockRepository.GenerateStrictMock<IParserService>();
			parserService.Stub(p => p.GetCompilation(project)).WhenCalled(m => m.ReturnValue = projectContent.CreateCompilation());
			
			project = MockRepository.GenerateStrictMock<IProject>();
			
			projectContent = new CSharpProjectContent().SetAssemblyName("TestProject");
			projectContent = projectContent.AddAssemblyReferences(Corlib, NUnitFramework);
			projectContent = projectContent.AddOrUpdateFiles(codeFiles);
			
			testProject = new TestProject(project, framework);
			return testProject;
		}
		
		public TestProject CreateNUnitProject(params IUnresolvedFile[] codeFiles)
		{
			return CreateProject(new NUnitTestFramework(), codeFiles);
		}
		
		protected IUnresolvedFile Parse(string code, string fileName = "test.cs")
		{
			return new CSharpParser().Parse(code, fileName).ToTypeSystem();
		}
		
		protected void UpdateCodeFile(string code, string fileName = "test.cs")
		{
			var oldFile = projectContent.GetFile(fileName);
			var newFile = Parse(code, fileName);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			testProject.NotifyParseInformationChanged(oldFile, newFile);
		}
		
		protected void RemoveCodeFile(string fileName)
		{
			var oldFile = projectContent.GetFile(fileName);
			projectContent = projectContent.RemoveFiles(fileName);
			testProject.NotifyParseInformationChanged(oldFile, null);
		}
	}
}
