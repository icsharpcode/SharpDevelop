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
	public class LabelFontFromPropertyTestFixture : DialogLoadingTestFixtureBase
	{
		string titleLabelFontName;
		double titleLabelFontSize;
		bool titleLabelFontBold;
		string descLabelFontName;
		double descLabelFontSize;
		bool descLabelFontBold;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Label titleLabel = (Label)dialog.Controls[0];
				titleLabelFontName = titleLabel.Font.Name;
				titleLabelFontSize = titleLabel.Font.Size;
				titleLabelFontBold = titleLabel.Font.Bold;
				
				Label descLabel = (Label)dialog.Controls[1];
				descLabelFontName = descLabel.Font.Name;
				descLabelFontSize = descLabel.Font.Size;
				descLabelFontBold = descLabel.Font.Bold;
			}
		}
	
		[Test]
		public void TitleLabelFontName()
		{
			Assert.AreEqual("Verdana", titleLabelFontName);
		}
		
		[Test]
		public void TitleLabelFontSize()
		{
			Assert.AreEqual(13.0, titleLabelFontSize);
		}
		
		[Test]
		public void TitleLabelFontIsBold()
		{
			Assert.AreEqual(true, titleLabelFontBold);
		}
		
		[Test]
		public void DescriptionLabelFontName()
		{
			Assert.AreEqual("Arial", descLabelFontName);
		}
		
		[Test]
		public void DescriptionLabelFontSize()
		{
			Assert.AreEqual(10.0, descLabelFontSize);
		}
		
		[Test]
		public void DescriptionLabelFontIsBold()
		{
			Assert.AreEqual(false, descLabelFontBold);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Property Id='BigFont'>{&amp;BigFontStyle}</Property>\r\n" +
				"\t\t\t<TextStyle Id='BigFontStyle' FaceName='Verdana' Size='13' Bold='yes' />\r\n" +
				"\t\t\t<Property Id='SmallFont'>{\\SmallFontStyle}</Property>\r\n" +
				"\t\t\t<TextStyle Id='SmallFontStyle' FaceName='Arial' Size='10'/>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Title' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes'>\r\n" +
				"\t\t\t\t\t<Text>[BigFont]Welcome to the [ProductName] installation</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t\t<Control Id='Description' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes'>\r\n" +
				"\t\t\t\t\t<Text>[SmallFont]Install text...</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
