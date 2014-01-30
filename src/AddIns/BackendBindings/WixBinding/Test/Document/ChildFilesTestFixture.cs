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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests the WixFileElement GetFiles method and its FileName property.
	/// </summary>
	[TestFixture]
	public class ChildFilesTestFixture
	{
		WixDocument doc;
		WixFileElement[] files;
		
		[SetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			WixDirectoryElement rootDirectory = doc.GetRootDirectory();
			WixDirectoryElement[] rootChildDirectories = rootDirectory.GetDirectories();
			WixDirectoryElement programFilesDirectory = rootChildDirectories[0];
			WixDirectoryElement[] programFilesChildDirectories = programFilesDirectory.GetDirectories();
			WixDirectoryElement myAppDirectory = programFilesChildDirectories[0];
			WixComponentElement[] childComponents = myAppDirectory.GetComponents();
			WixComponentElement coreComponent = childComponents[0];
			files = coreComponent.GetFiles();
		}
		
		[Test]
		public void TwoFiles()
		{
			Assert.AreEqual(2, files.Length);
		}
		
		/// <summary>
		/// The filename should not include the ".." part.
		/// </summary>
		[Test]
		public void FirstFileElementFileName()
		{
			Assert.AreEqual(@"C:\Projects\doc\license.rtf", files[0].GetSourceFullPath());
		}
		
		[Test]
		public void SecondFileElementFileName()
		{
			Assert.AreEqual(@"C:\Projects\Setup\bin\myapp.exe", files[1].GetSourceFullPath());
		}
		
		/// <summary>
		/// Tests that the WixFileElement.FileName property returns the
		/// relative filename if no filename is given to the WixDocument.
		/// </summary>
		[Test]
		public void RelativeFileNameUsed()
		{
			doc.FileName = String.Empty;
			Assert.AreEqual(@"..\doc\license.rtf", files[0].GetSourceFullPath());
			Assert.AreEqual(@"bin\myapp.exe", files[1].GetSourceFullPath());
		}
		
		[Test]
		public void SourceAttributeUsed()
		{
			Assert.AreEqual(@"..\doc\license.rtf", files[0].Source);
			Assert.AreEqual(@"bin\myapp.exe", files[1].Source);
		}

		[Test]
		public void NameAttributeUsed()
		{
			Assert.AreEqual(@"License.rtf", files[0].FileName);
			Assert.AreEqual(@"MyApp.exe", files[1].FileName);
		}
		
		string GetWixXml()
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
				"\t\t\t\t\t\t<File Id=\"LicenseFile\" Name=\"License.rtf\" Source=\"..\\doc\\license.rtf\" />\r\n" +
				"\t\t\t\t\t\t<File Id=\"ExeFile\" Name=\"MyApp.exe\" Source=\"bin\\myapp.exe\" />\r\n" +
				"\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
