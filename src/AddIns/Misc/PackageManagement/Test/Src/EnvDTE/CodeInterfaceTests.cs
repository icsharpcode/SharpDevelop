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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeInterfaceTests : CodeModelTestBase
	{
		CodeInterface codeInterface;
		ITypeDefinition interfaceTypeDefinition;
		
		ITypeDefinition addMethodAtStartTypeDef;
		Accessibility addMethodAtStartAccess;
		IType addMethodAtStartReturnType;
		string addMethodAtStartName;
		
		void UpdateCode(string code, string fileName = @"c:\projects\MyProject\interface.cs")
		{
			CreateCompilationForUpdatedCodeFile(fileName, code);
		}
		
		void CreateInterface(string code, string fileName = @"c:\projects\MyProject\interface.cs")
		{
			CreateCodeModel();
			AddCodeFile(fileName, code);
			interfaceTypeDefinition = assemblyModel.TopLevelTypeDefinitions.First().Resolve();
			codeInterface = new CodeInterface(codeModelContext, interfaceTypeDefinition);
		}
		
		void CaptureCodeGeneratorAddMethodAtStartParameters()
		{
			codeGenerator
				.Stub(generator => generator.AddMethodAtStart(
					Arg<ITypeDefinition>.Is.Anything,
					Arg<Accessibility>.Is.Anything,
					Arg<IType>.Is.Anything,
					Arg<string>.Is.Anything))
				.Callback<ITypeDefinition, Accessibility, IType, string>((typeDef, access, returnType, name) => {
					addMethodAtStartTypeDef = typeDef;
					addMethodAtStartAccess = access;
					addMethodAtStartReturnType = returnType;
					addMethodAtStartName = name;
					return true;
				});
		}
		
		[Test]
		public void Kind_Interface_ReturnsInterface()
		{
			CreateInterface("interface MyInterface {}");
			
			global::EnvDTE.vsCMElement kind = codeInterface.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementInterface, kind);
		}
		
		[Test]
		public void AddFunction_PublicFunctionReturningSystemInt32_AddsPublicFunctionWithCodeConverter()
		{
			CreateInterface("interface MyInterface {}");
			var kind = global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			var access = global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			CaptureCodeGeneratorAddMethodAtStartParameters();
			string newCode = 
				"interface MyInterface {\r\n" +
				"    System.Int32 MyMethod();\r\n" +
				"}";
			UpdateCode(newCode);
			
			codeInterface.AddFunction("MyMethod", kind, "System.Int32", null, access);
			
			Assert.AreEqual(Accessibility.Public, addMethodAtStartAccess);
			Assert.AreEqual("MyMethod", addMethodAtStartName);
			Assert.AreEqual(interfaceTypeDefinition, addMethodAtStartTypeDef);
			Assert.AreEqual("System.Int32", addMethodAtStartReturnType.FullName);
			Assert.IsTrue(addMethodAtStartReturnType.IsKnownType (KnownTypeCode.Int32));
		}
		
		[Test]
		public void AddFunction_PrivateFunctionReturningUnknownType_AddsPrivateFunctionWithCodeConverter()
		{
			CreateInterface ("interface MyInterface {}");
			var kind = global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			var access = global::EnvDTE.vsCMAccess.vsCMAccessPrivate;
			CaptureCodeGeneratorAddMethodAtStartParameters();
			string newCode = 
				"interface MyInterface {\r\n" +
				"    Unknown.MyUnknownType MyMethod();\r\n" +
				"}";
			UpdateCode(newCode);
			
			codeInterface.AddFunction("MyMethod", kind, "Unknown.MyUnknownType", null, access);
			
			Assert.AreEqual(Accessibility.Private, addMethodAtStartAccess);
			Assert.AreEqual("MyMethod", addMethodAtStartName);
			Assert.AreEqual(interfaceTypeDefinition, addMethodAtStartTypeDef);
			Assert.AreEqual("Unknown.MyUnknownType", addMethodAtStartReturnType.FullName);
		}
		
		[Test]
		public void AddFunction_PublicFunctionReturningSystemInt32_ReturnsCodeFunctionForNewMethod()
		{
			string fileName = @"c:\projects\MyProject\interface.cs";
			CreateInterface("interface MyInterface {}", fileName);
			var kind = global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			var access = global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			string newCode = 
				"interface MyInterface {\r\n" +
				"    int MyMethod();\r\n" +
				"}";
			UpdateCode(newCode, fileName);
			
			var codefunction = codeInterface.AddFunction("MyMethod", kind, "System.Int32", null, access) as CodeFunction;
			
			Assert.AreEqual("MyMethod", codefunction.Name);
		}
		
		[Test]
		public void AddFunction_MethodNotFoundAfterReloadingTypeDefinition_ReturnsNull()
		{
			string fileName = @"c:\projects\MyProject\interface.cs";
			CreateInterface("interface MyInterface {}", fileName);
			var kind = global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			var access = global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			string newCode = "interface MyInterface {}";
			UpdateCode(newCode, fileName);
			
			var codefunction = codeInterface.AddFunction("MyMethod", kind, "System.Int32", null, access) as CodeFunction;
			
			Assert.IsNull(codefunction);
		}
		
		[Test]
		public void AddFunction_UnableToFindTypeDefinitionAfterUpdate_ReturnsNull()
		{
			string fileName = @"c:\projects\MyProject\interface.cs";
			CreateInterface("interface MyInterface {}", fileName);
			var kind = global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			var access = global::EnvDTE.vsCMAccess.vsCMAccessPublic;
			string newCode = "interface SomeOtherInterface {}";
			UpdateCode(newCode, fileName);
			
			var codefunction = codeInterface.AddFunction("MyMethod", kind, "System.Int32", null, access) as CodeFunction;
			
			Assert.IsNull(codefunction);
			Assert.AreEqual("MyInterface", codeInterface.Name);
		}
	}
}
