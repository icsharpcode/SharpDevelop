// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.AspxCSharp;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class AspxCSharpEmptyViewTemplateTests
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
		public void GetViewPageType_HostViewDataTypeNameIsMyAppMyModel_ReturnsMyAppMyModelSurroundedByAngleBrackets()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string viewPageType = emptyViewTemplate.GetViewPageType();
			
			Assert.AreEqual("<MyApp.MyModel>", viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string viewPageType = emptyViewTemplate.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateEmptyViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string viewPageType = emptyViewTemplate.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
	}
}
