// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableScriptingDesignerGenerator : ScriptingDesignerGenerator
	{
		public bool IsParseFileCalled;
		public ParseInformation ParseInfoToReturnFromParseFile;
		public string FileNamePassedToParseFile;
		public string TextContentPassedToParseFile;
		public FakeCodeDomSerializer SerializerToReturnFromCreateCodeDomSerializer;
		
		public TestableScriptingDesignerGenerator(ITextEditorOptions textEditorOptions)
			: base(textEditorOptions)
		{
		}
		
		public ParseInformation CreateParseInfoWithOneClass()
		{
			MockClass c = CreateClass();
			return new ParseInformation(c.CompilationUnit);			
		}
		
		MockClass CreateClass()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.CompilationUnit.Classes.Add(c);
			return c;
		}
		
		protected override ParseInformation ParseFile(string fileName, string textContent)
		{
			IsParseFileCalled = true;
			FileNamePassedToParseFile = fileName;
			TextContentPassedToParseFile = textContent;
			
			return ParseInfoToReturnFromParseFile;
		}
		
		public ParseInformation CreateParseInfoWithOneMethod(string name)
		{
			MockMethod method = CreateMethod(name);
			return new ParseInformation(method.CompilationUnit);
		}
		
		MockMethod CreateMethod(string name)
		{
			MockClass c = CreateClass();
			MockMethod method = new MockMethod(c, name);
			c.Methods.Add(method);
			return method;
		}
		
		public ParseInformation CreateParseInfoWithOneMethodWithTwoParameters(string name)
		{
			MockMethod method = CreateMethod(name);
			method.Parameters.Add(new MockParameter());
			method.Parameters.Add(new MockParameter());
			return new ParseInformation(method.CompilationUnit);
		}
		
		public override IScriptingCodeDomSerializer CreateCodeDomSerializer(ITextEditorOptions options)
		{
			return SerializerToReturnFromCreateCodeDomSerializer;
		}
	}
}
