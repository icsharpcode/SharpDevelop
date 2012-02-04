// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
