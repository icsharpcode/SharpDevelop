// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			mvcViewFileName.ViewName = "Index";
			mvcViewFileName.ViewFolder = @"d:\projects\MyAspProject\Views\About";
			
			string fileName = mvcViewFileName.ToString();
			string expectedFileName = @"d:\projects\MyAspProject\Views\About\Index.aspx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetPath_ViewFolderIsNull_ReturnsViewFileNameUsingOnlyViewName()
		{
			CreateFileName();
			mvcViewFileName.ViewFolder = null;
			mvcViewFileName.ViewName = "Home";
			
			string fileName = mvcViewFileName.GetPath();
			
			Assert.AreEqual("Home.aspx", fileName);
		}
		
		[Test]
		public void GetPath_ViewNameIsNull_ReturnsView1AsFileNameAndUsesViewFolder()
		{
			CreateFileName();
			mvcViewFileName.ViewFolder = @"d:\projects\MyProject\Views\Home";
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
			mvcViewFileName.ViewFolder = null;
			
			string viewFolder = mvcViewFileName.ViewFolder;
			
			Assert.AreEqual(String.Empty, viewFolder);
		}
		
		[Test]
		public void GetFileName_ViewNameAndViewFolderSpecified_ReturnsViewNameWithExtensionButNoFolderPath()
		{
			CreateFileName();
			mvcViewFileName.ViewFolder = @"d:\projects\myproject\Views";
			mvcViewFileName.ViewName = "Index";
			
			string fileName = mvcViewFileName.GetFileName();
			
			Assert.AreEqual("Index.aspx", fileName);
		}
	}
}
