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
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Tests that files can be added when a directory element is selected in the PackageFilesEditor.
	/// </summary>
	[TestFixture]
	public class AddFilesToDirectoryTestFixture : PackageFilesTestFixtureBase
	{
		XmlElement exeFileElement;
		XmlElement installDirElement;
		XmlElement readmeFileElement;
		XmlElement exeFileComponentElement;
		XmlElement readmeFileComponentElement;
		string[] fileNames;
		string exeFileName;
		string readmeFileName;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			installDirElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[0].ChildNodes[0];
			view.SelectedElement = installDirElement;
			editor.SelectedElementChanged();
			exeFileName = Path.Combine(project.Directory, @"bin\TestApplication.exe");
			readmeFileName = Path.Combine(project.Directory, @"docs\readme.rtf");
			fileNames = new string[] {exeFileName, readmeFileName};
			editor.AddFiles(fileNames);
			if (installDirElement.ChildNodes.Count > 1) {
				exeFileComponentElement = (XmlElement)installDirElement.ChildNodes[0];
				if (exeFileComponentElement.HasChildNodes) {
					exeFileElement = (XmlElement)exeFileComponentElement.ChildNodes[0];
				}
				readmeFileComponentElement = (XmlElement)installDirElement.ChildNodes[1];
				if (readmeFileComponentElement.HasChildNodes) {
					readmeFileElement = (XmlElement)readmeFileComponentElement.ChildNodes[0];
				}
			}
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(view.IsDirty);
		}
		
		/// <summary>
		/// The two parent component elements are added to the view. The child elements
		/// are not added directly to the view since they are child nodes of the component elements.
		/// </summary>
		[Test]
		public void TwoElementsAddedToView()
		{
			Assert.AreEqual(2, view.ElementsAdded.Length);
		}
		
		[Test]
		public void SanityCheckInstallDirectoryElement()
		{
			Assert.AreEqual("INSTALLDIR", installDirElement.GetAttribute("Id"));
		}
		
		[Test]
		public void InstallDirectoryElementHasTwoChildNodes()
		{
			Assert.AreEqual(2, installDirElement.ChildNodes.Count);
		}
		
		[Test]
		public void ExeFileComponentElementExists()
		{
			Assert.IsNotNull(exeFileComponentElement);
		}
		
		[Test]
		public void ReadmeFileComponentElementExists()
		{
			Assert.IsNotNull(readmeFileComponentElement);
		}

		[Test]
		public void ExeFileComponentId()
		{
			WixComponentElement component = new WixComponentElement(new WixDocument());
			string expectedId = component.GenerateIdFromFileName("TestApplication.exe");
			Assert.AreEqual(expectedId, exeFileComponentElement.GetAttribute("Id"));
		}

		[Test]
		public void ReadmeFileComponentId()
		{
			WixComponentElement component = new WixComponentElement(new WixDocument());
			string expectedId = component.GenerateIdFromFileName("readme.rtf");
			Assert.AreEqual(expectedId, readmeFileComponentElement.GetAttribute("Id"));
		}

		[Test]
		public void ExeFileId()
		{
			Assert.AreEqual("TestApplication.exe", exeFileElement.GetAttribute("Id"));
		}
		
		[Test]
		public void ReadmeFileId()
		{
			Assert.AreEqual("readme.rtf", readmeFileElement.GetAttribute("Id"));
		}
		
		[Test]
		public void ExeFileElementExists()
		{
			Assert.IsNotNull(exeFileElement);
		}
		
		[Test]
		public void ReadmeFileElementExists()
		{
			Assert.IsNotNull(readmeFileElement);
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
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}		
	}
}
