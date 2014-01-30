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
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeFunction2Tests
//	{
//		CodeFunction2 codeFunction;
//		MethodHelper helper;
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
//		void CreateFunction()
//		{
//			codeFunction = new CodeFunction2(helper.Method);
//		}
//		
//		[Test]
//		public void OverrideKind_OrdinaryMethod_ReturnsNone()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNone, kind);
//		}
//		
//		[Test]
//		public void OverrideKind_AbstractMethod_ReturnsAbstract()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.MakeMethodAbstract();
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract, kind);
//		}
//		
//		[Test]
//		public void OverrideKind_VirtualMethod_ReturnsVirtual()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.MakeMethodVirtual();
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual, kind);
//		}
//		
//		[Test]
//		public void OverrideKind_MethodOverride_ReturnsOverride()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.MakeMethodOverride();
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride, kind);
//		}
//		
//		[Test]
//		public void OverrideKind_SealedMethod_ReturnsSealed()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.MakeMethodSealed();
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed, kind);
//		}
//		
//		[Test]
//		public void OverrideKind_MethodHiddenByNewKeyword_ReturnsNew()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.MakeMethodNewOverride();
//			
//			global::EnvDTE.vsCMOverrideKind kind = codeFunction.OverrideKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew, kind);
//		}
//		
//		[Test]
//		public void IsGeneric_MethodHasTypeParameter_ReturnsTrue()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.AddTypeParameter("TResult");
//			
//			bool generic = codeFunction.IsGeneric;
//			
//			Assert.IsTrue(generic);
//		}
//		
//		[Test]
//		public void IsGeneric_MethodHasTypeParameters_ReturnsFalse()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			helper.NoTypeParameters();
//			
//			bool generic = codeFunction.IsGeneric;
//			
//			Assert.IsFalse(generic);
//		}
//		
//		[Test]
//		public void IsGeneric_MethodTypeParametersIsNull_ReturnsFalse()
//		{
//			CreatePublicFunction("MyClass.MyFunction");
//			
//			bool generic = codeFunction.IsGeneric;
//			
//			Assert.IsFalse(generic);
//		}
//	}
//}
