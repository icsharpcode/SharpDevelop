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
	public class CodeFunction2Tests : CodeModelTestBase
	{
		CodeFunction2 codeFunction;
		
		void CreateFunction(string code)
		{
			AddCodeFile("class.cs", code);
			
			IMethod method = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Members
				.First()
				.Resolve() as IMethod;
			
			codeFunction = new CodeFunction2(codeModelContext, method);
		}
		
		[Test]
		public void OverrideKind_OrdinaryMethod_ReturnsNone()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNone, kind);
		}
		
		[Test]
		public void OverrideKind_AbstractMethod_ReturnsAbstract()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public abstract void MyFunction();\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract, kind);
		}
		
		[Test]
		public void OverrideKind_VirtualMethod_ReturnsVirtual()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public virtual void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual, kind);
		}
		
		[Test]
		public void OverrideKind_MethodOverride_ReturnsOverride()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public override string ToString() { return \"MyClass\"; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride, kind);
		}
		
		[Test]
		public void OverrideKind_SealedMethod_ReturnsSealed()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public sealed void MyFunction() {}\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed, kind);
		}
		
		[Test]
		public void OverrideKind_MethodHiddenByNewKeyword_ReturnsNew()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public new string ToString() { return \"MyClass\"; }\r\n" +
				"}");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew, kind);
		}
		
		[Test]
		public void IsGeneric_MethodHasTypeParameter_ReturnsTrue()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction<TResult>() {}\r\n" +
				"}");
			
			bool generic = codeFunction.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void IsGeneric_MethodHasNoTypeParameters_ReturnsFalse()
		{
			CreateFunction(
				"public class MyClass {\r\n" +
				"    public void MyFunction() {}\r\n" +
				"}");
			
			bool generic = codeFunction.IsGeneric;
			
			Assert.IsFalse(generic);
		}
	}
}
