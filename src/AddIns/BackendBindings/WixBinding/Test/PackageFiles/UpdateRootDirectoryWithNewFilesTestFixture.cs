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
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class UpdateRootDirectoryWithNewFilesTestFixture : UpdateRootDirectoryWithNewFilesTestFixtureBase
	{
		protected override string GetWixXml()
		{
			return 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
		
		protected override void AddNewChildElementsToDirectory()
		{
			WixDirectoryElement dir = document.GetRootDirectory();
			WixDirectoryElement programFilesDir = dir.AddDirectory("ProgramFilesFolder");
			programFilesDir.SourceName = "PFiles";
			programFilesDir.RemoveAttribute("Name");
			
			WixDirectoryElement sharpDevelopDir = programFilesDir.AddDirectory("SharpDevelop");
			sharpDevelopDir.Id = "SharpDevelopFolder";
			
			WixComponentElement component = sharpDevelopDir.AddComponent("SharpDevelopExe");
			component.Guid = "guid";
			WixFileElement file = component.AddFile("SharpDevelop.exe");
			file.Source = @"..\..\bin\SharpDevelop.exe";
		}

		[Test]
		public void GetExpectedWixXml()
		{
			string expectedXml = 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" SourceName=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"SharpDevelopFolder\" Name=\"SharpDevelop\">\r\n" +
				"\t\t\t\t\t<Component Id=\"SharpDevelopExe\" Guid=\"guid\">\r\n" +
				"\t\t\t\t\t\t<File Id=\"SharpDevelop.exe\" Name=\"SharpDevelop.exe\" Source=\"..\\..\\bin\\SharpDevelop.exe\" />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
			
			Assert.AreEqual(expectedXml, textEditor.Document.Text, textEditor.Document.Text);
		}
		
		[Test]
		public void PackageFilesViewIsDirtyIsFalse()
		{
			Assert.IsFalse(packageFilesView.IsDirty);
		}
	}
}
