// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Checks the filenames created when a new WixFileElement is created.
	/// </summary>
	[TestFixture]
	public class NewWixFileElementTests
	{
		WixDocument doc;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
		}
		
		[Test]
		public void SubFolderFile()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\Files\readme.txt");
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual(@"Files\readme.txt", fileElement.Source);
		}

		[Test]
		public void DifferentParentFolderFile()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\AnotherSetup\Files\readme.txt");
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual("readme.txt", fileElement.Id);
			Assert.AreEqual(@"..\AnotherSetup\Files\readme.txt", fileElement.Source);
		}		
	}
}
