// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		[ExpectedException(typeof(ArgumentException))]
		public void PassingFileLoaderIntoWixDocument()
		{
			IFileLoader fileLoader = null;
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project, fileLoader);
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
