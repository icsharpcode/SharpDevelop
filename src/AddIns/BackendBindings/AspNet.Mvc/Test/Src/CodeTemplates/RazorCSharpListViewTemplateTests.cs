// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using NUnit.Framework;
using CSHtml = ICSharpCode.AspNet.Mvc.CSHtml;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class RazorCSharpListViewTemplateTests
	{
		CSHtml.List templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new CSHtml.List();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<CSHtml.List.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		CSHtml.List.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		CSHtml.List.ModelProperty GetModelProperty(string name)
		{
			return GetModelProperties().First(p => p.Name == name);
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsMyAppMyModel_ReturnsRazorModelFollowedByIEnumerableMyAppMyModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string modelDirective = templatePreprocessor.GetModelDirective();
			
			Assert.AreEqual("@model IEnumerable<MyApp.MyModel>", modelDirective);
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string modelDirective = templatePreprocessor.GetModelDirective();
			
			Assert.AreEqual(String.Empty, modelDirective);
		}
		
		[Test]
		public void GetModelDirective_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string modelDirective = templatePreprocessor.GetModelDirective();
			
			Assert.AreEqual(String.Empty, modelDirective);
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
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>

<!DOCTYPE html>
<html>
	<head runat=""server"">
		<title>MyView</title>
	</head>
	<body>
		<p>
			@Html.ActionLink(""Create"", ""Create"")
		</p>
		<table>
		@foreach (var item in Model) {
			<tr>
				<td>
					@Html.ActionLink(""Edit"", ""Edit"") |
					@Html.ActionLink(""Details"", ""Details"") |
					@Html.ActionLink(""Delete"", ""Delete"")
				</td>
			</tr>
		}
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
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>

<p>
	@Html.ActionLink(""Create"", ""Create"")
</p>
<table>
@foreach (var item in Model) {
	<tr>
		<td>
			@Html.ActionLink(""Edit"", ""Edit"") |
			@Html.ActionLink(""Details"", ""Details"") |
			@Html.ActionLink(""Delete"", ""Delete"")
		</td>
	</tr>
}
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
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>

@{
	ViewBag.Title = ""MyView"";
	Layout = ""~/Views/Shared/Site.master"";
}

<h2>MyView</h2>

<p>
	@Html.ActionLink(""Create"", ""Create"")
</p>
<table>
@foreach (var item in Model) {
	<tr>
		<td>
			@Html.ActionLink(""Edit"", ""Edit"") |
			@Html.ActionLink(""Details"", ""Details"") |
			@Html.ActionLink(""Delete"", ""Delete"")
		</td>
	</tr>
}
</table>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			CSHtml.List.ModelProperty modelProperty = GetFirstModelProperty();
			
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
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithOneProperty>

<p>
	@Html.ActionLink(""Create"", ""Create"")
</p>
<table>
	<tr>
		<th>
			@Html.LabelFor(model => model.Name)
		</th>
		<th></th>
	</tr>
	
@foreach (var item in Model) {
	<tr>
		<td>
			@Html.DisplayFor(model => model.Name)
		</td>
		<td>
			@Html.ActionLink(""Edit"", ""Edit"") |
			@Html.ActionLink(""Details"", ""Details"") |
			@Html.ActionLink(""Delete"", ""Delete"")
		</td>
	</tr>
}
</table>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasTwoPropertiesAndIsPartialView_ReturnsControlWithFormAndHtmlHelpersForModelProperties()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithTwoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithTwoProperties>

<p>
	@Html.ActionLink(""Create"", ""Create"")
</p>
<table>
	<tr>
		<th>
			@Html.LabelFor(model => model.FirstName)
		</th>
		<th>
			@Html.LabelFor(model => model.LastName)
		</th>
		<th></th>
	</tr>
	
@foreach (var item in Model) {
	<tr>
		<td>
			@Html.DisplayFor(model => model.FirstName)
		</td>
		<td>
			@Html.DisplayFor(model => model.LastName)
		</td>
		<td>
			@Html.ActionLink(""Edit"", ""Edit"") |
			@Html.ActionLink(""Details"", ""Details"") |
			@Html.ActionLink(""Delete"", ""Delete"")
		</td>
	</tr>
}
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
@"@model IEnumerable<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithIdProperty>

<p>
	@Html.ActionLink(""Create"", ""Create"")
</p>
<table>
	<tr>
		<th>
			@Html.LabelFor(model => model.Name)
		</th>
		<th></th>
	</tr>
	
@foreach (var item in Model) {
	<tr>
		<td>
			@Html.DisplayFor(model => model.Name)
		</td>
		<td>
			@Html.ActionLink(""Edit"", ""Edit"", new { id = item.Id }) |
			@Html.ActionLink(""Details"", ""Details"", new { id = item.Id }) |
			@Html.ActionLink(""Delete"", ""Delete"", new { id = item.Id })
		</td>
	</tr>
}
</table>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			CSHtml.List.ModelProperty modelProperty = GetModelProperty("Id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_NamePropertyIsNotMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			CSHtml.List.ModelProperty modelProperty = GetModelProperty("Name");
			
			Assert.IsFalse(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdPropertyInLowerCase_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			CSHtml.List.ModelProperty modelProperty = GetModelProperty("id");
			
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
			
			CSHtml.List.ModelProperty modelProperty = GetModelProperty("modelwithprefixedidpropertyinlowercaseid");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
	}
}
