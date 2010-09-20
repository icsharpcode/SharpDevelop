// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		[SetUp]
		public void Init()
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
			return 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='Banner' SourceFile='$(var.DATADIR)\\Banner.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
