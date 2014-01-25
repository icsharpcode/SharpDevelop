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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
			diffResults = diff.Compare(doc.GetRootDirectory());
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
			StringBuilder directoriesChecked = new StringBuilder();
			foreach (string dir in directoryExistsChecks) {
				directoriesChecked.AppendLine(dir);
			}
			Assert.AreEqual(1, directoryExistsChecks.Count, directoriesChecked.ToString());
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
