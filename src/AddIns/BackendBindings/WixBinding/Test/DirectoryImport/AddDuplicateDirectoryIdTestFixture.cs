// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.PackageFiles;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DirectoryImport
{
	[TestFixture]
	public class AddDuplicateDirectoryIdTestFixture : PackageFilesTestFixtureBase
	{
		string docsDirectory = @"C:\Projects\Test\MyApp\docs";
		string duplicateDocsDirectory = @"C:\Projects\Test\MyApp\docs\docs";
		string[] files = new string[] {"readme.txt"};
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			view.SelectedElement = GetInstallDirectoryElement();
			editor.AddDirectory(docsDirectory);
		}
			
		WixDirectoryElement GetInstallDirectoryElement()
		{
			WixDirectoryElement rootDir = editor.Document.GetRootDirectory();
			WixDirectoryElement programFilesDir = (WixDirectoryElement)rootDir.FirstChild;
			return (WixDirectoryElement)programFilesDir.FirstChild;
		}
		
		WixDirectoryElement GetParentDocsDirectoryElement()
		{
			WixDirectoryElement installDir = GetInstallDirectoryElement();
			return installDir.FirstChild as WixDirectoryElement;
		}
		
		[Test]
		public void EditorAddsParentDocsDirectoryElementToInstallDirectoryElement()
		{
			WixDirectoryElement parentDocDir = GetParentDocsDirectoryElement();
			Assert.AreEqual("docs", parentDocDir.Id);
		}
		
		[Test]
		public void EditorAddsDuplicateDocsDirectoryElementWithUniqueId()
		{
			WixDirectoryElement duplicateDocDir = GetDuplicateDocsDirectoryElement();
			Assert.AreEqual("docsdocs", duplicateDocDir.Id);
		}
		
		WixDirectoryElement GetDuplicateDocsDirectoryElement()
		{
			WixDirectoryElement parentDocDir = GetParentDocsDirectoryElement();
			return parentDocDir.FirstChild as WixDirectoryElement;
		}
		
		[Test]
		public void AddingThirdDuplicateDocsDirectoryGeneratesUniqueDirectoryId()
		{
			WixDirectoryElement duplicateDocsDir = GetDuplicateDocsDirectoryElement();
			view.SelectedElement = duplicateDocsDir;
			editor.AddDirectory(@"C:\Projects\Test\MyApp\docs\docs");
			
			WixDirectoryElement newDir = (WixDirectoryElement)duplicateDocsDir.FirstChild;
			Assert.AreEqual("docsdocsdocs", newDir.Id);
		}
		
		public override string[] GetFiles(string path)
		{
			return new string[0];
		}
		
		public override string[] GetDirectories(string path)
		{
			if (path == docsDirectory) {
				return new string[] { duplicateDocsDirectory };
			}
			return new string[0];
		}
		
		protected override string GetWixXml()
		{
			return 
				"<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"    <Product Name=\"MySetup\" \r\n" +
				"             Manufacturer=\"\" \r\n" +
				"             Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"             Language=\"1033\" \r\n" +
				"             Version=\"1.0.0.0\">\r\n" +
				"        <Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"        <Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"            <Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"                <Directory Id=\"INSTALLDIR\" Name=\"OtherFolder\">\r\n" +
				"                </Directory>\r\n" +
				"            </Directory>\r\n" +
				"        </Directory>\r\n" +
				"    </Product>\r\n" +
				"</Wix>";
		}
	}
}
