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
using System.IO;
using System.Text;

namespace WixBinding.Tests.Diff
{
	/// <summary>
	/// If there is a new directory which is not included in the setup then this should be returned
	/// in the diff result.
	/// </summary>
	[TestFixture]
	public class MissingDirectoryTestFixture : IDirectoryReader
	{
		WixPackageFilesDiffResult[] diffResults;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			WixPackageFilesDiff diff = new WixPackageFilesDiff(this);
			diff.ExcludedFileNames.Add(".svn");
			diffResults = diff.Compare(doc.GetRootDirectory());
		}
		
		[Test]
		public void OneDiffResultFound()
		{
			StringBuilder fileNames = new StringBuilder();
			foreach (WixPackageFilesDiffResult result in diffResults) {
				fileNames.AppendLine(result.FileName);
			}
			Assert.AreEqual(1, diffResults.Length, fileNames.ToString());
		}
		
		[Test]
		public void DiffResultFileName()
		{
			Assert.AreEqual(@"c:\projects\setup\bin\addins", diffResults[0].FileName.ToLowerInvariant());
		}
		
		[Test]
		public void DiffResultType()
		{
			Assert.AreEqual(WixPackageFilesDiffResultType.NewDirectory, diffResults[0].DiffType);
		}

		public string[] GetFiles(string path)
		{
			if (path.StartsWith(@"C:\Projects\Setup\bin")) {
				return new string[] {@"license.rtf"};
			}
			return new string[0];
		}
		
		public string[] GetDirectories(string path)
		{
			return new string[] { Path.Combine(path, "AddIns"), Path.Combine(path, ".svn")};
		}
		
		public bool DirectoryExists(string path)
		{
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
