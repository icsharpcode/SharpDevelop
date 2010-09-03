// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the label font is retrieved using a Property element defined inside the
	/// document.
	/// </summary>
	[TestFixture]
	public class ButtonFontTestFixture : DialogLoadingTestFixtureBase
	{
		string fontName;
		double fontSize;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Button button = (Button)dialog.Controls[0];
				fontName = button.Font.Name;
				fontSize = button.Font.Size;
			}
		}
		
		[Test]
		public void ButtonFontName()
		{
			Assert.AreEqual("Arial", fontName);
		}
		
		[Test]
		public void ButtonFontSize()
		{
			Assert.AreEqual(10.0, fontSize);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Property Id='SmallFont'>{\\SmallFontStyle}</Property>\r\n" +
				"\t\t\t<TextStyle Id='SmallFontStyle' FaceName='Arial' Size='10'/>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' Text='[SmallFont][Button_Next]' X='236' Y='243' Width='60' Height='20' Default='yes'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
