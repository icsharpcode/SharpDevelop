// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	/// This test fixture checks that unique short directory names are generated.
	/// </summary>
	[TestFixture]
	public class DuplicateShortDirectoryNameTestFixture : PackageFilesTestFixtureBase
	{			
		WixDirectoryElement appDirectoryElement;
		WixDirectoryElement directoryElementA;
		WixDirectoryElement directoryElementB;
		WixDirectoryElement directoryElementC;
		WixDirectoryElement directoryElementD;
		
		string directory = @"C:\Projects\Test\MyApp";
		string[] directories = new string[] {"DirectoryA", "DirectoryB", "DirectoryC", "Dire.ctoryD"};
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			view.SelectedElement = editor.Document.RootDirectory;
			editor.AddDirectory(directory);
			
			WixNamespaceManager nsManager = new WixNamespaceManager(editor.Document.NameTable);
			appDirectoryElement = (WixDirectoryElement)editor.Document.RootDirectory.SelectSingleNode("w:Directory[@Name='MyApp']", nsManager);;
			
			directoryElementA = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@LongName='DirectoryA']", nsManager);
			directoryElementB = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@LongName='DirectoryB']", nsManager);
			directoryElementC = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@LongName='DirectoryC']", nsManager);
			directoryElementD = (WixDirectoryElement)appDirectoryElement.SelectSingleNode("w:Directory[@LongName='Dire.ctoryD']", nsManager);
		}

		[Test]
		public void DirectoryElementAShortName()
		{
			Assert.AreEqual("Director", directoryElementA.ShortName);
		}
		
		[Test]
		public void DirectoryElementBShortName()
		{
			Assert.AreEqual("Directo1", directoryElementB.ShortName);
		}
		
		[Test]
		public void DirectoryElementCShortName()
		{
			Assert.AreEqual("Directo2", directoryElementC.ShortName);
		}
		
		[Test]
		public void DirectoryElementDShortName()
		{
			Assert.AreEqual("Directo3", directoryElementD.ShortName);
		}
		
		public override string[] GetFiles(string path)
		{
			return new string[0];
		}
		
		public override string[] GetDirectories(string path)
		{
			if (path == directory) {
				return directories;
			}
			return new string[0];
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2003/01/wi\">\r\n" +
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
