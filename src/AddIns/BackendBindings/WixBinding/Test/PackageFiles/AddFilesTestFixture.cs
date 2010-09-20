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
	/// Adds several files to the selected component node.
	/// </summary>
	[TestFixture]
	public class AddFilesTestFixture : PackageFilesTestFixtureBase
	{		
		XmlElement componentElement;
		XmlElement exeFileElement;
		XmlElement readmeFileElement;
		string[] fileNames;
		string exeFileName;
		string readmeFileName;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			componentElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[0].ChildNodes[0].ChildNodes[0];
			view.SelectedElement = componentElement;
			editor.SelectedElementChanged();
			exeFileName = Path.Combine(project.Directory, @"bin\TestApplication.exe");
			readmeFileName = Path.Combine(project.Directory, @"docs\readme.rtf");
			fileNames = new string[] {exeFileName, readmeFileName};
			editor.AddFiles(fileNames);
			exeFileElement = (XmlElement)componentElement.ChildNodes[0];
			readmeFileElement = (XmlElement)componentElement.ChildNodes[1];
			
			// Try to add files when the selected element is null.
			view.SelectedElement = null;
			editor.SelectedElementChanged();
			editor.AddFiles(fileNames);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(view.IsDirty);
		}
		
		[Test]
		public void ComponentElementHasTwoChildren()
		{
			Assert.AreEqual(2, componentElement.ChildNodes.Count);
		}
		
		[Test]
		public void ExeFileNameAdded()
		{
			Assert.AreEqual(@"bin\TestApplication.exe", exeFileElement.GetAttribute("Source"));
		}
		
		[Test]
		public void ReadmeFileNameAdded()
		{
			Assert.AreEqual(@"docs\readme.rtf", readmeFileElement.GetAttribute("Source"));
		}
		
		[Test]
		public void ReadmeFileElementKeyPathNotSet()
		{
			Assert.IsFalse(readmeFileElement.HasAttribute("KeyPath"));
		}
		
		[Test]
		public void ExeFileLongNameAttributeDoesNotExist()
		{
			Assert.IsFalse(exeFileElement.HasAttribute("LongName"));
		}
		
		[Test]
		public void ExeFileName()
		{
			Assert.AreEqual("TestApplication.exe", exeFileElement.GetAttribute("Name"));
		}
		
		[Test]
		public void ExeFileId()
		{
			Assert.AreEqual("TestApplication.exe", exeFileElement.GetAttribute("Id"));
		}
		
		[Test]
		public void ReadmeFileLongName()
		{
			Assert.IsFalse(readmeFileElement.HasAttribute("LongName"));
		}

		[Test]
		public void ReadmeFileName()
		{
			Assert.AreEqual("readme.rtf", readmeFileElement.GetAttribute("Name"));
		}
		
		[Test]
		public void ReadmeFileId()
		{
			Assert.AreEqual("readme.rtf", readmeFileElement.GetAttribute("Id"));
		}
		
		[Test]
		public void ExeFileElementAddedToView()
		{
			Assert.AreSame(view.ElementsAdded[0], exeFileElement);
		}
		
		[Test]
		public void ReadmeFileElementAddedToView()
		{
			Assert.AreSame(view.ElementsAdded[1], readmeFileElement);
		}
		
		[Test]
		public void ComponentElementDiskId()
		{
			Assert.AreEqual("1", componentElement.GetAttribute("DiskId"));
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
				"\t\t\t\t\t<Component Id=\"CoreComponents\">\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
