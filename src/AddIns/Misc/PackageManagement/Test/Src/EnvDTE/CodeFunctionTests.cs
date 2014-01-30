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
//using ICSharpCode.PackageManagement;
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeFunctionTests
//	{
//		CodeFunction codeFunction;
//		MethodHelper helper;
//		IVirtualMethodUpdater methodUpdater;
//		
//		[SetUp]
//		public void Init()
//		{
//			helper = new MethodHelper();
//		}
//		
//		void CreatePublicFunction(string name)
//		{
//			helper.CreatePublicMethod(name);
//			CreateFunction();
//		}
//		
//		void CreatePrivateFunction(string name)
//		{
//			helper.CreatePrivateMethod(name);
//			CreateFunction();
//		}
//		
//		void CreateFunction()
//		{
//			methodUpdater = MockRepository.GenerateStub<IVirtualMethodUpdater>();
//			codeFunction = new CodeFunction(helper.Method, null, methodUpdater);
//		}
//		
//		void CreatePublicConstructor(string name)
//		{
//			helper.CreatePublicConstructor(name);
//			CreateFunction();
//		}
//		
//		void SetDeclaringTypeAsInterface(string name)
//		{
//			helper.AddDeclaringTypeAsInterface(name);
//		}
//		
//		void SetDeclaringType(string name)
//		{
//			helper.AddDeclaringType(name);
//		}
//		
//		void AddParameterToMethod(string name)
//		{
//			helper.AddParameter(name);
//		}
//		
//		void AddParameterToMethod(string type, string name)
//		{
//			helper.AddParameter(type, name);
//		}
//		
//		void AddReturnTypeToMethod(string type)
//		{
//			helper.AddReturnTypeToMethod(type);
//		}
//		
//		void MakeMethodStatic()
//		{
//			helper.MakeMethodStatic();
//		}
//		
//		void MakeMethodAbstract()
//		{
//			helper.MakeMethodAbstract();
//		}
//		
//		void MakeMethodVirtual()
//		{
//			helper.MakeMethodVirtual();
//		}
//		
//		void AddMethodAttribute(string attributeTypeName)
//		{
//			helper.AddAttributeToMethod(attributeTypeName);
//		}
//		
//		void MakeMethodOverridable()
//		{
//			helper.MakeMethodOverridable();
//		}
//		
//		[Test]
//		public void Access_PublicFunction_ReturnsPublic()
//		{
//			CreatePublicFunction("Class1.MyFunction");
//			
//			global::EnvDTE.vsCMAccess access = codeFunction.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
//		}
//		
//		[Test]
//		public void Access_PrivateFunction_ReturnsPrivate()
//		{
//			CreatePrivateFunction("Class1.MyFunction");
//			
//			global::EnvDTE.vsCMAccess access = codeFunction.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
//		}
//		
//		[Test]
//		public void GetStartPoint_FunctionStartsAtColumn3_ReturnsPointWithOffset3()
//		{
//			CreatePublicFunction("Class1.MyFunction");
//			SetDeclaringType("Class1");
//			helper.FunctionStartsAtColumn(3);
//			
//			global::EnvDTE.TextPoint point = codeFunction.GetStartPoint();
//			int offset = point.LineCharOffset;
//			
//			Assert.AreEqual(3, offset);
//		}
//		
//		[Test]
//		public void GetStartPoint_FunctionStartsAtLine2_ReturnsPointWithLine2()
//		{
//			CreatePublicFunction("Class1.MyFunction");
//			SetDeclaringType("Class1");
//			helper.FunctionStartsAtLine(2);
//			
//			global::EnvDTE.TextPoint point = codeFunction.GetStartPoint();
//			int line = point.Line;
//			
//			Assert.AreEqual(2, line);
//		}
//		
//		[Test]
//		public void GetEndPoint_FunctionBodyEndsAtColumn4_ReturnsPointWithOffset4()
//		{
//			CreatePublicFunction("Class1.MyFunction");
//			SetDeclaringType("Class1");
//			helper.FunctionBodyEndsAtColumn(4);
//			
//			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
//			int offset = point.LineCharOffset;
//			
//			Assert.AreEqual(4, offset);
//		}
//		
//		[Test]
//		public void GetEndPoint_FunctionBodyEndsAtLine4_ReturnsPointWithLine4()
//		{
//			CreatePublicFunction("Class1.MyFunction");
//			SetDeclaringType("Class1");
//			helper.FunctionBodyEndsAtLine(4);
//			
//			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
//			int line = point.Line;
//			
//			Assert.AreEqual(4, line);
//		}
//		
//		[Test]
//		public void Kind_PublicFunction_ReturnsFunction()
//		{
//			CreatePublicFunction("MyFunction");
//			
//			global::EnvDTE.vsCMElement kind = codeFunction.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementFunction, kind);
//		}
//				
//		[Test]
//		public void GetEndPoint_InterfaceMethod_ReturnsMethodsDeclarationRegionEndNotBodyRegionEnd()
//		{
//			CreatePublicFunction("MyInterface.MyMethod");
//			SetDeclaringTypeAsInterface("MyInterface");
//			helper.SetRegion(new DomRegion(1, 1, 1, 10));
//			helper.SetBodyRegion(new DomRegion(1, 10, 0, 0));
//			
//			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
//			
//			Assert.AreEqual(1, point.Line);
//			Assert.AreEqual(10, point.LineCharOffset);
//		}
//		
//		[Test]
//		public void Parameters_MethodHasNoParameters_ReturnsEmptyListOfItems()
//		{
//			CreatePublicFunction("MyClass.MyMethod");
//			
//			global::EnvDTE.CodeElements parameters = codeFunction.Parameters;
//			
//			Assert.AreEqual(0, parameters.Count);
//		}
//		
//		[Test]
//		public void Parameters_MethodHasOneParameter_ReturnsOneCodeParameter2()
//		{
//			AddParameterToMethod("test");
//			CreatePublicFunction("MyClass.MyMethod");
//			
//			CodeParameter2 parameter = codeFunction.Parameters.FirstCodeParameter2OrDefault();
//			
//			Assert.AreEqual("test", parameter.Name);
//		}
//		
//		[Test]
//		public void Parameters_MethodHasOneStringParameter_ReturnsOneCodeParameterWithStringType()
//		{
//			AddParameterToMethod("System.String", "test");
//			CreatePublicFunction("MyClass.MyMethod");
//			
//			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
//			
//			Assert.AreEqual("System.String", parameter.Type.AsFullName);
//		}
//		
//		[Test]
//		public void Parameters_MethodHasOneStringParameter_CreatesCodeParameterWithProjectContent()
//		{
//			AddParameterToMethod("System.String", "test");
//			CreatePublicFunction("MyClass.MyMethod");
//			
//			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
//			
//			Assert.AreEqual("string", parameter.Type.AsString);
//		}
//		
//		[Test]
//		public void Parameters_MethodHasOneStringParameter_CreatesCodeParameterWithCodeTypeRefThatHasParameterAsParent()
//		{
//			AddParameterToMethod("System.String", "test");
//			CreatePublicFunction("MyClass.MyMethod");
//			
//			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
//			
//			Assert.AreEqual(parameter, parameter.Type.Parent);
//		}
//		
//		[Test]
//		public void Type_MethodReturnsString_TypeRefHasSystemStringAsFullName()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			AddReturnTypeToMethod("System.String");
//			
//			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
//			
//			Assert.AreEqual("System.String", typeRef.AsFullName);
//		}
//		
//		[Test]
//		public void Type_MethodReturnsString_TypeRefUsesProjectContentFromMethod()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			AddReturnTypeToMethod("System.String");
//			
//			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
//			
//			Assert.AreEqual("string", typeRef.AsString);
//		}
//		
//		[Test]
//		public void Type_MethodReturnsString_TypeRefParentIsCodeFunction()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			AddReturnTypeToMethod("System.String");
//			
//			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
//			
//			Assert.AreEqual(codeFunction, typeRef.Parent);
//		}
//		
//		[Test]
//		public void FunctionKind_ClassMethod_ReturnsFunctionKind()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			global::EnvDTE.vsCMFunction kind = codeFunction.FunctionKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMFunction.vsCMFunctionFunction, kind);
//		}
//		
//		[Test]
//		public void FunctionKind_ClassConstructor_ReturnsConstructorKind()
//		{
//			CreatePublicConstructor("MyClass.MyClass");
//			
//			global::EnvDTE.vsCMFunction kind = codeFunction.FunctionKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMFunction.vsCMFunctionConstructor, kind);
//		}
//		
//		[Test]
//		public void IsShared_StaticMethod_ReturnsTrue()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			MakeMethodStatic();
//			
//			bool shared = codeFunction.IsShared;
//			
//			Assert.IsTrue(shared);
//		}
//		
//		[Test]
//		public void IsShared_MethodIsNotStatic_ReturnsFalse()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			bool shared = codeFunction.IsShared;
//			
//			Assert.IsFalse(shared);
//		}
//		
//		[Test]
//		public void MustImplement_AbstractMethod_ReturnsTrue()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			MakeMethodAbstract();
//			
//			bool mustImplement = codeFunction.MustImplement;
//			
//			Assert.IsTrue(mustImplement);
//		}
//		
//		[Test]
//		public void MustImplement_MethodIsNotAbstract_ReturnsFalse()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			bool mustImplement = codeFunction.MustImplement;
//			
//			Assert.IsFalse(mustImplement);
//		}
//		
//		[Test]
//		public void CanOverride_MethodIsOverridable_ReturnsTrue()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			MakeMethodOverridable();
//			
//			bool canOverride = codeFunction.CanOverride;
//			
//			Assert.IsTrue(canOverride);
//		}
//		
//		[Test]
//		public void CanOverride_MethodIsNotAbstractOrVirtual_ReturnsFalse()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			bool canOverride = codeFunction.CanOverride;
//			
//			Assert.IsFalse(canOverride);
//		}
//		
//		[Test]
//		public void Attributes_MethodHasOneAttribute_ReturnsOneAttribute()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			AddMethodAttribute("System.ObsoleteAttribute");
//			
//			global::EnvDTE.CodeElements attributes = codeFunction.Attributes;
//			
//			CodeAttribute2 attribute = attributes.FirstCodeAttribute2OrDefault();
//			Assert.AreEqual(1, attributes.Count);
//			Assert.AreEqual("System.ObsoleteAttribute", attribute.FullName);
//		}
//		
//		[Test]
//		public void CanOverride_SetToTrueForFunction_VirtualKeywordAddedToFunction()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			codeFunction.CanOverride = true;
//			
//			methodUpdater.AssertWasCalled(updater => updater.MakeMethodVirtual());
//		}
//		
//		[Test]
//		public void CanOverride_SetToFalseForFunction_VirtualKeywordNotAddedToFunction()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			codeFunction.CanOverride = false;
//			
//			methodUpdater.AssertWasNotCalled(updater => updater.MakeMethodVirtual());
//		}
//	}
//}
