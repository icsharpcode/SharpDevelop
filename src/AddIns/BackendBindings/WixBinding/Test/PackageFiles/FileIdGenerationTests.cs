// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Xml;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class FileIdGenerationTests
	{
		WixDocument doc;
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml("<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'/>");
		}
		
		[Test]
		public void ShortFileName()
		{
			string fileName = "test.txt";
			Assert.AreEqual(fileName, WixFileElement.GenerateId(fileName));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.AreEqual(String.Empty, WixFileElement.GenerateId(null));
		}
				
		[Test]
		public void TruncatedShortFileName()
		{
			string fileName = "TESTAP~1.TXT";
			Assert.AreEqual("TESTAP_1.TXT", WixFileElement.GenerateId(fileName));
		}

		[Test]
		public void FirstCharIsDigit()
		{
			string fileName = "8ESTAP.TXT";
			Assert.AreEqual("_ESTAP.TXT", WixFileElement.GenerateId(fileName));
		}
		
		[Test]
		public void FirstCharIsDot()
		{
			string fileName = ".ESTAP.TXT";
			Assert.AreEqual("_ESTAP.TXT", WixFileElement.GenerateId(fileName));
		}
	
		[Test]
		public void CharIsNotStandardAscii()
		{
			string fileName = "AEèSTAP.TXT";
			Assert.AreEqual("AE_STAP.TXT", WixFileElement.GenerateId(fileName));
		}
		
		[Test]
		public void NewWixFileElement()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\Files\readme.txt");
			Assert.AreEqual("readme.txt", fileElement.Id);
		}
		
		[Test]
		public void LongFileName()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\Files\LongFileName.LongExtension");
			Assert.AreEqual("LongFileName.LongExtension", fileElement.Id);
		}		
	}
}
