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
	public class RazorCSharpCreateViewTemplateTests
	{
		Create templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Create();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<Create.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		Create.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		Create.ModelProperty GetModelProperty(string name)
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
		@using (Html.BeginForm()) {
			@Html.ValidationSummary(true)
			<fieldset>
				<legend>ModelWithNoProperties</legend>
				
				<p>
					<input type=""submit"" value=""Create""/>
				</p>
			</fieldset>
		}
		<div>
			@Html.ActionLink(""Back"", ""Index"")
		</div>
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

@using (Html.BeginForm()) {
	@Html.ValidationSummary(true)
	<fieldset>
		<legend>ModelWithNoProperties</legend>
		
		<p>
			<input type=""submit"" value=""Create""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
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

@using (Html.BeginForm()) {
	@Html.ValidationSummary(true)
	<fieldset>
		<legend>ModelWithNoProperties</legend>
		
		<p>
			<input type=""submit"" value=""Create""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			Create.ModelProperty modelProperty = GetFirstModelProperty();
			
			Assert.AreEqual("Name", modelProperty.Name);
		}
		
		[Test]
		public void TransformText_ModelHasOnePropertyAndIsPartialView_ReturnsControlWithHtmlEditorsForModelProperties()
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

@using (Html.BeginForm()) {
	@Html.ValidationSummary(true)
	<fieldset>
		<legend>ModelWithOneProperty</legend>
		
		<div class=""editor-label"">
			@Html.LabelFor(model => model.Name)
		</div>
		<div class=""editor-field"">
			@Html.EditorFor(model => model.Name)
			@Html.ValidationMessageFor(model => model.Name)
		</div>
		
		<p>
			<input type=""submit"" value=""Create""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasTwoPropertiesAndIsPartialView_ReturnsControlWithHtmlEditorsForModelProperties()
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

@using (Html.BeginForm()) {
	@Html.ValidationSummary(true)
	<fieldset>
		<legend>ModelWithTwoProperties</legend>
		
		<div class=""editor-label"">
			@Html.LabelFor(model => model.FirstName)
		</div>
		<div class=""editor-field"">
			@Html.EditorFor(model => model.FirstName)
			@Html.ValidationMessageFor(model => model.FirstName)
		</div>
		
		<div class=""editor-label"">
			@Html.LabelFor(model => model.LastName)
		</div>
		<div class=""editor-field"">
			@Html.EditorFor(model => model.LastName)
			@Html.ValidationMessageFor(model => model.LastName)
		</div>
		
		<p>
			<input type=""submit"" value=""Create""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasIdPropertyAndIsPartialView_ReturnsControlWithHtmlEditorsForNonIdModelProperty()
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

@using (Html.BeginForm()) {
	@Html.ValidationSummary(true)
	<fieldset>
		<legend>ModelWithIdProperty</legend>
		
		<div class=""editor-label"">
			@Html.LabelFor(model => model.Name)
		</div>
		<div class=""editor-field"">
			@Html.EditorFor(model => model.Name)
			@Html.ValidationMessageFor(model => model.Name)
		</div>
		
		<p>
			<input type=""submit"" value=""Create""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Create.ModelProperty modelProperty = GetModelProperty("Id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdAndNameProperty_NamePropertyIsNotMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdProperty);
			
			Create.ModelProperty modelProperty = GetModelProperty("Name");
			
			Assert.IsFalse(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasIdPropertyInLowerCase_IdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithIdPropertyInLowerCase);
			
			Create.ModelProperty modelProperty = GetModelProperty("id");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
		
		[Test]
		public void GetModelProperties_ModelHasPrefixedIdPropertyInLowerCase_PrefixedIdPropertyIsMarkedAsPrimaryKey()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithPrefixedIdPropertyInLowerCase);
			
			Create.ModelProperty modelProperty = GetModelProperty("modelwithprefixedidpropertyinlowercaseid");
			
			Assert.IsTrue(modelProperty.IsPrimaryKey);
		}
	}
}
