// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class CodeGeneratorTests
	{
		CSharpCodeGenerator codeGenerator;
		
		void CreateCodeGenerator()
		{
			codeGenerator = new CSharpCodeGenerator();
		}
		
		[Test]
		public void GenerateCode_Field_CreatesField()
		{
			CreateCodeGenerator();
			var field = new FieldDeclaration(new List<AttributeSection>());
			field.TypeReference = new TypeReference("MyClass");
			field.Modifier = Modifiers.Public;
			field.Fields.Add(new VariableDeclaration("myField"));
			
			string code = codeGenerator.GenerateCode(field, String.Empty);
			
			string expectedCode = "public MyClass myField;\r\n";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void GenerateCode_Method_CreatesMethod()
		{
			CreateCodeGenerator();
			var method = new MethodDeclaration();
			method.Name = "MyMethod";
			method.TypeReference = new TypeReference("MyReturnType");
			method.Modifier = Modifiers.Public;
			method.Body = new BlockStatement();
			
			string code = codeGenerator.GenerateCode(method, String.Empty);
			
			string expectedCode = 
				"public MyReturnType MyMethod()\r\n" +
				"{\r\n" +
				"}\r\n";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void GenerateCode_InterfaceMethodDeclaration_CreatesMethod()
		{
			CreateCodeGenerator();
			var method = new MethodDeclaration();
			method.Name = "MyMethod";
			method.TypeReference = new TypeReference("MyReturnType");
			
			string code = codeGenerator.GenerateCode(method, String.Empty);
			
			string expectedCode = "MyReturnType MyMethod();\r\n";
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
