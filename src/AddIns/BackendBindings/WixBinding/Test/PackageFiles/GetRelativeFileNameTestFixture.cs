// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
