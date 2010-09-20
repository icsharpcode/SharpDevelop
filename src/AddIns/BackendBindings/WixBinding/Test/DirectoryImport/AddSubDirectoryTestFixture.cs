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
	/// Adds a subdirectory to an existing directory in the package files editor.
	/// </summary>
	[TestFixture]
	public class AddSubDirectoryTestFixture : PackageFilesTestFixtureBase
	{			
		WixDirectoryElement appDirectoryElement;
		WixFileElement exeFileElement;
		WixFileElement readmeFileElement;
		string directory = @"C:\Projects\Setup\MyApplication";
		string[] files = new string[] {"MyApp.exe", "readme.txt"};
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			WixDirectoryElement programFilesFolderElement = (WixDirectoryElement)editor.Document.GetRootDirectory().FirstChild;
			view.SelectedElement = programFilesFolderElement;
			editor.AddDirectory(directory);
			
			appDirectoryElement = (WixDirectoryElement)programFilesFolderElement.FirstChild;
			exeFileElement = (WixFileElement)appDirectoryElement.SelectSingleNode("w:Component/w:File[@Name='MyApp.exe']", new WixNamespaceManager(editor.Document.NameTable));
			readmeFileElement = (WixFileElement)appDirectoryElement.SelectSingleNode("w:Component/w:File[@Name='readme.txt']", new WixNamespaceManager(editor.Document.NameTable));
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
			Assert.AreEqual("MyApplication", appDirectoryElement.DirectoryName);
		}
		
		[Test]
		public void AppDirectoryNameAttributeMatchesDirectoryName()
		{
			Assert.AreEqual("MyApplication", appDirectoryElement.GetAttribute("Name"));
		}
		
		[Test]
		public void AppDirectoryLongNameAttributeDoesNotExist()
		{
			Assert.IsFalse(appDirectoryElement.HasAttribute("LongName"));
		}
		
		[Test]
		public void ExeFileElementAdded()
		{
			Assert.IsNotNull(exeFileElement);
		}
		
		[Test]
		public void ReadmeFileElementAdded()
		{
			Assert.IsNotNull(readmeFileElement);
		}
		
		[Test]
		public void ReadmeFileElementIsKeyPath()
		{
			Assert.AreEqual("yes", readmeFileElement.GetAttribute("KeyPath"));
		}
		
		/// <summary>
		/// Gets the MyApp directory files.
		/// </summary>
		public override string[] GetFiles(string path)
		{
			return files;
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
				"\t\t\t<Directory Id='ProgramFiles' SourceName='ProgramFiles'>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
