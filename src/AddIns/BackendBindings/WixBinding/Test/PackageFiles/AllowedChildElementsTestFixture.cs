// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Tests the child elements that can be added to the selected node.
	/// </summary>
	[TestFixture]
	public class AllowedChildElementsTestFixture : PackageFilesTestFixtureBase
	{
		string[] childElementAllowedWhenNoItemSelected;
		StringCollection childElementsAllowedWhenDirectoryElementSelected;
		string[] expectedDirectoryChildElementNames;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			childElementAllowedWhenNoItemSelected = new string[view.AllowedChildElements.Count];
			view.AllowedChildElements.CopyTo(childElementAllowedWhenNoItemSelected, 0);
			WixDirectoryElement rootDir = editor.Document.GetRootDirectory();
			XmlElement directoryElement = (XmlElement)rootDir.ChildNodes[0];
			view.SelectedElement = directoryElement;
			editor.SelectedElementChanged();
			childElementsAllowedWhenDirectoryElementSelected = view.AllowedChildElements;
			
			WixSchemaCompletion schema = new WixSchemaCompletion();
			expectedDirectoryChildElementNames = schema.GetChildElements(directoryElement.Name);
		}
		
		[Test]
		public void NoElementSelectedOneChildElementAllowed()
		{
			Assert.AreEqual(1, childElementAllowedWhenNoItemSelected.Length);
		}
		
		[Test]
		public void NoElementSelectedChildElementIsDirectory()
		{
			Assert.AreEqual("Directory", childElementAllowedWhenNoItemSelected[0]);
		}
		
		[Test]
		public void DirectoryElementSelectedChildElementsAllowed()
		{
			foreach (string name in expectedDirectoryChildElementNames) {
				Assert.IsTrue(childElementsAllowedWhenDirectoryElementSelected.Contains(name), "Element '" + name + "' is missing");
			}
		}
		
		[Test]
		public void AtLeastOneChildElementAllowedWhenDirectoryElementSelected()
		{
			Assert.IsTrue(childElementsAllowedWhenDirectoryElementSelected.Count > 0);
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
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t<Component Id=\"CoreComponents\">\r\n" +
				"\t\t\t\t\t\t<File Id=\"CoreComponents\">\r\n" +
				"\t\t\t\t\t\t</File>\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
