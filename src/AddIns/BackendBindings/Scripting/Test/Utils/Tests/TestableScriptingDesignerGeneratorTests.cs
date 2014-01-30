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
//using ICSharpCode.SharpDevelop.Parser;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Utils.Tests
//{
//	[TestFixture]
//	public class TestableScriptingDesignerGeneratorTests
//	{
//		ParseInformation parseInfo;
//		TestableScriptingDesignerGenerator generator;
//		
//		[Test]
//		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsParseInfoWithOneClass()
//		{
//			CreateTestableScriptingDesignerGenerator();
//			CreateParseInfoWithOneMethod("MyMethod");
//			
//			int classCount = parseInfo.CompilationUnit.Classes.Count;
//			
//			Assert.AreEqual(1, classCount);
//		}
//		
//		void CreateTestableScriptingDesignerGenerator()
//		{
//			MockTextEditorOptions options = new MockTextEditorOptions();
//			generator = new TestableScriptingDesignerGenerator(options);
//		}
//		
//		void CreateParseInfoWithOneMethod(string name)
//		{
//			parseInfo = generator.CreateParseInfoWithOneMethod(name);
//		}
//		
//		[Test]
//		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsClassWithOneMethod()
//		{
//			CreateTestableScriptingDesignerGenerator();
//			CreateParseInfoWithOneMethod("MyMethod");
//			
//			IClass c = parseInfo.CompilationUnit.Classes[0];
//			int methodCount = c.Methods.Count;
//			
//			Assert.AreEqual(1, methodCount);
//		}
//		
//		[Test]
//		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsMethodWithExpectedMethodName()
//		{
//			CreateTestableScriptingDesignerGenerator();
//			CreateParseInfoWithOneMethod("MyMethod");
//			
//			IClass c = parseInfo.CompilationUnit.Classes[0];
//			IMethod method = c.Methods[0];
//			string name = method.Name;
//			string expectedName = "MyMethod";
//			
//			Assert.AreEqual(expectedName, name);
//		}
//		
//		[Test]
//		public void Constructor_NewInstance_ParseInfoToReturnFromParseFileHasOneClass()
//		{
//			CreateTestableScriptingDesignerGenerator();
//			parseInfo = generator.CreateParseInfoWithOneClass();
//			int count = parseInfo.CompilationUnit.Classes.Count;
//			Assert.AreEqual(1, count);
//		}
//		
//		[Test]
//		public void CreateParseInfoWithOneMethodWithTwoParameters_MethodNamePassed_ReturnsMethodWithTwoParameters()
//		{
//			CreateTestableScriptingDesignerGenerator();
//			CreateParseInfoWithOneMethodWithTwoParameters("MyMethod");
//			
//			IClass c = parseInfo.CompilationUnit.Classes[0];
//			IMethod method = c.Methods[0];
//			int parameterCount = method.Parameters.Count;
//			
//			Assert.AreEqual(2, parameterCount);
//		}
//		
//		void CreateParseInfoWithOneMethodWithTwoParameters(string name)
//		{
//			parseInfo = generator.CreateParseInfoWithOneMethodWithTwoParameters(name);
//		}
//	}
//}
