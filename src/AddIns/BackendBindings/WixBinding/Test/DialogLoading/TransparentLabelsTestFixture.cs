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
	/// Tests that the transparent labels have their alpha part of their BackColor
	/// set to zero.
	/// </summary>
	[TestFixture]
	public class TransparentLabelsTestFixture : DialogLoadingTestFixtureBase
	{
		Color titleLabelColor;
		Color descriptionLabelColor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Label titleLabel = (Label)dialog.Controls[0];
				titleLabelColor = titleLabel.BackColor;
				
				Label descriptionLabel = (Label)dialog.Controls[1];
				descriptionLabelColor = descriptionLabel.BackColor;
			}
		}
		
		[Test]
		public void TitleLabelBackColourIsTransparent()
		{
			Assert.AreEqual(Color.FromArgb(0, SystemColors.Control), titleLabelColor);
		}
		
		[Test]
		public void DescriptionLabelBackColorIsNotTransparent()
		{
			Assert.AreEqual(SystemColors.Control, descriptionLabelColor);
		}
	
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<TextStyle Id='BigFont' FaceName='Verdana' Size='13' Bold='yes' />\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Title' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes' Text='test'/>\r\n" +
				"\t\t\t\t<Control Id='Description' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='no' NoPrefix='yes' Text='test'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
