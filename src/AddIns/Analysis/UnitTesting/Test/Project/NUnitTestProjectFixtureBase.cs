// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
	public class NUnitTestProjectFixtureBase
	{
		protected IProject project;
		protected NUnitTestProject testProject;
		protected IProjectContent projectContent;
		
		[SetUp]
		public virtual void SetUp()
		{
			SD.InitializeForUnitTests();
			project = MockRepository.GenerateStrictMock<IProject>();
			project.Stub(p => p.RootNamespace).Return("RootNamespace");
			testProject = new NUnitTestProject(project);
			projectContent = new CSharpProjectContent().AddAssemblyReferences(NRefactoryHelper.Corlib, NRefactoryHelper.NUnitFramework);
			
			SD.Services.AddStrictMockService<IParserService>();
			SD.ParserService.Stub(p => p.GetCompilation(project)).WhenCalled(m => m.ReturnValue = projectContent.CreateCompilation());
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		protected List<string> GetTestMethodNames(NUnitTestClass testClass)
		{
			return testClass.NestedTests.OfType<NUnitTestMethod>().Select(m => m.MethodNameWithDeclaringTypeForInheritedTests).ToList();
		}
		
		protected void AddCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNull(oldFile);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			testProject.NotifyParseInformationChanged(oldFile, newFile);
		}
		
		IUnresolvedFile Parse(string fileName, string code)
		{
			var parser = new CSharpParser();
			var syntaxTree = parser.Parse(code, fileName);
			Assert.IsFalse(parser.HasErrors);
			return syntaxTree.ToTypeSystem();
		}
		
		protected void AddCodeFileInNamespace(string fileName, string code)
		{
			AddCodeFile(fileName, "using NUnit.Framework; namespace RootNamespace { " + code + " }");
		}
		
		protected void UpdateCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNotNull(oldFile);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			testProject.NotifyParseInformationChanged(oldFile, newFile);
		}
		
		protected void UpdateCodeFileInNamespace(string fileName, string code)
		{
			UpdateCodeFile(fileName, "using NUnit.Framework; namespace RootNamespace { " + code + " }");
		}
		
		protected void RemoveCodeFile(string fileName)
		{
			var oldFile = projectContent.GetFile(fileName);
			projectContent = projectContent.RemoveFiles(fileName);
			testProject.NotifyParseInformationChanged(oldFile, null);
		}
	}
}
