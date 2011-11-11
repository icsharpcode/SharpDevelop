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
		Empty templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Empty();
			templatePreprocessor.Host = mvcHost;
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsMyAppMyModel_ReturnsMyAppMyModelSurroundedByAngleBrackets()
		{
			CreateTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual("<MyApp.MyModel>", viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void TransformText_PartialViewUsingModel_ReturnsTypedUserControlUsingViewDataTypeName()
		{
			CreateTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			mvcHost.ViewDataTypeName = "MyNamespace.MyViewDataType";
			
			string output = templatePreprocessor.TransformText();
			
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<MyNamespace.MyViewDataType>"" %>

";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_PartialViewWithoutModel_ReturnsUntypedUserControl()
		{
			CreateTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			mvcHost.ViewDataTypeName = null;
			
			string output = templatePreprocessor.TransformText();
			
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl"" %>

";
			Assert.AreEqual(expectedOutput, output);
		}
	}
}
