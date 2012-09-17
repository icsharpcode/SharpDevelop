// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Dom
{
	[TestFixture]
	public class CSharpModelTests
	{
		IProject project;
		IProjectContent projectContent;
		IEntityModelContext context;
		TopLevelTypeDefinitionModelCollection topLevelTypeModels;
		
		#region SetUp and other helper methods
		[SetUp]
		public virtual void SetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddStrictMockService<IParserService>();
			project = MockRepository.GenerateStrictMock<IProject>();
			projectContent = new CSharpProjectContent().AddAssemblyReferences(AssemblyLoader.Corlib);
			context = new ProjectEntityModelContext(project, ".cs");
			topLevelTypeModels = new TopLevelTypeDefinitionModelCollection(context);
			
			SD.ParserService.Stub(p => p.GetCompilation(project)).WhenCalled(c => c.ReturnValue = projectContent.CreateCompilation());
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		protected void AddCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNull(oldFile);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			topLevelTypeModels.NotifyParseInformationChanged(oldFile, newFile);
		}
		
		IUnresolvedFile Parse(string fileName, string code)
		{
			var parser = new CSharpParser();
			var syntaxTree = parser.Parse(code, fileName);
			Assert.IsFalse(parser.HasErrors);
			return syntaxTree.ToTypeSystem();
		}
		
		protected void UpdateCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNotNull(oldFile);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			topLevelTypeModels.NotifyParseInformationChanged(oldFile, newFile);
		}
		
		protected void RemoveCodeFile(string fileName)
		{
			var oldFile = projectContent.GetFile(fileName);
			projectContent = projectContent.RemoveFiles(fileName);
			topLevelTypeModels.NotifyParseInformationChanged(oldFile, null);
		}
		#endregion
		
		[Test]
		public void EmptyProject()
		{
			Assert.AreEqual(0, topLevelTypeModels.Count);
		}
	}
}
