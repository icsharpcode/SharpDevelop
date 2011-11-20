// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.CSHtml;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class RazorCSharpEditViewTemplateTests
	{
		Edit templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Edit();
			templatePreprocessor.Host = mvcHost;
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
					<input type=""submit"" value=""Save""/>
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
			<input type=""submit"" value=""Save""/>
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
			<input type=""submit"" value=""Save""/>
		</p>
	</fieldset>
}
<div>
	@Html.ActionLink(""Back"", ""Index"")
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
	}
}
