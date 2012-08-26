// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeParameter2Tests
	{
		ParameterHelper helper;
		CodeParameter2 parameter;
		
		[SetUp]
		public void Init()
		{
			helper = new ParameterHelper();
		}
		
		void CreateParameter()
		{
			parameter = new CodeParameter2(null, helper.Parameter);
		}
		
		[Test]
		public void ParameterKind_NormalParameter_ReturnsNone()
		{
			CreateParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindNone, kind);
		}
		
		[Test]
		public void ParameterKind_OptionalParameter_ReturnsOptional()
		{
			CreateParameter();
			helper.MakeOptionalParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindOptional, kind);
		}
		
		[Test]
		public void ParameterKind_OutParameter_ReturnsOut()
		{
			CreateParameter();
			helper.MakeOutParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindOut, kind);
		}
		
		[Test]
		public void ParameterKind_RefParameter_ReturnsRef()
		{
			CreateParameter();
			helper.MakeRefParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindRef, kind);
		}
		
		[Test]
		public void ParameterKind_ParamArrayParameter_ReturnsParamArray()
		{
			CreateParameter();
			helper.MakeParamArrayParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindParamArray, kind);
		}
		
		[Test]
		public void ParameterKind_InParameter_ReturnsIn()
		{
			CreateParameter();
			helper.MakeInParameter();
			
			vsCMParameterKind kind = parameter.ParameterKind;
			
			Assert.AreEqual(vsCMParameterKind.vsCMParameterKindIn, kind);
		}
	}
}
