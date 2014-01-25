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
	public class MvcControllerFileNameTests
	{
		MvcControllerFileName mvcControllerFileName;
		
		void CreateFileName()
		{
			mvcControllerFileName = new MvcControllerFileName();
		}
		
		[Test]
		public void ToString_ControllerNameAndFolderSpecified_ReturnsControllerFileNameGeneratedFromControllerNameAndFolder()
		{
			CreateFileName();
			mvcControllerFileName.Language = MvcTextTemplateLanguage.CSharp;
			mvcControllerFileName.ControllerName = "AboutController";
			mvcControllerFileName.Folder = @"d:\projects\MyAspProject\Controller\About";
			
			string fileName = mvcControllerFileName.ToString();
			string expectedFileName = @"d:\projects\MyAspProject\Controller\About\AboutController.cs";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetPath_ControllerFolderIsNull_ReturnsControllerFileNameUsingOnlyControllerName()
		{
			CreateFileName();
			mvcControllerFileName.Language = MvcTextTemplateLanguage.CSharp;
			mvcControllerFileName.ControllerName = "HomeController";
			mvcControllerFileName.Folder = null;
			
			string fileName = mvcControllerFileName.GetPath();
			
			Assert.AreEqual("HomeController.cs", fileName);
		}
		
		[Test]
		public void GetPath_ControllerNameIsNull_ReturnsDefault1ControllerAsFileNameAndUsesControllerFolder()
		{
			CreateFileName();
			mvcControllerFileName.Language = MvcTextTemplateLanguage.CSharp;
			mvcControllerFileName.Folder = @"d:\projects\MyProject\Controllers\Home";
			mvcControllerFileName.ControllerName = null;
			
			string fileName = mvcControllerFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Controllers\Home\Default1Controller.cs";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void ControllerName_ControllerNameIsNull_ReturnsEmptyString()
		{
			CreateFileName();
			mvcControllerFileName.ControllerName = null;
			
			string controllerName = mvcControllerFileName.ControllerName;
			
			Assert.AreEqual(String.Empty, controllerName);
		}
		
		[Test]
		public void ControllerFolder_ControllerFolderIsNull_ReturnsEmptyString()
		{
			CreateFileName();
			mvcControllerFileName.Folder = null;
			
			string controllerFolder = mvcControllerFileName.Folder;
			
			Assert.AreEqual(String.Empty, controllerFolder);
		}
		
		[Test]
		public void GetFileName_ControllerNameAndFolderSpecified_ReturnsControllerNameWithExtensionButNoFolderPath()
		{
			CreateFileName();
			mvcControllerFileName.Language = MvcTextTemplateLanguage.CSharp;
			mvcControllerFileName.Folder = @"d:\projects\myproject\Controllers";
			mvcControllerFileName.ControllerName = "Index";
			
			string fileName = mvcControllerFileName.GetFileName();
			
			Assert.AreEqual("Index.cs", fileName);
		}
		
		[Test]
		public void GetPath_LanguageIsSetToVisualBasic_ReturnsVisualBasicControllerFileName()
		{
			CreateFileName();
			mvcControllerFileName.Language = MvcTextTemplateLanguage.VisualBasic;
			mvcControllerFileName.ControllerName = "AboutController";
			mvcControllerFileName.Folder = @"d:\projects\MyAspProject\Controller\About";
			
			string fileName = mvcControllerFileName.GetPath();
			string expectedFileName = @"d:\projects\MyAspProject\Controller\About\AboutController.vb";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
	}
}
