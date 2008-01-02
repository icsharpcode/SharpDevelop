// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDirectory object correctly identifies its own child
	/// directories.
	/// </summary>
	[TestFixture]
	public class ChildDirectoriesTestFixture
	{		
		WixDirectoryElement rootDirectory;
		WixDirectoryElement programFilesDirectory;
		WixDirectoryElement myAppDirectory;
		WixDirectoryElement testDirectory;
		WixDirectoryElement[] rootChildDirectories;
		WixDirectoryElement[] programFilesChildDirectories;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			rootDirectory = doc.RootDirectory;
			rootChildDirectories = rootDirectory.GetDirectories();
			programFilesDirectory = rootChildDirectories[0];
			programFilesChildDirectories = programFilesDirectory.GetDirectories();
			myAppDirectory = programFilesChildDirectories[0];
			testDirectory = rootChildDirectories[1];
		}
		
		[Test]
		public void RootDirectoryHasTwoChildDirectories()
		{
			Assert.AreEqual(2, rootChildDirectories.Length);
		}
		
		[Test]
		public void MyAppDirectoryHasNoChildDirectories()
		{
			Assert.AreEqual(0, myAppDirectory.GetDirectories().Length);
		}
		
		[Test]
		public void MyAppDirectorySourceName()
		{
			Assert.AreEqual("MyAppSrc", myAppDirectory.SourceName);
		}
		
		[Test]
		public void MyAppDirectoryName()
		{
			Assert.AreEqual("My Application", myAppDirectory.DirectoryName);
		}

		[Test]
		public void ProgramFilesDirectoryHasOneChildDirectory()
		{
			Assert.AreEqual(1, programFilesChildDirectories.Length);
		}
		
		[Test]
		public void TestDirectoryId()
		{
			Assert.AreEqual("Test", testDirectory.Id);
		}
		
		[Test]
		public void TestDirectoryName()
		{
			Assert.AreEqual("App", testDirectory.DirectoryName);
		}
		
		[Test]
		public void ProgramFilesDirectoryName()
		{
			Assert.AreEqual("Program Files", programFilesDirectory.DirectoryName);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"MyApp\" SourceName=\"MyAppSrc\" Name=\"My Application\">\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t\t<Directory Id=\"Test\" SourceName=\"Test\" Name=\"App\"/>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
