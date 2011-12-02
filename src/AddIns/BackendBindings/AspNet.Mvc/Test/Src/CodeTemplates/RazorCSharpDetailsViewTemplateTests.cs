// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.CSHtml;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class RazorCSharpDetailsViewTemplateTests
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
		public void GetModelDirective_HostViewDataTypeNameIsMyAppMyModel_ReturnsRazorModelFollowedByMyAppMyModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string modelDirective = templatePreprocessor.GetModelDirective();
			
			Assert.AreEqual("@model MyApp.MyModel", modelDirective);
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
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties

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
			@Html.ActionLink(""Edit"", ""Edit"") |
			@Html.ActionLink(""Back"", ""Index"")
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
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties

<fieldset>
	<legend>ModelWithNoProperties</legend>
</fieldset>
<p>
	@Html.ActionLink(""Edit"", ""Edit"") |
	@Html.ActionLink(""Back"", ""Index"")
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
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties

@{
	ViewBag.Title = ""MyView"";
	Layout = ""~/Views/Shared/Site.master"";
}

<h2>MyView</h2>

<fieldset>
	<legend>ModelWithNoProperties</legend>
</fieldset>
<p>
	@Html.ActionLink(""Edit"", ""Edit"") |
	@Html.ActionLink(""Back"", ""Index"")
</p>
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
		public void TransformText_ModelHasOnePropertyAndIsPartialView_ReturnsControlWithHtmlHelperForModelProperty()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithOneProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithOneProperty

<fieldset>
	<legend>ModelWithOneProperty</legend>
	
	<div class=""display-label"">
		@Html.LabelFor(model => model.Name)
	</div>
	<div class=""display-field"">
		@Html.DisplayFor(model => model.Name)
	</div>
</fieldset>
<p>
	@Html.ActionLink(""Edit"", ""Edit"") |
	@Html.ActionLink(""Back"", ""Index"")
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasTwoPropertiesAndIsPartialView_ReturnsControlWithHtmlHelperForModelProperties()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithTwoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithTwoProperties

<fieldset>
	<legend>ModelWithTwoProperties</legend>
	
	<div class=""display-label"">
		@Html.LabelFor(model => model.FirstName)
	</div>
	<div class=""display-field"">
		@Html.DisplayFor(model => model.FirstName)
	</div>
	
	<div class=""display-label"">
		@Html.LabelFor(model => model.LastName)
	</div>
	<div class=""display-field"">
		@Html.DisplayFor(model => model.LastName)
	</div>
</fieldset>
<p>
	@Html.ActionLink(""Edit"", ""Edit"") |
	@Html.ActionLink(""Back"", ""Index"")
</p>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasIdPropertyAndIsPartialView_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithIdProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"@model AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithIdProperty

<fieldset>
	<legend>ModelWithIdProperty</legend>
	
	<div class=""display-label"">
		@Html.LabelFor(model => model.Name)
	</div>
	<div class=""display-field"">
		@Html.DisplayFor(model => model.Name)
	</div>
</fieldset>
<p>
	@Html.ActionLink(""Edit"", ""Edit"", new { id = Model.Id }) |
	@Html.ActionLink(""Back"", ""Index"")
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
