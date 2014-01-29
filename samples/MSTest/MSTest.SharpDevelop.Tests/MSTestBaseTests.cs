// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace MSTest.SharpDevelop.Tests
{
	public class MSTestBaseTests
	{
		protected IProject project;
		protected IProjectContent projectContent;
		
		[SetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			project = MockRepository.GenerateStrictMock<IProject>();
			projectContent = new CSharpProjectContent();
			
			SD.ParserService
				.Stub(p => p.GetCompilation(project))
				.WhenCalled(c => c.ReturnValue = projectContent.CreateCompilation());
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		public void AddCodeFile(string fileName, string code)
		{
			IUnresolvedFile oldFile = projectContent.GetFile(fileName);
			IUnresolvedFile newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
		}
		
		IUnresolvedFile Parse(string fileName, string code)
		{
			var parser = new CSharpParser();
			SyntaxTree syntaxTree = parser.Parse(code, fileName);
			Assert.IsFalse(parser.HasErrors);
			return syntaxTree.ToTypeSystem();
		}
		
		protected ITypeDefinition GetFirstTypeDefinition()
		{
			ICompilation compilation = projectContent.CreateCompilation();
			return compilation.MainAssembly.TopLevelTypeDefinitions.First();
		}
	}
}
