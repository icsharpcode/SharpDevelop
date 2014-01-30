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
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.AspxCSharp;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class AspxCSharpDetailsViewTemplateTests
	{
		Details templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Details();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<Details.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		Details.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		Details.ModelProperty GetModelProperty(string name)
		{
			return GetModelProperties().First(p => p.Name == name);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsMyAppMyModel_ReturnsMyAppMyModelSurroundedByAngleBrackets()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual("<MyApp.MyModel>", viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndNoMasterPage_ReturnsFullHtmlPageWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Page Language=""C#"" Inherits=""System.Web.Mvc.ViewPage<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<!DOCTYPE html>
<html>
	<head runat=""server"">
		<title>MyView</title>
	</head>
	<body>
		<fieldset>
			<legend>ModelWithNoProperties</legend>
		</fieldset>
		<p>
			<%: Html.ActionLink(""Edit"", ""Edit"") %> |
			<%: Html.ActionLink(""Back"", ""Index"") %>
		</p>
	</body>
</html>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndIsPartialView_ReturnsControlWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<fieldset>
	<legend>ModelWithNoProperties</legend>
</fieldset>
<p>
	<%: Html.ActionLink(""Edit"", ""Edit"") %> |
	<%: Html.ActionLink(""Back"", ""Index"") %>
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndIsContentPage_ReturnsContentPageWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsContentPage = true;
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			mvcHost.MasterPageFile = "~/Views/Shared/Site.master";
			mvcHost.PrimaryContentPlaceHolderID = "Main";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Page Language=""C#"" MasterPageFile=""~/Views/Shared/Site.master"" Inherits=""System.Web.Mvc.ViewPage<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<asp:Content ID=""Content1"" ContentPlaceHolderID=""Title"" runat=""server"">
MyView
</asp:Content>

<asp:Content ID=""Content2"" ContentPlaceHolderID=""Main"" runat=""server"">
	<fieldset>
		<legend>ModelWithNoProperties</legend>
	</fieldset>
	<p>
		<%: Html.ActionLink(""Edit"", ""Edit"") %> |
		<%: Html.ActionLink(""Back"", ""Index"") %>
	</p>
</asp:Content>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			Details.ModelProperty modelProperty = GetFirstModelProperty();
			
			Assert.AreEqual("Name", modelProperty.Name);
		}
		
		[Test]
		public void TransformText_ModelHasOnePropertyAndIsPartialView_ReturnsControlWithFormAndHtmlHelpersForModelProperty()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithOneProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithOneProperty>"" %>

<fieldset>
	<legend>ModelWithOneProperty</legend>
	
	<div class=""display-label"">
		<%: Html.LabelFor(model => model.Name) %>
	</div>
	<div class=""display-field"">
		<%: Html.DisplayFor(model => model.Name) %>
	</div>
</fieldset>
<p>
	<%: Html.ActionLink(""Edit"", ""Edit"") %> |
	<%: Html.ActionLink(""Back"", ""Index"") %>
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasTwoPropertiesAndIsPartialView_ReturnsControlWithFormAndHtmlHelpersForModelProperty()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithTwoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithTwoProperties>"" %>

<fieldset>
	<legend>ModelWithTwoProperties</legend>
	
	<div class=""display-label"">
		<%: Html.LabelFor(model => model.FirstName) %>
	</div>
	<div class=""display-field"">
		<%: Html.DisplayFor(model => model.FirstName) %>
	</div>
	
	<div class=""display-label"">
		<%: Html.LabelFor(model => model.LastName) %>
	</div>
	<div class=""display-field"">
		<%: Html.DisplayFor(model => model.LastName) %>
	</div>
</fieldset>
<p>
	<%: Html.ActionLink(""Edit"", ""Edit"") %> |
	<%: Html.ActionLink(""Back"", ""Index"") %>
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasIdProperty_EditActionLinkUsesModelIdProperty()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithIdProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithIdProperty>"" %>

<fieldset>
	<legend>ModelWithIdProperty</legend>
	
	<div class=""display-label"">
		<%: Html.LabelFor(model => model.Name) %>
	</div>
	<div class=""display-field"">
		<%: Html.DisplayFor(model => model.Name) %>
	</div>
</fieldset>
<p>
	<%: Html.ActionLink(""Edit"", ""Edit"", new { id = Model.Id }) %> |
	<%: Html.ActionLink(""Back"", ""Index"") %>
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Details.ModelProperty modelProperty = GetModelProperty("Id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_NamePropertyIsNotMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Details.ModelProperty modelProperty = GetModelProperty("Name");
			
			Assert.IsFalse(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdPropertyInLowerCase_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			Details.ModelProperty modelProperty = GetModelProperty("id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelPrimaryKeyName_ModelHasIdAndNameProperty_ReturnsId()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			string primaryKeyName = templatePreprocessor.GetModelPrimaryKeyName();
			
			Assert.AreEqual("Id", primaryKeyName);
		}
		
		[Test]
		public void GetModelPrimaryKeyName_ModelHasIdPropertyInLowerCase_ReturnsLowercaseId()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			string primaryKeyName = templatePreprocessor.GetModelPrimaryKeyName();
			
			Assert.AreEqual("id", primaryKeyName);
		}
		
		[Test]
		public void GetModelPrimaryKeyName_ModelHasNameProperty_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			string primaryKeyName = templatePreprocessor.GetModelPrimaryKeyName();
			
			Assert.AreEqual(String.Empty, primaryKeyName);
		}
		
		[Test]
		public void GetModelProperties_ModelHasPrefixedIdPropertyInLowerCase_PrefixedIdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithPrefixedIdPropertyInLowerCase);
			
			Details.ModelProperty modelProperty = GetModelProperty("modelwithprefixedidpropertyinlowercaseid");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
	}
}
