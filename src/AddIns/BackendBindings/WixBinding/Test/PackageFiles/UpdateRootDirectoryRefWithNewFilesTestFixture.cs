// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class UpdateRootDirectoryRefWithNewFilesTestFixture : UpdateRootDirectoryWithNewFilesTestFixtureBase
	{
		protected override string GetWixXml()
		{
			return 
				"<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment> \r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFiles\" SourceName=\"PFiles\"/>\r\n" +
				"\t\t</DirectoryRef>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
		
		protected override void AddNewChildElementsToDirectory()
		{
			WixDirectoryRefElement dirRef = document.GetRootDirectoryRef();
			WixDirectoryElement programFilesDir = dirRef.FirstChild as WixDirectoryElement;
			
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
				"<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment> \r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFiles\" SourceName=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"SharpDevelopFolder\" Name=\"SharpDevelop\">\r\n" +
				"\t\t\t\t\t<Component Id=\"SharpDevelopExe\" Guid=\"guid\">\r\n" +
				"\t\t\t\t\t\t<File Id=\"SharpDevelop.exe\" Name=\"SharpDevelop.exe\" Source=\"..\\..\\bin\\SharpDevelop.exe\" />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</DirectoryRef>\r\n" +
				"\t</Fragment>\r\n" +
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
