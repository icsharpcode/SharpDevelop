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
using NUnit.Framework;
using AspxCSharp = ICSharpCode.AspNet.Mvc.AspxCSharp;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class AspxCSharpListViewTemplateTests
	{
		AspxCSharp.List templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new AspxCSharp.List();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<AspxCSharp.List.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		AspxCSharp.List.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		AspxCSharp.List.ModelProperty GetModelProperty(string name)
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
@"<%@ Page Language=""C#"" Inherits=""System.Web.Mvc.ViewPage<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>>"" %>

<!DOCTYPE html>
<html>
	<head runat=""server"">
		<title>MyView</title>
	</head>
	<body>
		<p>
			<%: Html.ActionLink(""Create"", ""Create"") %>
		</p>
		<table>
		<% foreach (var item in Model) { %>
			<tr>
				<td>
					<%: Html.ActionLink(""Edit"", ""Edit"") %> |
					<%: Html.ActionLink(""Details"", ""Details"") %> |
					<%: Html.ActionLink(""Delete"", ""Delete"") %>
				</td>
			</tr>
		<% } %>
		</table>
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
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>>"" %>

<p>
	<%: Html.ActionLink(""Create"", ""Create"") %>
</p>
<table>
<% foreach (var item in Model) { %>
	<tr>
		<td>
			<%: Html.ActionLink(""Edit"", ""Edit"") %> |
			<%: Html.ActionLink(""Details"", ""Details"") %> |
			<%: Html.ActionLink(""Delete"", ""Delete"") %>
		</td>
	</tr>
<% } %>
</table>
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
@"<%@ Page Language=""C#"" MasterPageFile=""~/Views/Shared/Site.master"" Inherits=""System.Web.Mvc.ViewPage<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>>"" %>

<asp:Content ID=""Content1"" ContentPlaceHolderID=""Title"" runat=""server"">
MyView
</asp:Content>

<asp:Content ID=""Content2"" ContentPlaceHolderID=""Main"" runat=""server"">
	<p>
		<%: Html.ActionLink(""Create"", ""Create"") %>
	</p>
	<table>
	<% foreach (var item in Model) { %>
		<tr>
			<td>
				<%: Html.ActionLink(""Edit"", ""Edit"") %> |
				<%: Html.ActionLink(""Details"", ""Details"") %> |
				<%: Html.ActionLink(""Delete"", ""Delete"") %>
			</td>
		</tr>
	<% } %>
	</table>
</asp:Content>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			AspxCSharp.List.ModelProperty modelProperty = GetFirstModelProperty();
			
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
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithOneProperty>>"" %>

<p>
	<%: Html.ActionLink(""Create"", ""Create"") %>
</p>
<table>
	<tr>
		<th>
			<%: Html.LabelFor(model => model.Name) %>
		</th>
		<th></th>
	</tr>
	
<% foreach (var item in Model) { %>
	<tr>
		<td>
			<%: Html.DisplayFor(model => model.Name) %>
		</td>
		<td>
			<%: Html.ActionLink(""Edit"", ""Edit"") %> |
			<%: Html.ActionLink(""Details"", ""Details"") %> |
			<%: Html.ActionLink(""Delete"", ""Delete"") %>
		</td>
	</tr>
<% } %>
</table>
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
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithTwoProperties>>"" %>

<p>
	<%: Html.ActionLink(""Create"", ""Create"") %>
</p>
<table>
	<tr>
		<th>
			<%: Html.LabelFor(model => model.FirstName) %>
		</th>
		<th>
			<%: Html.LabelFor(model => model.LastName) %>
		</th>
		<th></th>
	</tr>
	
<% foreach (var item in Model) { %>
	<tr>
		<td>
			<%: Html.DisplayFor(model => model.FirstName) %>
		</td>
		<td>
			<%: Html.DisplayFor(model => model.LastName) %>
		</td>
		<td>
			<%: Html.ActionLink(""Edit"", ""Edit"") %> |
			<%: Html.ActionLink(""Details"", ""Details"") %> |
			<%: Html.ActionLink(""Delete"", ""Delete"") %>
		</td>
	</tr>
<% } %>
</table>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasIdPropertyAndIsPartialView_UsesIdPropertyInActionLinksOnTableRow()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithIdProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithIdProperty>>"" %>

<p>
	<%: Html.ActionLink(""Create"", ""Create"") %>
</p>
<table>
	<tr>
		<th>
			<%: Html.LabelFor(model => model.Name) %>
		</th>
		<th></th>
	</tr>
	
<% foreach (var item in Model) { %>
	<tr>
		<td>
			<%: Html.DisplayFor(model => model.Name) %>
		</td>
		<td>
			<%: Html.ActionLink(""Edit"", ""Edit"", new { id = item.Id }) %> |
			<%: Html.ActionLink(""Details"", ""Details"", new { id = item.Id }) %> |
			<%: Html.ActionLink(""Delete"", ""Delete"", new { id = item.Id }) %>
		</td>
	</tr>
<% } %>
</table>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			AspxCSharp.List.ModelProperty modelProperty = GetModelProperty("Id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_NamePropertyIsNotMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			AspxCSharp.List.ModelProperty modelProperty = GetModelProperty("Name");
			
			Assert.IsFalse(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdPropertyInLowerCase_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			AspxCSharp.List.ModelProperty modelProperty = GetModelProperty("id");
			
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
			
			AspxCSharp.List.ModelProperty modelProperty = GetModelProperty("modelwithprefixedidpropertyinlowercaseid");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
	}
}
