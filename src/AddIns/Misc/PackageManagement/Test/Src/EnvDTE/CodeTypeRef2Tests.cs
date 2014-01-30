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
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeTypeRef2Tests
//	{
//		CodeTypeRef2 typeRef;
//		ReturnTypeHelper helper;
//		CodeElement parent;
//		ClassHelper classHelper;
//		
//		[SetUp]
//		public void Init()
//		{
//			helper = new ReturnTypeHelper();
//			classHelper = new ClassHelper();
//			parent = new CodeElement();
//		}
//		
//		void AddUnderlyingClassToReturnType(string fullyQualifiedName)
//		{
//			classHelper.CreatePublicClass(fullyQualifiedName);
//			helper.AddUnderlyingClass(classHelper.Class);
//		}
//		
//		void CreateCodeTypeRef2()
//		{
//			typeRef = new CodeTypeRef2(classHelper.ProjectContentHelper.ProjectContent, parent, helper.ReturnType);
//		}
//		
//		void ReturnTypeUsesDifferentProjectContent()
//		{
//			classHelper = new ClassHelper();
//			classHelper.ProjectContentHelper.SetProjectForProjectContent(ProjectHelper.CreateTestProject());
//		}
//		
//		void ReturnTypeSameProjectContent()
//		{
//			var project = ProjectHelper.CreateTestProject();
//			classHelper.ProjectContentHelper.SetProjectForProjectContent(project);
//		}
//		
//		void ProjectContentIsForVisualBasicProject()
//		{
//			classHelper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
//		}
//		
//		void ProjectContentIsForCSharpProject()
//		{
//			classHelper.ProjectContentHelper.ProjectContentIsForCSharpProject();
//		}
//
//		[Test]
//		public void CodeType_ReturnTypeIsSystemString_ReturnsCodeClass2ForSystemStringType()
//		{
//			helper.CreateReturnType("System.String");
//			AddUnderlyingClassToReturnType("System.String");
//			CreateCodeTypeRef2();
//			
//			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
//			string name = codeClass.FullName;
//			
//			Assert.AreEqual("System.String", name);
//		}
//		
//		[Test]
//		public void CodeType_ReturnTypeFromDifferentProjectContent_CodeTypeLocationIsExternal()
//		{
//			helper.CreateReturnType("System.String");
//			AddUnderlyingClassToReturnType("System.String");
//			ReturnTypeUsesDifferentProjectContent();
//			CreateCodeTypeRef2();
//			
//			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
//			global::EnvDTE.vsCMInfoLocation location = codeClass.InfoLocation;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, location);
//		}
//		
//		[Test]
//		public void CodeType_ReturnTypeFromSameProjectContent_CodeTypeLocationIsProject()
//		{
//			helper.CreateReturnType("MyType");
//			AddUnderlyingClassToReturnType("MyType");
//			ReturnTypeSameProjectContent();
//			CreateCodeTypeRef2();
//			
//			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
//			global::EnvDTE.vsCMInfoLocation location = codeClass.InfoLocation;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, location);
//		}
//		
//		[Test]
//		public void IsGeneric_NotGenericReturnType_ReturnsFalse()
//		{
//			helper.CreateReturnType("MyType");
//			helper.AddDotNetName("MyType");
//			CreateCodeTypeRef2();
//			
//			bool generic = typeRef.IsGeneric;
//			
//			Assert.IsFalse(generic);
//		}
//		
//		[Test]
//		public void IsGeneric_GenericReturnType_ReturnsTrue()
//		{
//			helper.CreateReturnType("System.Nullable");
//			helper.AddDotNetName("System.Nullable{System.String}");
//			CreateCodeTypeRef2();
//			
//			bool generic = typeRef.IsGeneric;
//			
//			Assert.IsTrue(generic);
//		}
//		
//		[Test]
//		public void AsFullName_GenericReturnType_ReturnsDotNetNameWithCurlyBracesReplacedWithAngleBrackets()
//		{
//			helper.CreateReturnType("System.Nullable");
//			helper.AddDotNetName("System.Nullable{System.String}");
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsFullName;
//			
//			Assert.AreEqual("System.Nullable<System.String>", name);
//		}
//		
//		[Test]
//		public void AsString_ReturnTypeIsSystemStringInCSharpProject_ReturnsString()
//		{
//			helper.CreateReturnType("System.String");
//			ProjectContentIsForCSharpProject();
//			AddUnderlyingClassToReturnType("System.String");
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsString;
//			
//			Assert.AreEqual("string", name);
//		}
//		
//		[Test]
//		public void AsString_ReturnTypeIsSystemInt32InCSharpProject_ReturnsInt()
//		{
//			helper.CreateReturnType("System.Int32");
//			AddUnderlyingClassToReturnType("System.Int32");
//			ProjectContentIsForCSharpProject();
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsString;
//			
//			Assert.AreEqual("int", name);
//		}
//		
//		[Test]
//		public void AsString_ReturnTypeIsSystemInt32InVisualBasicProject_ReturnsInteger()
//		{
//			helper.CreateReturnType("System.Int32");
//			AddUnderlyingClassToReturnType("System.Int32");
//			ProjectContentIsForVisualBasicProject();
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsString;
//			
//			Assert.AreEqual("Integer", name);
//		}
//		
//		[Test]
//		public void AsString_ReturnTypeIsCustomType_ReturnsFullTypeName()
//		{
//			helper.CreateReturnType("Test.MyClass");
//			AddUnderlyingClassToReturnType("Test.MyClass");
//			ProjectContentIsForCSharpProject();
//			helper.AddDotNetName("Test.MyClass");
//			CreateCodeTypeRef2();
//			
//			string name = typeRef.AsString;
//			
//			Assert.AreEqual("Test.MyClass", name);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsReferenceType_ReturnsClassType()
//		{
//			helper.CreateReturnType("Test.MyClass");
//			AddUnderlyingClassToReturnType("Test.MyClass");
//			helper.MakeReferenceType();
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefCodeType, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsNotReferenceType_ReturnsNonClassType()
//		{
//			helper.CreateReturnType("Test.MyClass");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemVoid_ReturnsVoidType()
//		{
//			helper.CreateReturnType("System.Void");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemString_ReturnsStringType()
//		{
//			helper.CreateReturnType("System.String");
//			helper.MakeReferenceType();
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefString, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemBoolean_ReturnsBooleanType()
//		{
//			helper.CreateReturnType("System.Boolean");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefBool, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemByte_ReturnsByteType()
//		{
//			helper.CreateReturnType("System.Byte");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefByte, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemChar_ReturnsCharType()
//		{
//			helper.CreateReturnType("System.Char");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefChar, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemDecimal_ReturnsDecimalType()
//		{
//			helper.CreateReturnType("System.Decimal");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefDecimal, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemDouble_ReturnsDoubleType()
//		{
//			helper.CreateReturnType("System.Double");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefDouble, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemSingle_ReturnsFloatType()
//		{
//			helper.CreateReturnType("System.Single");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefFloat, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemInt32_ReturnsIntType()
//		{
//			helper.CreateReturnType("System.Int32");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemInt16_ReturnsShortType()
//		{
//			helper.CreateReturnType("System.Int16");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemInt64_ReturnsLongType()
//		{
//			helper.CreateReturnType("System.Int64");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemUInt32_ReturnsIntType()
//		{
//			helper.CreateReturnType("System.UInt32");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemUInt16_ReturnsShortType()
//		{
//			helper.CreateReturnType("System.UInt16");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemUInt64_ReturnsLongType()
//		{
//			helper.CreateReturnType("System.UInt64");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong, kind);
//		}
//		
//		[Test]
//		public void TypeKind_ReturnTypeIsSystemObject_ReturnsObjectType()
//		{
//			helper.CreateReturnType("System.Object");
//			CreateCodeTypeRef2();
//			
//			global::EnvDTE.vsCMTypeRef kind = typeRef.TypeKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject, kind);
//		}
//	}
//}
