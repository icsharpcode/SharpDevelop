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
using System.Xml;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class ExistingFileIdGenerationTests
	{
		WixDocument doc;
		WixComponentElement component;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			component = (WixComponentElement)doc.SelectSingleNode("//w:Component", new WixNamespaceManager(doc.NameTable));
		}
		
		[Test]
		public void FileIdAlreadyExists()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\doc\license.txt");
			Assert.AreEqual("doc.license.txt", fileElement.Id);
		}
		
		[Test]
		public void FileIdExists()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\doc\license.txt");
			Assert.IsTrue(doc.FileIdExists("license.txt"));
		}
		
		[Test]
		public void FileIdAlreadyExistsAndParentDirectoryUsingAltDirSeparator()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:/Projects/Setup/doc/license.txt");
			Assert.AreEqual("doc.license.txt", fileElement.Id);
		}
		
		[Test]
		public void FileIdAlreadyExistsAndNoParentDirectory()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"license.txt");
			Assert.AreEqual("license1.txt", fileElement.Id);
		}
		
		[Test]
		public void FileIdAlreadyExistsAndParentDirectoryIsRoot()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\license.txt");
			Assert.AreEqual("license1.txt", fileElement.Id);
		}
		
		[Test]
		public void FileIdWithNumberAlreadyExists()
		{
			component.AddFile(@"license.txt");
			WixFileElement fileElement = new WixFileElement(doc, @"license.txt");
			Assert.AreEqual("license2.txt", fileElement.Id);
		}
		
		[Test]
		public void FileIdWithParentDirectoryAndNumberExists()
		{
			component.AddFile(@"C:/Projects/Setup/doc/license.txt");
			WixFileElement fileElement = new WixFileElement(doc, @"C:/Projects/Setup/doc/license.txt");
			Assert.AreEqual("doc.license1.txt", fileElement.Id);
		}

		[Test]
		public void FileIdWithSingleQuote()
		{
			Assert.IsFalse(doc.FileIdExists("lice'nse.txt"));
		}
		
		[Test]
		public void FileIdAlreadyExistsAndParentDirectoryUsingHyphen()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:/Projects/Setup/a-docs/readme.rtf");
			Assert.AreEqual("a_docs.readme.rtf", fileElement.Id);
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
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t\t<Component Guid=\"370DE542-C4A9-48DA-ACF8-09949CDCD808\" Id=\"SharpDevelopDocFiles\" DiskId=\"1\">\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\AssemblyBaseAddresses.txt\" Name=\"baseaddr.txt\" Id=\"AssemblyBaseAddresses.txt\" LongName=\"AssemblyBaseAddresses.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\BuiltWithSharpDevelop.png\" Name=\"bw-sd.PNG\" Id=\"BuiltWithSharpDevelop.png\" LongName=\"BuiltWithSharpDevelop.png\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\ChangeLog.xml\" Name=\"change.xml\" Id=\"ChangeLog.xml\" LongName=\"ChangeLog.xml\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\copyright.txt\" Name=\"cpyright.txt\" Id=\"copyright.txt\" LongName=\"copyright.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\license.txt\" Name=\"license.txt\" Id=\"license.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\readme.rtf\" Name=\"readme.rtf\" Id=\"readme.rtf\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\SharpDevelopInfoResources.txt\" Name=\"Resource.txt\" Id=\"SharpDevelopInfoResources.txt\" LongName=\"SharpDevelopInfoResources.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"a-docs\\readme.rtf\" Name=\"readme.rtf\" Id=\"readme.rtf\" />\r\n" +
				"\t\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
