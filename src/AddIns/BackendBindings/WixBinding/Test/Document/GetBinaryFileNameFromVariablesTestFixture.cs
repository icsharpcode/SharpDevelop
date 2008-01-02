// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Gets the filename when the it is defined inside a preprocessor variable.
	/// </summary>
	[TestFixture]
	public class GetBinaryFileNameFromVariablesTestFixture
	{
		WixDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.SetProperty("DefineConstants", @"DATADIR=Bitmaps;");
			document = new WixDocument(project);
			document.FileName = @"C:\Projects\Setup\Setup.wxs";
			document.LoadXml(GetWixXml());
		}
		
		[Test]
		public void GetBannerBitmapFileName()
		{
			Assert.AreEqual(@"C:\Projects\Setup\Bitmaps\Banner.bmp", document.GetBinaryFileName("Banner"));
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='Banner' SourceFile='$(var.DATADIR)\\Banner.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
