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
