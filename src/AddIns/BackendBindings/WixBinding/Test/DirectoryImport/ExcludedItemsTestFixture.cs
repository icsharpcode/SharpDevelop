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

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using WixBinding.Tests.PackageFiles;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DirectoryImport
{
	/// <summary>
	/// Tests that files and folders are ignored if they are on the package editor's 
	/// ignore list.
	/// </summary>
	[TestFixture]
	public class ExcludedItemsTestFixture : PackageFilesTestFixtureBase
	{			
		WixDirectoryElement appDirectoryElement;
		WixDirectoryElement docsDirectoryElement;
		WixDirectoryElement srcDirectoryElement;
		WixFileElement readmeFileElement;
		string docsDirectory = @"C:\Projects\Test\MyApp\docs";
		string objDirectory = @"C:\Projects\Test\MyApp\docs\obj";
		string srcDirectory = @"C:\Projects\Test\MyApp\src";
		string directory = @"C:\Projects\Test\MyApp";
		string myAppExeFile = "MyApp.exe";
		string[] srcFiles = new string[] {"MyProj.sln", "MyProj.csproj", "Main.cs"};
		string[] docFiles = new string[] {"readme.txt", "license.txt"};
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			editor.ExcludedItems.AddRange(new string[] {"readme.txt", "obj"});
			editor.AddDirectory(directory);
			
			WixNamespaceManager nsManager = new WixNamespaceManager(editor.Document.NameTable);
			appDirectoryElement = (WixDirectoryElement)editor.Document.GetRootDirectory().FirstChild;
			docsDirectoryElement = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@Name='docs']", nsManager);
			srcDirectoryElement = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@Name='src']", nsManager);
			readmeFileElement = (WixFileElement)docsDirectoryElement.SelectSingleNode("w:Component/w:File[@Name='readme.txt']", nsManager);
		}
		
		[Test]
		public void ReadmeFileNotAdded()
		{
			Assert.IsNull(readmeFileElement);
		}
		
		[Test]
		public void ObjDirectoryNotAdded()
		{
			Assert.AreEqual(0, docsDirectoryElement.GetDirectories().Length);
		}

		public override string[] GetFiles(string path)
		{
			if (path == docsDirectory) {
				return docFiles;
			} else if (path == srcDirectory) {
				return srcFiles;
			} else if (path == objDirectory) {
				return new string[0];
			}
			return new string[] {myAppExeFile};
		}
		
		public override string[] GetDirectories(string path)
		{
			if (path == directory) {
				return new string[] {docsDirectory, srcDirectory};
			} else if (path == docsDirectory) {
				return new string[] {objDirectory};
			}
			return new string[0];
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
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
