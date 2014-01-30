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
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that a dialog element which is an empty element (no end tag) can 
	/// be located using the WixDocument.GetDialogId method based on the current 
	/// line in the Wix document xml.
	/// </summary>
	[TestFixture]
	public class GetEmptyElementDialogIdAtLineTestFixture
	{
		string expectedDialogId = "WelcomeDialog";
		WixDocumentReader wixReader;
		
		[SetUp]
		public void SetUpFixture()
		{
			wixReader = new WixDocumentReader(GetWixXml());
		}
		
		[Test]
		public void OnDialogStartTagLine()
		{
			Assert.AreEqual(expectedDialogId, wixReader.GetDialogId(9));
		}
		
		[Test]
		public void StartOfDocument()
		{
			Assert.IsNull(wixReader.GetDialogId(1));
		}
		
		[Test]
		public void LineBeforeDialogStartTag()
		{
			Assert.IsNull(wixReader.GetDialogId(8));
		}
		
		[Test]
		public void LineAfterDialogStartTag()
		{
			Assert.IsNull(wixReader.GetDialogId(10));			
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
