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
	public class AddDirectoryToDirectoryRefTestFixture : PackageFilesTestFixtureBase
	{
		XmlElement directoryElement;
		XmlElement childDirectoryElement;
		XmlElement firstSelectedElement;
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			editor.AddElement("Directory");
			directoryElement = (XmlElement)editor.Document.GetRootDirectoryRef().ChildNodes[0];
			firstSelectedElement = view.SelectedElement;
			editor.AddElement("Directory");
			childDirectoryElement = (XmlElement)directoryElement.ChildNodes[0];
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
		public void ChildDirectoryAdded()
		{
			Assert.IsNotNull(childDirectoryElement);
			Assert.AreSame(childDirectoryElement, view.ElementsAdded[1]);
		}
		
		[Test]
		public void FirstSelectedElementIsFirstDirectory()
		{
			Assert.AreSame(directoryElement, firstSelectedElement);
		}
		
		[Test]
		public void DirectoryAttributesDisplayed()
		{
			Assert.AreEqual(String.Empty, base.GetAttribute(view.Attributes, "FileSource").Value);
		}
		
		[Test]
		public void TwoElementsAddedToView()
		{
			Assert.AreEqual(2, view.ElementsAdded.Length);
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\"/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
