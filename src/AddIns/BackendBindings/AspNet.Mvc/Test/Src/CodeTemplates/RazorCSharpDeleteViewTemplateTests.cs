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
	public class RazorCSharpDeleteViewTemplateTests
	{
		Delete templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Delete();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<Delete.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		Delete.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		Delete.ModelProperty GetModelProperty(string name)
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
		@using (Html.BeginForm()) {
			<p>
				<input type=""submit"" value=""Delete""/> |
				@Html.ActionLink(""Back"", ""Index"")
			</p>
		}
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
@using (Html.BeginForm()) {
	<p>
		<input type=""submit"" value=""Delete""/> |
		@Html.ActionLink(""Back"", ""Index"")
	</p>
}
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
@using (Html.BeginForm()) {
	<p>
		<input type=""submit"" value=""Delete""/> |
		@Html.ActionLink(""Back"", ""Index"")
	</p>
}
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			Delete.ModelProperty modelProperty = GetFirstModelProperty();
			
			Assert.AreEqual("Name", modelProperty.Name);
		}
		
		[Test]
		public void TransformText_ModelHasOnePropertyAndIsPartialView_ReturnsControlWithHtmlHelpersForModelProperty()
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
@using (Html.BeginForm()) {
	<p>
		<input type=""submit"" value=""Delete""/> |
		@Html.ActionLink(""Back"", ""Index"")
	</p>
}
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasTwoPropertiesAndIsPartialView_ReturnsControlWithHtmlHelpersForModelProperties()
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
@using (Html.BeginForm()) {
	<p>
		<input type=""submit"" value=""Delete""/> |
		@Html.ActionLink(""Back"", ""Index"")
	</p>
}
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasIdProperty_NoHtmlHelpersGeneratedForModelIdProperty()
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
@using (Html.BeginForm()) {
	<p>
		<input type=""submit"" value=""Delete""/> |
		@Html.ActionLink(""Back"", ""Index"")
	</p>
}
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Delete.ModelProperty modelProperty = GetModelProperty("Id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_NamePropertyIsNotMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Delete.ModelProperty modelProperty = GetModelProperty("Name");
			
			Assert.IsFalse(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdPropertyInLowerCase_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			Delete.ModelProperty modelProperty = GetModelProperty("id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasPrefixedIdPropertyInLowerCase_PrefixedIdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithPrefixedIdPropertyInLowerCase);
			
			Delete.ModelProperty modelProperty = GetModelProperty("modelwithprefixedidpropertyinlowercaseid");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
	}
}
