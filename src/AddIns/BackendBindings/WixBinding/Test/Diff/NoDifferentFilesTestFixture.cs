// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace WixBinding.Tests.Diff
{
	[TestFixture]
	public class NoDifferentFilesTestFixture : IDirectoryReader
	{
		WixPackageFilesDiffResult[] diffResults;
		List<string> directories;
		List<string> directoryExistsChecks;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			directories = new List<string>();
			directoryExistsChecks = new List<string>();
			WixDocument doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			WixPackageFilesDiff diff = new WixPackageFilesDiff(this);
			diffResults = diff.Compare(doc.RootDirectory);
		}
		
		[Test]
		public void NoDiffResultsFound()
		{
			Assert.AreEqual(0, diffResults.Length);
		}
		
		[Test]
		public void FilesRequestedFromDirectory()
		{
			Assert.AreEqual(1, directories.Count);
			Assert.AreEqual(@"C:\Projects\Setup\bin", directories[0]);
		}
		
		[Test]
		public void DirectoryExistsChecks()
		{
			Assert.AreEqual(1, directoryExistsChecks.Count);
			Assert.AreEqual(@"C:\Projects\Setup\bin", directoryExistsChecks[0]);
		}

		public string[] GetFiles(string path)
		{
			directories.Add(path);
			return new string[] {@"license.rtf"};
		}
		
		public string[] GetDirectories(string path)
		{
			return new string[0];
		}
		
		public bool DirectoryExists(string path)
		{
			directoryExistsChecks.Add(path);
			return true;
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2003/01/wi'>\r\n" +
				"\t<Product Name='Test' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id='TARGETDIR' SourceName='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFilesFolder' Name='PFiles'>\r\n" +
				"\t\t\t\t<Directory Id='INSTALLDIR' Name='YourApp' LongName='Your Application'>\r\n" +
				"\t\t\t\t\t<Component Id='MyComponent' DiskId='1'>\r\n" +
				"\t\t\t\t\t\t<File Id='LicenseFile' Name='license.rtf' Source='bin\\license.rtf' />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
