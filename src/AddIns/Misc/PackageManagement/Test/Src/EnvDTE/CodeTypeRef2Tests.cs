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

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeTypeRef2Tests : CodeModelTestBase
	{
		CodeTypeRef2 typeRef;
		CodeFunction2 parent;
		
		void CreateCodeTypeRef2(string code)
		{
			AddCodeFile("class.cs", code);
			IMethod method = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Members
				.First()
				.Resolve() as IMethod;
			
			parent = new CodeFunction2(codeModelContext, method);
			typeRef = parent.Type as CodeTypeRef2;
		}

		[Test]
		public void CodeType_ReturnTypeIsSystemString_ReturnsCodeClass2ForSystemStringType()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    string MyMethod() { return null; }\r\n" +
				"}");
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			string name = codeClass.FullName;
			
			Assert.AreEqual("System.String", name);
		}
		
		[Test]
		public void CodeType_ReturnTypeFromDifferentAssemblyFromProjectt_CodeTypeLocationIsExternal()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    string MyMethod() { return null; }\r\n" +
				"}");
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			global::EnvDTE.vsCMInfoLocation location = codeClass.InfoLocation;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, location);
		}
		
		[Test]
		public void CodeType_ReturnTypeFromSameProjectContent_CodeTypeLocationIsProject()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    MyType MyMethod() { return null; }\r\n" +
				"}\r\n" +
				"class MyType {}");
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			global::EnvDTE.vsCMInfoLocation location = codeClass.InfoLocation;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, location);
		}
		
		[Test]
		public void IsGeneric_NotGenericReturnType_ReturnsFalse()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    string MyMethod() { return null; }\r\n" +
				"}");
			
			bool generic = typeRef.IsGeneric;
			
			Assert.IsFalse(generic);
		}
		
		[Test]
		public void IsGeneric_GenericReturnType_ReturnsTrue()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    Nullable<int> MyMethod() { return null; }\r\n" +
				"}");
			
			bool generic = typeRef.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void AsFullName_GenericReturnType_ReturnsDotNetNameWithCurlyBracesReplacedWithAngleBrackets()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    Nullable<int> MyMethod() { return null; }\r\n" +
				"}");
			
			string name = typeRef.AsFullName;
			
			Assert.AreEqual("System.Nullable<System.Int32>", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsSystemStringInCSharpProject_ReturnsString()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    string MyMethod() { return null; }\r\n" +
				"}");
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("string", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsSystemInt32InCSharpProject_ReturnsInt()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"class MyClass {\r\n" +
				"    System.Int32 MyMethod() { return 10; }\r\n" +
				"}");
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("int", name);
		}
		
		[Test]
		[Ignore("VB.NET not supported")]
		public void AsString_ReturnTypeIsSystemInt32InVisualBasicProject_ReturnsInteger()
		{
//			helper.CreateReturnType("System.Int32");
//			AddUnderlyingClassToReturnType("System.Int32");
//			ProjectContentIsForVisualBasicProject();
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsString;
//			
//			Assert.AreEqual("Integer", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsCustomType_ReturnsFullTypeName()
		{
			CreateCodeTypeRef2(
				"using System;\r\n" +
				"namespace Test {\r\n" +
				"    class MyClass {\r\n" +
				"        CustomType MyMethod() { return null; }\r\n" +
				"    }\r\n" +
				"    class CustomType {}\r\n" +
				"}");
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("Test.CustomType", name);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsReferenceType_ReturnsClassType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    MyType MyMethod() { return null; }\r\n" +
				"}\r\n" +
				"class MyType {}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefCodeType, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsUnknownTypeAndNotReferenceType_ReturnsNonClassType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    MyType MyMethod() { return null; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemVoid_ReturnsVoidType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    void MyMethod() { }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemString_ReturnsStringType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    string MyMethod() { return null; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefString, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemBoolean_ReturnsBooleanType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    bool MyMethod() { return false; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefBool, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemByte_ReturnsByteType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    byte MyMethod() { return 0x1; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefByte, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemChar_ReturnsCharType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    char MyMethod() { return 'a'; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefChar, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemDecimal_ReturnsDecimalType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    decimal MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefDecimal, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemDouble_ReturnsDoubleType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    double MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefDouble, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemSingle_ReturnsFloatType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.Single MyMethod() { return 0.1; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefFloat, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemInt32_ReturnsIntType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.Int32 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemInt16_ReturnsShortType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.Int16 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemInt64_ReturnsLongType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.Int64 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemUInt32_ReturnsIntType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.UInt32 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemUInt16_ReturnsShortType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.UInt16 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemUInt64_ReturnsLongType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.UInt64 MyMethod() { return 0; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong, kind);
		}
		
		[Test]
		public void TypeKind_ReturnTypeIsSystemObject_ReturnsObjectType()
		{
			CreateCodeTypeRef2(
				"class MyClass {\r\n" +
				"    System.Object MyMethod() { return null; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject, kind);
		}
	}
}
