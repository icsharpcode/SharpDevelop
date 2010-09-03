// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Adds a directory to a package with no directories.
	/// </summary>
	[TestFixture]
	public class AddDirectoryElementToPackageWithNoDirectoriesTestFixture : PackageFilesTestFixtureBase
	{
		XmlElement directoryElement;
		XmlElement childDirectoryElement;
		XmlElement firstSelectedElement;
		XmlElement secondSelectedElement;
		XmlElement secondDirectoryElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			editor.AddElement("Directory");
			directoryElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[0];
			firstSelectedElement = view.SelectedElement;
			editor.AddElement("Directory");
			childDirectoryElement = (XmlElement)directoryElement.ChildNodes[0];
			secondSelectedElement = view.SelectedElement;
			view.SelectedElement = null;
			editor.AddElement("Directory");
			secondDirectoryElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[1];
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(view.IsDirty);
		}
		
		[Test]
		public void DirectoryAdded()
		{
			Assert.IsNotNull(directoryElement);
			Assert.AreSame(directoryElement, view.ElementsAdded[0]);
		}
		
		[Test]
		public void DirectoryId()
		{
			Assert.AreEqual("NewDirectory", directoryElement.GetAttribute("Id"));
		}
		
		[Test]
		public void DirectoryShortName()
		{
			Assert.AreEqual(String.Empty, directoryElement.GetAttribute("Name"));
		}
		
		[Test]
		public void DirectoryHasNoLongNameAttribute()
		{
			Assert.IsFalse(directoryElement.HasAttribute("LongName"));
		}

		
		[Test]
		public void SecondDirectoryAdded()
		{
			Assert.IsNotNull(secondDirectoryElement);
			Assert.AreSame(secondDirectoryElement, view.ElementsAdded[2]);
		}
		
		[Test]
		public void ChildDirectoryAdded()
		{
			Assert.IsNotNull(childDirectoryElement);
			Assert.AreSame(childDirectoryElement, view.ElementsAdded[1]);
		}
		
		[Test]
		public void RootDirectoryId()
		{
			Assert.AreEqual("TARGETDIR", editor.Document.GetRootDirectory().GetAttribute("Id"));
		}
		
		[Test]
		public void RootDirectorySourceName()
		{
			Assert.AreEqual("SourceDir", editor.Document.GetRootDirectory().GetAttribute("SourceName"));
		}
		
		[Test]
		public void FirstSelectedElementIsFirstDirectory()
		{
			Assert.AreSame(directoryElement, firstSelectedElement);
		}
		
		[Test]
		public void SecondSelectedElementIsChildDirectory()
		{
			Assert.AreSame(childDirectoryElement, secondSelectedElement);
		}
		
		[Test]
		public void DirectoryAttributesDisplayed()
		{
			Assert.AreEqual(String.Empty, base.GetAttribute(view.Attributes, "FileSource").Value);
		}
		
		[Test]
		public void ThreeElementsAddedToView()
		{
			Assert.AreEqual(3, view.ElementsAdded.Length);
		}
		
		[Test]
		public void ContextMenuIsEnabled()
		{
			Assert.IsTrue(view.ContextMenuEnabled);
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
