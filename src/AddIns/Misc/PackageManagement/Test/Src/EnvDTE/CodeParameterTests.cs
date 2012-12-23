// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeParameterTests
	{
		ParameterHelper helper;
		CodeParameter parameter;
		
		[SetUp]
		public void Init()
		{
			helper = new ParameterHelper();
		}
		
		void CreateParameter()
		{
			parameter = new CodeParameter(null, helper.Parameter);
		}
		
		[Test]
		public void Kind_Parameter_ReturnsParameter()
		{
			CreateParameter();
			
			global::EnvDTE.vsCMElement kind = parameter.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementParameter, kind);
		}
		
		[Test]
		public void Attributes_ParameterHasOneAttribute_ReturnsOneAttribute()
		{
			CreateParameter();
			helper.AddAttributeToParameter("System.Web.Mvc.BindAttribute");
			
			global::EnvDTE.CodeElements attributes = parameter.Attributes;
			
			CodeAttribute2 attribute = parameter.Attributes.FirstCodeAttribute2OrDefault();
			Assert.AreEqual(1, attributes.Count);
			Assert.AreEqual("System.Web.Mvc.BindAttribute", attribute.FullName);
		}
	}
}
