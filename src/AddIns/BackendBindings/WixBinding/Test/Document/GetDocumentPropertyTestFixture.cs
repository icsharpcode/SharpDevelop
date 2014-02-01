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

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDocument.GetProperty returns the inner text of the 
	/// Wix Property element.
	/// </summary>
	[TestFixture]
	public class GetDocumentPropertyTestFixture
	{
		WixDocument doc;
		
		[SetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml(GetWixXml());
		}
		
		[Test]
		public void ButtonCancelProperty()
		{
			Assert.AreEqual("Cancel", doc.GetProperty("Button_Cancel"));
		}
		
		[Test]
		public void MissingProperty()
		{
			Assert.AreEqual(String.Empty, doc.GetProperty("MissingProperty"));
		}		
		
		[Test]
		public void PropertyWithSingleQuoteInName()
		{
			Assert.AreEqual(String.Empty, doc.GetProperty("Test'Property"));
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t\t<Property Id='Button_Cancel'>Cancel</Property>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
