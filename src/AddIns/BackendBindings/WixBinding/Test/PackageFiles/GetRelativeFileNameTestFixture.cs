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

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class GetRelativeFileNameTestFixture
	{
		WixDocument doc;
		string fullPathBeforeWixDocumentLoaded;
		string relativePathBeforeWixDocumentLoaded;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			string relativeFileNamePath = @"..\..\bin\Test.exe";
			fullPathBeforeWixDocumentLoaded = doc.GetFullPath(relativeFileNamePath);
			string fullFileName = @"C:\Projects\MySetup\Test\Test.exe";
			relativePathBeforeWixDocumentLoaded = doc.GetRelativePath(fullFileName);

			doc.LoadXml("<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'></Wix>");
			doc.FileName = @"C:\Projects\Test\Setup.wxs";
		}
		
		/// <summary>
		/// The WixPackageFilesEditor does not change the path if no document
		/// is loaded since there is no WixDocument to work out the full path.
		/// </summary>
		[Test]
		public void FullPathBeforeWixDocumentLoaded()
		{
			Assert.AreEqual(@"..\..\bin\Test.exe", fullPathBeforeWixDocumentLoaded);
		}
		
		/// <summary>
		/// Again the path should be the same as that passed in since
		/// no WixDocument is available to work out the relative path.
		/// </summary>
		[Test]
		public void RelativePathBeforeWixDocumentLoaded()
		{
			Assert.AreEqual(@"C:\Projects\MySetup\Test\Test.exe", relativePathBeforeWixDocumentLoaded);
		}
		
		[Test]
		public void FullPath()
		{
			string relativePath = @"..\Source\bin\Test.exe";
			Assert.AreEqual(@"C:\Projects\Source\bin\Test.exe", doc.GetFullPath(relativePath));
		}
		
		[Test]
		public void RelativePath()
		{
			string fullPath = @"C:\Projects\MyTest\bin\Test.exe";
			Assert.AreEqual(@"..\MyTest\bin\Test.exe", doc.GetRelativePath(fullPath));
		}
		
		[Test]
		public void NullWixDocumentFileName()
		{
			doc.FileName = null;
			string relativeFileNamePath = @"..\..\bin\Test.exe";
			Assert.AreEqual(relativeFileNamePath, doc.GetFullPath(relativeFileNamePath));
		}
	}
}
