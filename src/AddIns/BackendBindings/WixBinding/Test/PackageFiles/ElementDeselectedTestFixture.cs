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
	/// The package item (e.g. directory) is selected in the view.
	/// </summary>
	[TestFixture]
	public class ElementDeselectedTestFixture : PackageFilesTestFixtureBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			
			XmlElement rootDirectoryElement = editor.Document.GetRootDirectory();
			XmlElement directoryElement = (XmlElement)rootDirectoryElement.ChildNodes[0];
			view.SelectedElement = directoryElement;
			editor.SelectedElementChanged();
			
			// Make sure attributes displayed are cleared when the selected 
			// element is set to null.
			view.SelectedElement = null;
			editor.SelectedElementChanged();
		}
		
		[Test]
		public void AttributesDisplayed()
		{
			Assert.AreEqual(0, view.Attributes.Count);
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
				"\t\t\t\t<Directory Id=\"MyCompany\" Name=\"MyComp\">\r\n" +
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
