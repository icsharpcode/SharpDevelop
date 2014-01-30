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

//using System;
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Editor;
//using UnitTesting.Tests.Utils;
//
//namespace ICSharpCode.Scripting.Tests.Utils
//{
//	public class TestableScriptingDesignerGenerator : ScriptingDesignerGenerator
//	{
//		public bool IsParseFileCalled;
//		public ParseInformation ParseInfoToReturnFromParseFile;
//		public string FileNamePassedToParseFile;
//		public string TextContentPassedToParseFile;
//		public FakeCodeDomSerializer SerializerToReturnFromCreateCodeDomSerializer;
//		
//		public TestableScriptingDesignerGenerator(ITextEditorOptions textEditorOptions)
//			: base(textEditorOptions)
//		{
//		}
//		
//		public ParseInformation CreateParseInfoWithOneClass()
//		{
//			MockClass c = CreateClass();
//			return new ParseInformation(c.CompilationUnit);			
//		}
//		
//		MockClass CreateClass()
//		{
//			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
//			c.CompilationUnit.Classes.Add(c);
//			return c;
//		}
//		
//		protected override ParseInformation ParseFile(string fileName, string textContent)
//		{
//			IsParseFileCalled = true;
//			FileNamePassedToParseFile = fileName;
//			TextContentPassedToParseFile = textContent;
//			
//			return ParseInfoToReturnFromParseFile;
//		}
//		
//		public ParseInformation CreateParseInfoWithOneMethod(string name)
//		{
//			MockMethod method = CreateMethod(name);
//			return new ParseInformation(method.CompilationUnit);
//		}
//		
//		MockMethod CreateMethod(string name)
//		{
//			MockClass c = CreateClass();
//			MockMethod method = new MockMethod(c, name);
//			c.Methods.Add(method);
//			return method;
//		}
//		
//		public ParseInformation CreateParseInfoWithOneMethodWithTwoParameters(string name)
//		{
//			MockMethod method = CreateMethod(name);
//			method.Parameters.Add(new MockParameter());
//			method.Parameters.Add(new MockParameter());
//			return new ParseInformation(method.CompilationUnit);
//		}
//		
//		public override IScriptingCodeDomSerializer CreateCodeDomSerializer(ITextEditorOptions options)
//		{
//			return SerializerToReturnFromCreateCodeDomSerializer;
//		}
//	}
//}
