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

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that we can correctly locate the location of the Product end element.
	/// </summary>
	[TestFixture]
	public class GetProductEndElementLocationTests
	{
		[Test]
		public void EndElementOnOwnLine()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			TextLocation location = wixReader.GetEndElementLocation("Product", "????????-????-????-????-????????????");
			var expectedLocation = new TextLocation(7, 2);
			Assert.AreEqual(expectedLocation, location);
		}
		
		/// <summary>
		/// Should not find an end element location since there is only the start tag.
		/// </summary>
		[Test]
		public void EmptyElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'/>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			TextLocation location = wixReader.GetEndElementLocation("Product", "????????-????-????-????-????????????");
			TextLocation expectedLocation = TextLocation.Empty;
			Assert.AreEqual(expectedLocation, location);
		}
	}
}
