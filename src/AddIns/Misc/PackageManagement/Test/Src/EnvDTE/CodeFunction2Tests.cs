// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeFunction2Tests
	{
		CodeFunction2 codeFunction;
		MethodHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new MethodHelper();
		}
		
		void CreatePublicFunction(string name)
		{
			helper.CreatePublicMethod(name);
			CreateFunction();
		}
		
		void CreateFunction()
		{
			codeFunction = new CodeFunction2(helper.Method);
		}
		
		[Test]
		public void OverrideKind_OrdinaryMethod_ReturnsNone()
		{
			CreatePublicFunction("MyClass.MyFunction");
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNone, kind);
		}
		
		[Test]
		public void OverrideKind_AbstractMethod_ReturnsAbstract()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.MakeMethodAbstract();
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract, kind);
		}
		
		[Test]
		public void OverrideKind_VirtualMethod_ReturnsVirtual()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.MakeMethodVirtual();
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual, kind);
		}
		
		[Test]
		public void OverrideKind_MethodOverride_ReturnsOverride()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.MakeMethodOverride();
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride, kind);
		}
		
		[Test]
		public void OverrideKind_SealedMethod_ReturnsSealed()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.MakeMethodSealed();
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed, kind);
		}
		
		[Test]
		public void OverrideKind_MethodHiddenByNewKeyword_ReturnsNew()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.MakeMethodNewOverride();
			
			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew, kind);
		}
		
		[Test]
		public void IsGeneric_MethodHasTypeParameter_ReturnsTrue()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.AddTypeParameter("TResult");
			
			bool generic = codeFunction.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void IsGeneric_MethodHasTypeParameters_ReturnsFalse()
		{
			CreatePublicFunction("MyClass.MyFunction");
			helper.NoTypeParameters();
			
			bool generic = codeFunction.IsGeneric;
			
			Assert.IsFalse(generic);
		}
		
		[Test]
		public void IsGeneric_MethodTypeParametersIsNull_ReturnsFalse()
		{
			CreatePublicFunction("MyClass.MyFunction");
			
			bool generic = codeFunction.IsGeneric;
			
			Assert.IsFalse(generic);
		}
	}
}
