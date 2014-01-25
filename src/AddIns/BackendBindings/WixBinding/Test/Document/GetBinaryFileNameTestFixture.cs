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

using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests the WixDialog.GetBinaryFileName method which returns the binary
	/// filename from the Binary element given an id.
	/// </summary>
	[TestFixture]
	public class GetBinaryFileNameTestFixture
	{
		WixDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			document = new WixDocument();
			document.LoadXml(GetWixXml());
		}
		
		[Test]
		public void MissingBinaryId()
		{
			Assert.AreEqual(null, document.GetBinaryFileName("MissingId"));
		}
		
		[Test]
		public void GetBannerBitmapFileName()
		{
			Assert.AreEqual("Bitmaps/Banner.bmp", document.GetBinaryFileName("Banner"));
		}
		
		[Test]
		public void GetInfoIconFileName()
		{
			Assert.AreEqual("Bitmaps/Info.ico", document.GetBinaryFileName("Info"));
		}
		
		[Test]
		public void SingleQuoteInBinaryFileName()
		{
			Assert.AreEqual(null, document.GetBinaryFileName("test'id"));
		}
		
		[Test]
		public void CreatingWixDocumentWithNullFileLoaderThrowsArgumentNullException()
		{
			IFileLoader fileLoader = null;
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			Assert.Throws<ArgumentNullException>(delegate { WixDocument doc = new WixDocument(project, fileLoader); });
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='Banner' SourceFile='Bitmaps/Banner.bmp'/>\r\n" +
				"\t\t<Binary Id='Info' src='Bitmaps/Info.ico'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
