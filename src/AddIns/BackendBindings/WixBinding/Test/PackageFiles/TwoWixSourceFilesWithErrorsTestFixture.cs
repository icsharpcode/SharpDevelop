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
	/// The WixPackageFilesEditor should try all the WiX source files before reporting
	/// an error.
	/// </summary>
	[TestFixture]
	public class TwoWixSourceFilesWithErrorsIgnoredTestFixture : ITextFileReader, IWixDocumentWriter
	{
		MockWixPackageFilesView view;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.Name = "MySetup";
			FileProjectItem item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Dialogs.wxs";
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Setup.wxs";
			ProjectService.AddProjectItem(p, item);
			
			view = new MockWixPackageFilesView();
			WixPackageFilesEditor editor = new WixPackageFilesEditor(view, this, this);
			editor.ShowFiles(p);
		}
		
		[Test]
		public void SourceFilesContainErrorsMessageDisplayed()
		{
			Assert.IsTrue(view.IsSourceFilesContainErrorsMessageDisplayed);
		}
		
		public TextReader Create(string fileName)
		{
			return new StringReader("<Wix>");
		}
		
		public void Write(WixDocument document)
		{
		}
		
		string GetWixXml()
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
