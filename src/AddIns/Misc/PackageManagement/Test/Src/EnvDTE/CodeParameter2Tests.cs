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
//	public class CodeParameter2Tests
//	{
//		ParameterHelper helper;
//		CodeParameter2 parameter;
//		
//		[SetUp]
//		public void Init()
//		{
//			helper = new ParameterHelper();
//		}
//		
//		void CreateParameter()
//		{
//			parameter = new CodeParameter2(null, helper.Parameter);
//		}
//		
//		[Test]
//		public void ParameterKind_NormalParameter_ReturnsNone()
//		{
//			CreateParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindNone, kind);
//		}
//		
//		[Test]
//		public void ParameterKind_OptionalParameter_ReturnsOptional()
//		{
//			CreateParameter();
//			helper.MakeOptionalParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindOptional, kind);
//		}
//		
//		[Test]
//		public void ParameterKind_OutParameter_ReturnsOut()
//		{
//			CreateParameter();
//			helper.MakeOutParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindOut, kind);
//		}
//		
//		[Test]
//		public void ParameterKind_RefParameter_ReturnsRef()
//		{
//			CreateParameter();
//			helper.MakeRefParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindRef, kind);
//		}
//		
//		[Test]
//		public void ParameterKind_ParamArrayParameter_ReturnsParamArray()
//		{
//			CreateParameter();
//			helper.MakeParamArrayParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindParamArray, kind);
//		}
//		
//		[Test]
//		public void ParameterKind_InParameter_ReturnsIn()
//		{
//			CreateParameter();
//			helper.MakeInParameter();
//			
//			global::EnvDTE.vsCMParameterKind kind = parameter.ParameterKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMParameterKind.vsCMParameterKindIn, kind);
//		}
//	}
//}
