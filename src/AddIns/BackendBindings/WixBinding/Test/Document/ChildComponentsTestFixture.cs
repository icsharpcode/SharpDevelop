// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDirectory object correctly identifies its own child
	/// components.
	/// </summary>
	[TestFixture]
	public class ChildComponentsTestFixture
	{				
		WixComponentElement[] childComponents;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDirectoryElement rootDirectory = doc.GetRootDirectory();
			WixDirectoryElement[] rootChildDirectories = rootDirectory.GetDirectories();
			WixDirectoryElement programFilesDirectory = rootChildDirectories[0];
			WixDirectoryElement[] programFilesChildDirectories = programFilesDirectory.GetDirectories();
			WixDirectoryElement myAppDirectory = programFilesChildDirectories[0];
			childComponents = myAppDirectory.GetComponents();
		}

		[Test]
		public void TwoChildComponents()
		{
			Assert.AreEqual(2, childComponents.Length);
		}
		
		[Test]
		public void FirstComponentId()
		{
			Assert.AreEqual("ComponentOne", childComponents[0].Id);
		}
		
		[Test]
		public void SecondComponentId()
		{
			Assert.AreEqual("ComponentTwo", childComponents[1].Id);
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
				"\t\t\t\t<Directory Id=\"MyApp\" SourceName=\"MyAppSrc\" Name=\"MyApp\" LongName=\"My Application\">\r\n" +
				"\t\t\t\t\t<Component Id='ComponentOne'/>\r\n"+
				"\t\t\t\t\t<Component Id='ComponentTwo'/>\r\n"+
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
