// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Adds several directories and its contained files to the setup package.
	/// The original package has no directories, only the root directory is
	/// defined.
	/// </summary>
	[TestFixture]
	public class AddDirectoryTestFixture : PackageFilesTestFixtureBase
	{			
		WixDirectoryElement appDirectoryElement;
		WixComponentElement myAppExeFileComponentElement;
		WixFileElement myAppExeFileElement;
		WixDirectoryElement docsDirectoryElement;
		WixDirectoryElement srcDirectoryElement;
		WixDirectoryElement cssDirectoryElement;
		WixFileElement readmeFileElement;
		string docsDirectory = @"C:\Projects\Test\MyApp\docs";
		string cssDirectory = @"C:\Projects\Test\MyApp\docs\css";
		string srcDirectory = @"C:\Projects\Test\MyApp\src";
		string directory = @"C:\Projects\Test\MyApp";
		string myAppExeFile = "MyApp.exe";
		string[] srcFiles = new string[] {"MyProj.sln", "MyProj.csproj", "Main.cs"};
		string[] docFiles = new string[] {"readme.txt", "license.txt"};
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			editor.AddDirectory(directory);
			
			WixNamespaceManager nsManager = new WixNamespaceManager(editor.Document.NameTable);
			appDirectoryElement = (WixDirectoryElement)editor.Document.GetRootDirectory().FirstChild;
			myAppExeFileComponentElement = (WixComponentElement)appDirectoryElement.SelectSingleNode("w:Component", nsManager);
			myAppExeFileElement = (WixFileElement)myAppExeFileComponentElement.LastChild;
			docsDirectoryElement = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@Name='docs']", nsManager);
			srcDirectoryElement = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@Name='src']", nsManager);
			cssDirectoryElement = (docsDirectoryElement.GetDirectories())[0];
			readmeFileElement = (WixFileElement)docsDirectoryElement.SelectSingleNode("w:Component/w:File[@Name='readme.txt']", nsManager);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(view.IsDirty);
		}
		
		[Test]
		public void DirectoryElementAddedToView()
		{
			Assert.IsInstanceOf(typeof(WixDirectoryElement), view.ElementsAdded[0]);
		}
		
		[Test]
		public void AppDirectoryName()
		{
			Assert.AreEqual("MyApp", appDirectoryElement.DirectoryName);
		}
			
		[Test]
		public void AppDirectoryLongNameAttributeDoesNotExist()
		{
			Assert.IsFalse(appDirectoryElement.HasAttribute("LongName"));
		}
		
		[Test]
		public void AppDirectoryId()
		{
			Assert.AreEqual("MyApp", appDirectoryElement.Id);
		}
		
		[Test]
		public void AppDirectoryHasChildComponent()
		{
			Assert.IsNotNull(myAppExeFileComponentElement);
		}
		
		[Test]
		public void AppExeComponentDiskId()
		{
			Assert.AreEqual("1", myAppExeFileComponentElement.DiskId);
		}
		
		[Test]
		public void AppExeFileElementRelativeFileName()
		{
			Assert.AreEqual("MyApp.exe", myAppExeFileElement.FileName);
		}
		
		[Test]
		public void AppExeFileElementId()
		{
			Assert.AreEqual("MyApp.exe", myAppExeFileElement.Id);
		}
		
		[Test]
		public void AddExeSourceFile()
		{
			Assert.AreEqual(@"MyApp\MyApp.exe", myAppExeFileElement.Source);
		}
		
		[Test]
		public void DocDirectoryElementExists()
		{
			Assert.IsNotNull(docsDirectoryElement);
		}
		
		[Test]
		public void SrcDirectoryElementExists()
		{
			Assert.IsNotNull(srcDirectoryElement);
		}
		
		[Test]
		public void CssDirectoryElementExists()
		{
			Assert.IsNotNull(cssDirectoryElement);
		}
		
		[Test]
		public void ReadmeFileElementExists()
		{
			Assert.IsNotNull(readmeFileElement);
		}
		
		[Test]
		public void AppExeComponentId()
		{
			Assert.AreEqual("MyAppExe", myAppExeFileComponentElement.Id);
		}
		
		public override string[] GetFiles(string path)
		{
			if (path == docsDirectory) {
				return docFiles;
			} else if (path == srcDirectory) {
				return srcFiles;
			} else if (path == cssDirectory) {
				return new string[0];
			}
			return new string[] {myAppExeFile};
		}
		
		public override string[] GetDirectories(string path)
		{
			if (path == directory) {
				return new string[] {docsDirectory, srcDirectory};
			} else if (path == docsDirectory) {
				return new string[] {cssDirectory};
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
