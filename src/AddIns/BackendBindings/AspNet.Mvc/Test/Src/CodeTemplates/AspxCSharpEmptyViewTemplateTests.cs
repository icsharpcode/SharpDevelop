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
