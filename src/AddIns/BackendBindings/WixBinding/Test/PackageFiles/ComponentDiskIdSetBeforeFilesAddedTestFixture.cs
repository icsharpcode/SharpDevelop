// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Adds several files to the selected component node when the component's
	/// disk id is already set. This makes sure we do not change the disk id.
	/// </summary>
	[TestFixture]
	public class ComponentDiskIdSetBeforeFilesAddedTestFixture : PackageFilesTestFixtureBase
	{		
		XmlElement componentElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			componentElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[0].ChildNodes[0].ChildNodes[0];
			view.SelectedElement = componentElement;
			editor.SelectedElementChanged();
			string exeFileName = Path.Combine(project.Directory, @"bin\TestApplication.exe");
			string readmeFileName = Path.Combine(project.Directory, @"docs\readme.rtf");
			string[] fileNames = new string[] {exeFileName, readmeFileName};
			editor.AddFiles(fileNames);
		}

		[Test]
		public void ComponentElementDiskId()
		{
			Assert.AreEqual("2", componentElement.GetAttribute("DiskId"));
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
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t<Component Id=\"CoreComponents\" DiskId=\"2\">\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
