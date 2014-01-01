// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
