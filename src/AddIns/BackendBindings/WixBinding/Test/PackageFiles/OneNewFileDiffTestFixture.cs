// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class OneNewFileDiffTestFixture : PackageFilesTestFixtureBase
	{
		WixDirectoryElement installDirectory;
		WixDirectoryElement binDirectory;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			editor.ExcludedItems.AddRange(new string[] {"*.pdb"});
			WixDirectoryElement programFilesDirectory = (WixDirectoryElement)editor.Document.GetRootDirectory().FirstChild;
			installDirectory = (WixDirectoryElement)programFilesDirectory.FirstChild;
			binDirectory = (WixDirectoryElement)installDirectory.LastChild;
		}
		
		[Test]
		public void InstallDirectorySelected()
		{
			view.SelectedElement = installDirectory;
			editor.CalculateDiff();
			Assert.AreEqual(1, view.DiffResults.Length);
			Assert.AreEqual(@"C:\Projects\Test\doc\files\newfile.txt", view.DiffResults[0].FileName);
			Assert.AreEqual(WixPackageFilesDiffResultType.NewFile, view.DiffResults[0].DiffType);
			Assert.IsTrue(view.SelectedElementAccessed);
		}
		
		[Test]
		public void BinDirectorySelected()
		{
			view.SelectedElement = binDirectory;
			editor.CalculateDiff();
			Assert.IsTrue(view.IsNoDifferencesFoundMessageDisplayed);
		}
		
		public override string[] GetFiles(string path)
		{
			if (path == @"C:\Projects\Test\doc\files") {
				return new string[] {@"license.rtf", "readme.txt", "newfile.txt"};
			} else {
				return new string[] {@"myapp.exe", "myapp.pdb"};
			}
		}

		protected override string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='Test' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id='TARGETDIR' SourceName='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFilesFolder' Name='PFiles'>\r\n" +
				"\t\t\t\t<Directory Id='INSTALLDIR' Name='YourApp' LongName='Your Application'>\r\n" +
				"\t\t\t\t\t<Component Id='DocComponent' DiskId='1'>\r\n" +
				"\t\t\t\t\t\t<File Id='LicenseFile' Name='license.rtf' Source='doc\\files\\license.rtf' />\r\n" +
				"\t\t\t\t\t\t<File Id='ReadMe' Name='readme.txt' Source='DOC/files\\readme.txt' />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t\t<Directory Id='bin' Name='bin'>\r\n" +
				"\t\t\t\t\t\t<Component Id='BinComponent' DiskId='1'>\r\n" +
				"\t\t\t\t\t\t\t<File Id='MyAppExe' Name='myapp.exe' Source='bin\\myapp.exe' />\r\n" +
				"\t\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +	
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
