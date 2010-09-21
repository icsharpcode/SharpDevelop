// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class TestableScriptingDesignerGeneratorTests
	{
		ParseInformation parseInfo;
		TestableScriptingDesignerGenerator generator;
		
		[Test]
		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsParseInfoWithOneClass()
		{
			CreateTestableScriptingDesignerGenerator();
			CreateParseInfoWithOneMethod("MyMethod");
			
			int classCount = parseInfo.CompilationUnit.Classes.Count;
			
			Assert.AreEqual(1, classCount);
		}
		
		void CreateTestableScriptingDesignerGenerator()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			generator = new TestableScriptingDesignerGenerator(options);
		}
		
		void CreateParseInfoWithOneMethod(string name)
		{
			parseInfo = generator.CreateParseInfoWithOneMethod(name);
		}
		
		[Test]
		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsClassWithOneMethod()
		{
			CreateTestableScriptingDesignerGenerator();
			CreateParseInfoWithOneMethod("MyMethod");
			
			IClass c = parseInfo.CompilationUnit.Classes[0];
			int methodCount = c.Methods.Count;
			
			Assert.AreEqual(1, methodCount);
		}
		
		[Test]
		public void CreateParseInfoWithOneMethod_MethodNamePassed_ReturnsMethodWithExpectedMethodName()
		{
			CreateTestableScriptingDesignerGenerator();
			CreateParseInfoWithOneMethod("MyMethod");
			
			IClass c = parseInfo.CompilationUnit.Classes[0];
			IMethod method = c.Methods[0];
			string name = method.Name;
			string expectedName = "MyMethod";
			
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void Constructor_NewInstance_ParseInfoToReturnFromParseFileHasOneClass()
		{
			CreateTestableScriptingDesignerGenerator();
			parseInfo = generator.CreateParseInfoWithOneClass();
			int count = parseInfo.CompilationUnit.Classes.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void CreateParseInfoWithOneMethodWithTwoParameters_MethodNamePassed_ReturnsMethodWithTwoParameters()
		{
			CreateTestableScriptingDesignerGenerator();
			CreateParseInfoWithOneMethodWithTwoParameters("MyMethod");
			
			IClass c = parseInfo.CompilationUnit.Classes[0];
			IMethod method = c.Methods[0];
			int parameterCount = method.Parameters.Count;
			
			Assert.AreEqual(2, parameterCount);
		}
		
		void CreateParseInfoWithOneMethodWithTwoParameters(string name)
		{
			parseInfo = generator.CreateParseInfoWithOneMethodWithTwoParameters(name);
		}
	}
}
