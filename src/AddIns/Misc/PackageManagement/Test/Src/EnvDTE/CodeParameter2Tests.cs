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
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeParameter2Tests : CodeModelTestBase
	{
		CodeParameter2 parameter;
		
		void CreateParameter(string code)
		{
			AddCodeFile("class.cs", code);
			IMethod method = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Members
				.First()
				.Resolve() as IMethod;
			
			IParameter member =  method.Parameters.First();
			parameter = new CodeParameter2(codeModelContext, member);
		}
		
		[Test]
		public void ParameterKind_NormalParameter_ReturnsNone()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(int parameter) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindNone, kind);
		}
		
		[Test]
		public void ParameterKind_OptionalParameter_ReturnsOptional()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(int parameter = 0) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindOptional, kind);
		}
		
		[Test]
		public void ParameterKind_OutParameter_ReturnsOut()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(out int parameter) { parameter = 2; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindOut, kind);
		}
		
		[Test]
		public void ParameterKind_RefParameter_ReturnsRef()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(ref int parameter) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindRef, kind);
		}
		
		[Test]
		public void ParameterKind_ParamArrayParameter_ReturnsParamArray()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(params int[] parameters) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindParamArray, kind);
		}
		
		[Test]
		[Ignore("Not supported by NRefactory. Maps to VB.NET's ByVal. For C# vsCMParameterKindNone is returned.")]
		public void ParameterKind_InParameter_ReturnsIn()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(int parameter) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindIn, kind);
		}
		
		[Test]
		public void Kind_Parameter_ReturnsParameter()
		{
			CreateParameter(
				"public class MyClass {\r\n" +
				"    public void MyMethod(int parameter) {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMElement kind = parameter.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementParameter, kind);
		}
		
		[Test]
		public void Attributes_ParameterHasOneAttribute_ReturnsOneAttribute()
		{
			CreateParameter(
				"using System;\r\n" +
				"public class MyClass {\r\n" +
				"    public void MyMethod([Obsolete] int parameter) {}\r\n" +
				"}");
			
			global::EnvDTE.CodeElements attributes = parameter.Attributes;
			
			CodeAttribute2 attribute = parameter.Attributes.FirstCodeAttribute2OrDefault();
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("System.ObsoleteAttribute", attribute.FullName);
		}
	}
}
