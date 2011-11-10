// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.CSHtml;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class RazorCSharpEmptyViewTemplateTests
	{
		Empty emptyViewTemplate;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateEmptyViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			emptyViewTemplate = new Empty();
			emptyViewTemplate.Host = mvcHost;
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsMyAppMyModel_ReturnsRazorModelFollowedByMyAppMyModel()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string modelDirective = emptyViewTemplate.GetModelDirective();
			
			Assert.AreEqual("@model MyApp.MyModel", modelDirective);
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string modelDirective = emptyViewTemplate.GetModelDirective();
			
			Assert.AreEqual(String.Empty, modelDirective);
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string modelDirective = emptyViewTemplate.GetModelDirective();
			
			Assert.AreEqual(String.Empty, modelDirective);
		}

	}
}
