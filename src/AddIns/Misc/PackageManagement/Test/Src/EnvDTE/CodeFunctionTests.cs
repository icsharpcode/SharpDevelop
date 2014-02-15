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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using Rhino.Mocks;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeFunctionTests : CodeModelTestBase
	{
		CodeFunction codeFunction;
		
		void CreateFunction(string code)
		{
			AddCodeFile("class.cs", code);
			
			IMethod method = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Members
				.First()
				.Resolve() as IMethod;
			
			codeFunction = new CodeFunction(codeModelContext, method);
		}
		
		[Test]
		public void Access_PublicFunction_ReturnsPublic()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateFunction_ReturnsPrivate()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    private void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void GetStartPoint_FunctionStartsAtColumnOne_ReturnsPointWithOffsetOne()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeFunction.GetStartPoint();
			
			int offset = point.LineCharOffset;
			Assert.AreEqual(1, offset);
		}
		
		[Test]
		public void GetStartPoint_FunctionStartsAtLine2_ReturnsPointWithLine2()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeFunction.GetStartPoint();
			int line = point.Line;
			
			Assert.AreEqual(2, line);
		}
		
		[Test]
		public void GetEndPoint_FunctionBodyEndsAtColumnTwo_ReturnsPointWithOffsetTwo()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction() {\r\n" +
			    "}\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(2, offset);
		}
		
		[Test]
		public void GetEndPoint_FunctionBodyEndsAtLine4_ReturnsPointWithLine4()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction()\r\n" +
				"    {\r\n" +
			    "    }\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
			int line = point.Line;
			
			Assert.AreEqual(4, line);
		}
		
		[Test]
		public void Kind_PublicFunction_ReturnsFunction()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMElement kind = codeFunction.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementFunction, kind);
		}
				
		[Test]
		public void GetEndPoint_InterfaceMethod_ReturnsMethodsDeclarationRegionEndNotBodyRegionEnd()
		{
			CreateFunction(
				"public interface Foo {\r\n" +
				"    void Bar();\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeFunction.GetEndPoint();
			
			Assert.AreEqual(2, point.Line);
			Assert.AreEqual(16, point.LineCharOffset);
		}
		
		[Test]
		public void Parameters_MethodHasNoParameters_ReturnsEmptyListOfItems()
		{
			CreateFunction(
				"public class Class1 {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeElements parameters = codeFunction.Parameters;
			
			Assert.AreEqual(0, parameters.Count);
		}
		
		[Test]
		public void Parameters_MethodHasOneParameter_ReturnsOneCodeParameter2()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyMethod(int test) {}\r\n" +
				"}");
			
			CodeParameter2 parameter = codeFunction.Parameters.FirstCodeParameter2OrDefault();
			
			Assert.AreEqual("test", parameter.Name);
		}
		
		[Test]
		public void Parameters_MethodHasOneStringParameter_ReturnsOneCodeParameterWithStringType()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public void MyMethod(string test) {}\r\n" +
				"}");
			
			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
			
			Assert.AreEqual("System.String", parameter.Type.AsFullName);
			Assert.AreEqual("string", parameter.Type.AsString);
		}
		
		[Test]
		public void Parameters_MethodHasOneStringParameter_CreatesCodeParameterWithContext()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyMethod(System.String test) {}\r\n" +
				"}");
			
			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
			global::EnvDTE.TextPoint textPoint = parameter.Type.CodeType.GetStartPoint();
			
			Assert.IsNotNull(textPoint);
		}
		
		[Test]
		public void Parameters_MethodHasOneStringParameter_CreatesCodeParameterWithCodeTypeRefThatHasParameterAsParent()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public void MyMethod(string test) {}\r\n" +
				"}");
			
			CodeParameter parameter = codeFunction.Parameters.FirstCodeParameterOrDefault();
			
			Assert.AreEqual(parameter, parameter.Type.Parent);
		}
		
		[Test]
		public void Type_MethodReturnsString_TypeRefHasSystemStringAsFullName()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public string MyMethod() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
			
			Assert.AreEqual("System.String", typeRef.AsFullName);
			Assert.AreEqual("string", typeRef.AsString);
		}
		
		[Test]
		public void Type_MethodReturnsString_TypeRefUsesProjectContentFromMethod()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public string MyMethod() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
			global::EnvDTE.TextPoint textPoint = typeRef.CodeType.GetStartPoint();
			
			Assert.IsNotNull(textPoint);
		}
		
		[Test]
		public void Type_MethodReturnsString_TypeRefParentIsCodeFunction()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public string MyMethod() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeTypeRef2 typeRef = codeFunction.Type;
			
			Assert.AreEqual(codeFunction, typeRef.Parent);
		}
		
		[Test]
		public void FunctionKind_ClassMethod_ReturnsFunctionKind()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyMethod() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMFunction kind = codeFunction.FunctionKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMFunction.vsCMFunctionFunction, kind);
		}
		
		[Test]
		public void FunctionKind_ClassConstructor_ReturnsConstructorKind()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public MyClass() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMFunction kind = codeFunction.FunctionKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMFunction.vsCMFunctionConstructor, kind);
		}
		
		[Test]
		public void IsShared_StaticMethod_ReturnsTrue()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public static void MyFunction() {}\r\n" +
				"}");
			
			bool shared = codeFunction.IsShared;
			
			Assert.IsTrue(shared);
		}
		
		[Test]
		public void IsShared_MethodIsNotStatic_ReturnsFalse()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			bool shared = codeFunction.IsShared;
			
			Assert.IsFalse(shared);
		}
		
		[Test]
		public void MustImplement_AbstractMethod_ReturnsTrue()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public abstract void MyFunction();\r\n" +
				"}");
			
			bool mustImplement = codeFunction.MustImplement;
			
			Assert.IsTrue(mustImplement);
		}
		
		[Test]
		public void MustImplement_MethodIsNotAbstract_ReturnsFalse()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			bool mustImplement = codeFunction.MustImplement;
			
			Assert.IsFalse(mustImplement);
		}
		
		[Test]
		public void CanOverride_MethodIsOverridable_ReturnsTrue()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public virtual void MyFunction() {}\r\n" +
				"}");
			
			bool canOverride = codeFunction.CanOverride;
			
			Assert.IsTrue(canOverride);
		}
		
		[Test]
		public void CanOverride_MethodIsNotAbstractOrVirtual_ReturnsFalse()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			bool canOverride = codeFunction.CanOverride;
			
			Assert.IsFalse(canOverride);
		}
		
		[Test]
		public void Attributes_MethodHasOneAttribute_ReturnsOneAttribute()
		{
			CreateFunction(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    [Obsolete]\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeElements attributes = codeFunction.Attributes;
			
			CodeAttribute2 attribute = attributes.FirstCodeAttribute2OrDefault();
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("System.ObsoleteAttribute", attribute.FullName);
		}
		
		[Test]
		public void CanOverride_SetToTrueForFunction_VirtualKeywordAddedToFunction()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			codeFunction.CanOverride = true;
			
			codeGenerator.AssertWasCalled(generator => generator.MakeVirtual(
				Arg<IMember>.Matches(member => member.Name == "MyFunction")));
		}
		
		[Test]
		public void CanOverride_SetToFalseForFunction_VirtualKeywordNotAddedToFunction()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			codeFunction.CanOverride = false;
			
			codeGenerator.AssertWasNotCalled(generator => generator.MakeVirtual(
				Arg<IMember>.Is.Anything));
		}
		
		[Test]
		public void CanOverride_SetToTrueForFunctionThatIsAlreadyVirtual_VirtualKeywordNotAddedToFunction()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public virtual void MyFunction() {}\r\n" +
				"}");
			
			codeFunction.CanOverride = true;
			
			codeGenerator.AssertWasNotCalled(generator => generator.MakeVirtual(
				Arg<IMember>.Is.Anything));
		}
	}
}
