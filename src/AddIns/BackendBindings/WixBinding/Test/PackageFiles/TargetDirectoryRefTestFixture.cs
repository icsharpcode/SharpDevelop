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

using System;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Project contains two files, one main Wix source file which has the TARGETDIR 
	/// directory element, and a Wix fragment which contains a TARGETDIR directory ref.
	/// The test fixture checks that the WixPackageFilesEditor uses the directory ref
	/// instead of the main directory element.
	/// </summary>
	[TestFixture]
	public class TargetDirectoryRefTestFixture : ITextFileReader, IWixDocumentWriter
	{
		MockWixPackageFilesView view;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.Name = "MySetup";
			FileProjectItem item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Setup.wxs";
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Files.wxs";
			ProjectService.AddProjectItem(p, item);
			
			view = new MockWixPackageFilesView();
			WixPackageFilesEditor editor = new WixPackageFilesEditor(view, this, this);
			editor.ShowFiles(p);
		}
		
		[Test]
		public void OneRootDirectoryAdded()
		{
			Assert.AreEqual(1, view.DirectoriesAdded.Count);
		}
		
		[Test]
		public void NoSourceFilesFoundMessageNotShown()
		{
			Assert.IsFalse(view.IsNoSourceFileFoundMessageDisplayed);
		}
		
		[Test]
		public void DirectoryId()
		{
			XmlElement element = view.DirectoriesAdded[0];
			Assert.AreEqual("ProgramFilesFolder", element.GetAttribute("Id"));
		}
		
		public TextReader Create(string fileName)
		{
			if (Path.GetFileName(fileName) == "Setup.wxs") {
				return new StringReader(GetMainWixXml());
			}
			return new StringReader(GetWixFragmentXml());
		}
		
		public void Write(WixDocument document)
		{
		}
		
		string GetMainWixXml()
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
		
		string GetWixFragmentXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"MyCompany\" Name=\"MyComp\">\r\n" +
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</DirectoryRef>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
