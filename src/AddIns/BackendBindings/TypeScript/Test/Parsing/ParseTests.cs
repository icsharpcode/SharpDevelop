
using System;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TypeScriptBinding;
using ICSharpCode.TypeScriptBinding.Hosting;
using NUnit.Framework;
using Rhino.Mocks;

namespace TypeScriptBinding.Tests.Parsing
{
	public abstract class ParseTests
	{
		public ParseInformation ParseInfo { get; private set; }
		public IProject Project { get; private set; }
		
		DefaultJavaScriptContext javaScriptContext;
		
		[SetUp]
		public void Init()
		{
			try {
				javaScriptContext = new DefaultJavaScriptContext();
			} catch (FileNotFoundException ex) {
				Assert.Inconclusive("Missing Visual C++ 2010 runtime. " + ex.Message);
			}
		}
		
		public void Parse(string text, string fileName = @"d:\projects\MyProject\test.ts")
		{
			Project = MockRepository.GenerateStub<IProject>();
			var fileContent = new TextDocument(text);
			
			var scriptLoader = new ParseTestScriptLoader();
			var logger = new LanguageServiceLogger();
			ITypeScriptContextFactory contextFactory = MockRepository.GenerateStub<ITypeScriptContextFactory>();
			contextFactory
				.Stub(f => f.CreateContext())
				.Return(new TypeScriptContext(javaScriptContext, scriptLoader, logger));
			
			var parser = new TypeScriptParser(contextFactory);
			ParseInfo = parser.Parse(new FileName(fileName), fileContent, null, new TypeScriptFile[0]);
		}
		
		public IUnresolvedTypeDefinition GetFirstClass()
		{
			return GetClassAtIndex(0);
		}
		
		public IUnresolvedTypeDefinition GetSecondClass()
		{
			return GetClassAtIndex(1);
		}
		
		public IUnresolvedTypeDefinition GetClassAtIndex(int index)
		{
			return ParseInfo.UnresolvedFile.TopLevelTypeDefinitions.Skip(index).First();
		}
	}
}
