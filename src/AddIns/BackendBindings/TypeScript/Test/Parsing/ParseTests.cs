
using System;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TypeScriptBinding;
using ICSharpCode.TypeScriptBinding.Hosting;
using Rhino.Mocks;

namespace TypeScriptBinding.Tests.Parsing
{
	public abstract class ParseTests
	{
		public ParseInformation ParseInfo { get; private set; }
		public IProject Project { get; private set; }
		
		public void Parse(string text, string fileName = @"d:\projects\MyProject\test.ts")
		{
			Project = MockRepository.GenerateStub<IProject>();
			var fileContent = new TextDocument(text);
			
			var scriptLoader = new ParseTestScriptLoader();
			var logger = new LanguageServiceLogger();
			ITypeScriptContextFactory contextFactory = MockRepository.GenerateStub<ITypeScriptContextFactory>();
			contextFactory
				.Stub(f => f.CreateContext())
				.Return(new TypeScriptContext(scriptLoader, logger));
			
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
