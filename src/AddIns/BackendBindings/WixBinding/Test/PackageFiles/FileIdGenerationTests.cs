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
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class FileIdGenerationTests
	{
		WixDocument doc;
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			SD.InitializeForUnitTests();
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
