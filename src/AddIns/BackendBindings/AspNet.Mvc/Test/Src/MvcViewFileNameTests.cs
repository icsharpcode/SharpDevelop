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
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcViewFileNameTests
	{
		MvcViewFileName mvcViewFileName;
		
		void CreateFileName()
		{
			mvcViewFileName = new MvcViewFileName();
		}
		
		[Test]
		public void ToString_ViewNameAndViewFolderSpecified_ReturnsViewFileNameGeneratedFromViewNameAndFolder()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Aspx;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			mvcViewFileName.ViewName = "Index";
			mvcViewFileName.Folder = @"d:\projects\MyAspProject\Views\About";
			
			string fileName = mvcViewFileName.ToString();
			string expectedFileName = @"d:\projects\MyAspProject\Views\About\Index.aspx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetPath_ViewFolderIsNull_ReturnsViewFileNameUsingOnlyViewName()
		{
			CreateFileName();
			mvcViewFileName.Folder = null;
			mvcViewFileName.ViewName = "Home";
			
			string fileName = mvcViewFileName.GetPath();
			
			Assert.AreEqual("Home.aspx", fileName);
		}
		
		[Test]
		public void GetPath_ViewNameIsNull_ReturnsView1AsFileNameAndUsesViewFolder()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Aspx;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			mvcViewFileName.Folder = @"d:\projects\MyProject\Views\Home";
			mvcViewFileName.ViewName = null;
			
			string fileName = mvcViewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Views\Home\View1.aspx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void ViewName_ViewNameIsNull_ReturnsEmptyString()
		{
			CreateFileName();
			mvcViewFileName.ViewName = null;
			
			string viewName = mvcViewFileName.ViewName;
			
			Assert.AreEqual(String.Empty, viewName);
		}
		
		[Test]
		public void ViewFolder_ViewFolderIsNull_ReturnsEmptyString()
		{
			CreateFileName();
			mvcViewFileName.Folder = null;
			
			string viewFolder = mvcViewFileName.Folder;
			
			Assert.AreEqual(String.Empty, viewFolder);
		}
		
		[Test]
		public void GetFileName_ViewNameAndViewFolderSpecified_ReturnsViewNameWithExtensionButNoFolderPath()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Aspx;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			mvcViewFileName.Folder = @"d:\projects\myproject\Views";
			mvcViewFileName.ViewName = "Index";
			
			string fileName = mvcViewFileName.GetFileName();
			
			Assert.AreEqual("Index.aspx", fileName);
		}
		
		[Test]
		public void GetPath_CSharpRazor_ReturnsCSharpRazorFileName()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Razor;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			mvcViewFileName.ViewName = "Index";
			mvcViewFileName.Folder = @"d:\projects\MyAspProject\Views\About";
			
			string fileName = mvcViewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyAspProject\Views\About\Index.cshtml";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetPath_VisualBasicRazor_ReturnsVisualBasicRazorFileName()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Razor;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
			mvcViewFileName.ViewName = "Index";
			mvcViewFileName.Folder = @"d:\projects\MyAspProject\Views\About";
			
			string fileName = mvcViewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyAspProject\Views\About\Index.vbhtml";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetPath_CSharpAspxTemplateWithIsPartial_ReturnsCSharpUserControlFileName()
		{
			CreateFileName();
			mvcViewFileName.TemplateType = MvcTextTemplateType.Aspx;
			mvcViewFileName.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			mvcViewFileName.ViewName = "Index";
			mvcViewFileName.Folder = @"d:\projects\MyAspProject\Views\About";
			mvcViewFileName.IsPartialView = true;
			
			string fileName = mvcViewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyAspProject\Views\About\Index.ascx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
	}
}
