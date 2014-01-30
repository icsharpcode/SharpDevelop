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
//using System.Collections.Generic;
//using ICSharpCode.NRefactory.Ast;
//using ICSharpCode.SharpDevelop.Dom.Refactoring;
//using NUnit.Framework;
//
//namespace PackageManagement.Tests
//{
//	[TestFixture]
//	public class CodeGeneratorTests
//	{
//		CSharpCodeGenerator codeGenerator;
//		
//		void CreateCodeGenerator()
//		{
//			codeGenerator = new CSharpCodeGenerator();
//		}
//		
//		[Test]
//		public void GenerateCode_Field_CreatesField()
//		{
//			CreateCodeGenerator();
//			var field = new FieldDeclaration(new List<AttributeSection>());
//			field.TypeReference = new TypeReference("MyClass");
//			field.Modifier = Modifiers.Public;
//			field.Fields.Add(new VariableDeclaration("myField"));
//			
//			string code = codeGenerator.GenerateCode(field, String.Empty);
//			
//			string expectedCode = "public MyClass myField;\r\n";
//			
//			Assert.AreEqual(expectedCode, code);
//		}
//		
//		[Test]
//		public void GenerateCode_Method_CreatesMethod()
//		{
//			CreateCodeGenerator();
//			var method = new MethodDeclaration();
//			method.Name = "MyMethod";
//			method.TypeReference = new TypeReference("MyReturnType");
//			method.Modifier = Modifiers.Public;
//			method.Body = new BlockStatement();
//			
//			string code = codeGenerator.GenerateCode(method, String.Empty);
//			
//			string expectedCode = 
//				"public MyReturnType MyMethod()\r\n" +
//				"{\r\n" +
//				"}\r\n";
//			
//			Assert.AreEqual(expectedCode, code);
//		}
//		
//		[Test]
//		public void GenerateCode_InterfaceMethodDeclaration_CreatesMethod()
//		{
//			CreateCodeGenerator();
//			var method = new MethodDeclaration();
//			method.Name = "MyMethod";
//			method.TypeReference = new TypeReference("MyReturnType");
//			
//			string code = codeGenerator.GenerateCode(method, String.Empty);
//			
//			string expectedCode = "MyReturnType MyMethod();\r\n";
//			
//			Assert.AreEqual(expectedCode, code);
//		}
//	}
//}
